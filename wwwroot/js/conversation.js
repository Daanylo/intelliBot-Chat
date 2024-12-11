window.SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;

const recognition = new SpeechRecognition();
recognition.interimResults = true;
recognition.lang = 'nl-NL';
let awaitingUserAction = false;
let isListening = false;
let userStoppedRecognition = false; // Flag to indicate if recognition was stopped by the user

recognition.addEventListener('result', (event) => {
    const transcript = Array.from(event.results)
        .map(result => result[0])
        .map(result => result.transcript)
        .join('');

    const question = document.querySelector('.conversation-container').lastChild;
    question.querySelector('p').textContent = transcript;
    document.querySelector('.conversation-container').scrollTop = document.querySelector('.conversation-container').scrollHeight;
});

document.addEventListener('DOMContentLoaded', () => {
    var newAnswer = document.createElement('div');
    newAnswer.classList.add('conversation-box', 'answer');
    newAnswer.innerHTML =
        `<div class="conversation-message">
            <p>Hi, I'm intelliBot. How can I help you today?</p>
        </div>`;
    document.querySelector('.conversation-container').appendChild(newAnswer);
    var newQuestion = document.createElement('div');
    newQuestion.classList.add('conversation-box', 'question');
    newQuestion.innerHTML =
    `<div class="conversation-message" id="question">
            <p></p>
     </div>`;
    document.querySelector('.conversation-container').appendChild(newQuestion);

    document.querySelector('.microphone-button').addEventListener('click', () => {
        if (!awaitingUserAction && !isListening) {
            recognition.start();
        } else if (!awaitingUserAction && isListening) {
            userStoppedRecognition = true; // Set the flag to true when the user stops recognition
            recognition.stop();
        }
    });
});

recognition.addEventListener('start', () => {
    isListening = true;
    userStoppedRecognition = false; // Reset the flag when recognition starts
    startMicrophoneVisualization();
    console.log('Speech recognition started');
});

recognition.addEventListener('end', async () => {
    if (userStoppedRecognition) {
        isListening = false;
        document.querySelector('.microphone-button').style.transform = 'scale(1)';
        document.querySelector('.microphone-button').classList.remove('active');
        return; // Exit early if recognition was stopped by the user
    }

    const question = document.querySelector('.conversation-container').lastChild;
    const transcript = question.querySelector('p').textContent;
    if (!transcript || transcript === "" && isListening) {
        recognition.start();
        return;
    }
    isListening = false;
    document.querySelector('.microphone-button').style.transform = 'scale(1)';
    document.querySelector('.microphone-button').classList.remove('active');
    document.querySelectorAll('.gui-button').forEach(button => 
        button.classList.add('active'));
    try {
        const action = await waitForUserAction();
        if (action === 'submit') {
            sendQuestion(transcript);
        } else if (action === 'retry') {
            question.querySelector('p').textContent = "";
            recognition.start();
        }
    } finally {
        awaitingUserAction = false;
        document.querySelectorAll('.gui-button').forEach(button => 
            button.classList.remove('active'));
    }
});

function waitForUserAction() {
    awaitingUserAction = true;
    return new Promise((resolve) => {
        document.getElementById('retry').addEventListener('click', () => {
            resolve('retry');
        });
        document.getElementById('submit').addEventListener('click', () => {
            resolve('submit');
        });
    });
}

const sendQuestion = (question) => {
    displayAnswer("I'm thinking...");
    return;

    fetch('/Home/ProcessTranscript', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(question)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('Success:', data);
            displayAnswer(data.answer);
        })
        .catch((error) => {
            console.error('Error:', error);
        });
}

const displayAnswer = (answer) => {
    var newAnswer = document.createElement('div');
    newAnswer.classList.add('conversation-box', 'answer');
    newAnswer.innerHTML =
        `<div class="conversation-message" id="answer">
            <p>${answer}</p>
        </div>`;
    const container = document.querySelector('.conversation-container');
    container.appendChild(newAnswer);
    var newQuestion = document.createElement('div');
    newQuestion.classList.add('conversation-box', 'question');
    newQuestion.innerHTML =
    `<div class="conversation-message" id="question">
            <p></p>
     </div>`;
    container.appendChild(newQuestion);
    container.scrollTop = container.scrollHeight;
}

document.getElementById("finish-conversation").addEventListener('click', () => {
    window.location.href = '/Home/Review';
});

function startMicrophoneVisualization() {
    const element = document.querySelector('.microphone-button');
    element.classList.add('active');
    navigator.mediaDevices.getUserMedia({ audio: true })
      .then(stream => {
        const audioContext = new (window.AudioContext || window.webkitAudioContext)();
        const analyser = audioContext.createAnalyser();
        const source = audioContext.createMediaStreamSource(stream);

        source.connect(analyser);
        analyser.fftSize = 256;

        const dataArray = new Uint8Array(analyser.frequencyBinCount);

        function updatePosition() {
          if (!isListening) {
            return;
          }

          analyser.getByteFrequencyData(dataArray);

          // Get average volume
          const volume = dataArray.reduce((acc, val) => acc + val, 0) / dataArray.length;

          // Normalize the volume to a scale between 1 and 1.5
          const minScale = 1; // Minimum scale
          const maxScale = 5; // Maximum scale
          const normalizedVolume = Math.min(volume / 255, 1); // Ensure volume is capped at 1
          const scale = minScale + (normalizedVolume * (maxScale - minScale));

          // Apply the scale transformation
          element.style.transform = `scale(${scale})`;

          requestAnimationFrame(updatePosition);
        }

        updatePosition();
      })
      .catch(err => {
        console.error('Error accessing the microphone:', err);
        alert('Microphone access is required for this demo.');
      });
}