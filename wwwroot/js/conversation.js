window.SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;

const recognition = new SpeechRecognition();
recognition.interimResults = true;
recognition.lang = 'nl-NL';
let listeningForQuestion = false;
let userStoppedRecognition = false;
let listeningForWakeWord = false;
let isBot;
let transcript = "";
let foundWakeWord = false;
let wakingWords = ['hey', 'hello', 'hé', 'hallo', 'hi', 'yo', 'hola', 'bonjour', 'salut'];
let sendWords = ['send', 'verstuur', 'verzenden', 'submit', 'stuur', 'verzend'];

document.addEventListener('DOMContentLoaded', async () => {
    isBot = await getIsBot();
    getLanguage();
    startRecognition(false);

    document.querySelector('.microphone-button').addEventListener('click', () => {
        if (!listeningForQuestion) {
            startRecognition(true);
        } else {
            userStoppedRecognition = true;
            recognition.stop();
        }
    });

    document.getElementById("finish-conversation").addEventListener('click', () => {
        FinishConversation();
    });

    document.getElementById('submit').addEventListener('click', () => {
        let input = document.getElementById('input').value;
        if (!input || input === "") {
            return;
        }
        sendQuestion(input);
    });

    document.getElementById('input').addEventListener('keydown', (event) => {
        const input = document.getElementById('input');
        if (!listeningForQuestion) {
            showSendButton();
            if (event.key === 'Backspace' && input.value.length === 1) {
                showMicrophone();
            } else if (input.value === "") {
                showMicrophone();
            }
        }
    });
});

function showMicrophone() {
    document.getElementById('submit').classList.remove('active');
    document.querySelector('.microphone-container').style.display = 'flex';
}


function startRecognition(forQuestion) {
    if (!forQuestion) {
        listeningForWakeWord = true;
        listeningForQuestion = false;
    } else {
        listeningForWakeWord = false;
        listeningForQuestion = true;
        userStoppedRecognition = false;
        foundWakeWord = false;
        showMicrophone();
        startMicrophoneVisualization();
        console.log('Speech recognition started');
    }
    recognition.start();
}

recognition.addEventListener('result', (event) => {
    transcript = Array.from(event.results)
    .map(result => result[0])
    .map(result => result.transcript)
    .join('');

    const input = document.getElementById('input');

    console.log('Transcript:', transcript); 
    if (listeningForWakeWord) {
        for (let word of wakingWords) {
            if (transcript.toLowerCase().includes(word)) {
                foundWakeWord = true;
                transcript = "";
                input.value = "";
                recognition.stop();
                return;
            }
        }
        for (let word of sendWords) {
            if (transcript.toLowerCase().includes(word)) {
                foundWakeWord = false;
                listeningForWakeWord = true;
                sendQuestion(input.value);
                transcript = "";
                return;
            }
        }
        return;
    }
    if (listeningForQuestion) {

        input.value = transcript;
    }
});

recognition.addEventListener('end', async () => {
    if (foundWakeWord) {
        startRecognition(true);
        return;
    }
    if (listeningForWakeWord) {
        transcript = [];
        startRecognition(false);
        return;
    }
    if (listeningForQuestion) {
        if (userStoppedRecognition) {
            listeningForQuestion = false;
            document.getElementById('microphone-backdrop').style.transform = 'scale(1)';
            document.getElementById('microphone-backdrop').style.display = 'none';
            document.querySelector('.microphone-button').classList.remove('active');
            startRecognition(false);
            return;
        }

        const input = document.getElementById('input');
        if (!input.value || input.value === "") {
            startRecognition(true);
        } else {
            startRecognition(false);
            showSendButton();
        }
    }
});

recognition.addEventListener('start', (event) => {
    if (listeningForQuestion) {
        const audio = new Audio('/resources/ping_sound.mp3');
        audio.play().catch(error => {
            console.error('Audio play failed:', error);
        });
    }
});

function showSendButton() {
    document.getElementById('microphone-backdrop').style.transform = 'scale(1)';
    document.getElementById('microphone-backdrop').style.display = 'none';
    document.querySelector('.microphone-button').classList.remove('active');
    document.querySelector('.microphone-container').style.display = 'none';
    document.getElementById('submit').classList.add('active');
}


const sendQuestion = (question) => {
    if (sendQuestion.isSending) return;
    sendQuestion.isSending = true;
    document.getElementById('submit').classList.remove('active');
    document.getElementById('microphone-container').style.display = 'flex';
    addMessage(question, true);
    document.getElementById('input').value = "";
    addThinkingMessage();
    fetch('/Conversation/ProcessTranscript', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(question)
    })
        .then(response => {
            sendQuestion.isSending = false;
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('Success:', data);
            document.querySelector('.conversation-container').removeChild(document.querySelector('.thinking'));
            displayAnswer(data.answer, data.conversationId, data.messageId);
        })
        .catch((error) => {
            console.error('Error:', error);
        });
}

const displayAnswer = (answer, conversationId, messageId) => {
    const uniqueQuery = `?t=${Date.now()}`;
    const audioUrl = `/resources/${conversationId}/${messageId}.mp3${uniqueQuery}`;

    fetch(audioUrl, { method: 'HEAD' })
        .then(response => {
            if (response.ok) {
                const audio = new Audio(audioUrl);
                audio.play().catch(error => {
                    console.error('Audio play failed:', error);
                });
            } else {
                showError('Kon audio niet afspelen');
            }
        })
        .catch(error => {
            console.error('Error checking audio file:', error);
            showError('Kon audio niet afspelen');
        });

    addMessage(answer, false);
    const container = document.querySelector('.conversation-wrapper');
    container.scrollTop = container.scrollHeight;
}



const FinishConversation = () => {
    fetch('/Conversation/FinishConversation', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            console.log('Response received:', response);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('Success:', data);
            window.location.href = '/Review';
        })
        .catch((error) => {
            console.error('Error:', error);
        });
}


function startMicrophoneVisualization() {
    const element = document.querySelector('.microphone-button');
    const backdrop = document.getElementById('microphone-backdrop');
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
                if (!listeningForQuestion) {
                    return;
                }

                analyser.getByteFrequencyData(dataArray);

                const volume = dataArray.reduce((acc, val) => acc + val, 0) / dataArray.length;

                const minScale = 1;
                const maxScale = 5;
                const normalizedVolume = Math.min(volume / 255, 1);
                const scale = minScale + (normalizedVolume * (maxScale - minScale));

                backdrop.style.display = 'block';
                backdrop.style.transform = `scale(${scale})`;

                requestAnimationFrame(updatePosition);
            }

            updatePosition();
        })
        .catch(err => {
            console.error('Error accessing the microphone:', err);
            alert('Microphone access is required for this demo.');
        });
}

function autoType(element, typingSpeed) {
    const container = document.querySelector('.conversation-wrapper');
    element.style.position = "relative";
    element.style.display = "inline-block";

    var textElement = element.querySelector('.message-text');
    var text = textElement.textContent.trim().split('');
    var amntOfChars = text.length;
    var newString = "";
    textElement.textContent = "";

    textElement.style.opacity = 1;
    for (var i = 0; i < amntOfChars; i++) {
        (function (i, char) {
            setTimeout(function () {
                newString += char;
                textElement.textContent = newString;
                container.scrollTop = container.scrollHeight; // Keep the container scrolled to the bottom
            }, i * typingSpeed);
        })(i + 1, text[i]);
    }
}

const addMessage = (message, isUser) => {
    const newMessage = document.createElement('div');
    newMessage.classList.add('conversation-box', isUser ? 'question' : 'answer');
    newMessage.innerHTML = isUser
        ? `
        <div class="conversation-message">
            <p class="message-text">${message}</p>
        </div>`
        : `
        <div class="message-top">
            <span>
                <img class="bot-avatar" src="${bot.avatar}" alt="Bot Avatar">
                <p class="bot-name">${bot.name}</p>
           </span>
            <p class="message-time">${new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</p>
        </div>
        <div class="conversation-message">
            <p class="message-text">${message}</p>
        </div>
        `;
    document.querySelector('.conversation-container').appendChild(newMessage);
    if (!isUser) {
        autoType(newMessage, 50);
    }
    if (message.includes('http') && isBot) {
        let link = message.match(/http\S+/)[0];
        if (link.endsWith('.')) {
            link = link.slice(0, -1);
        }
        const linkDiv = document.createElement('div');
        linkDiv.classList.add('qr-code');
        new QRCode(linkDiv, {
            text: link,
            width: 128,
            height: 128,
            colorDark: "#000000",
            colorLight: "#ffffff",
            correctLevel: QRCode.CorrectLevel.H
        });
        newMessage.querySelector('.conversation-message').appendChild(linkDiv);
    }
}

function addThinkingMessage() {
    const newMessage = document.createElement('div');
    newMessage.classList.add('conversation-box', 'thinking');
    newMessage.innerHTML = `
        <div class="conversation-message">
            <div id="lottie-container" class="lottie-container"></div>
        </div>
    `;
    document.querySelector('.conversation-container').appendChild(newMessage);
    document.querySelector('.conversation-wrapper').scrollTop = document.querySelector('.conversation-wrapper').scrollHeight;

    lottie.loadAnimation({
        container: document.getElementById('lottie-container'), // The container element
        renderer: 'svg', // Choose 'svg', 'canvas', or 'html'
        loop: true,      // Set to true if you want the animation to loop
        autoplay: true,  // Set to true to start animation automatically
        path: 'resources/loading-anim.json' // Path to your Lottie JSON file
});

}

const getGreeting = async () => {
    try {
        const response = await fetch('/Conversation/GetGreeting', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        console.log('Success:', data);
        return data.greeting;
    } catch (error) {
        console.error('Error:', error);
    }
}


const getLanguage = () => {
    fetch('/Index/GetLanguage', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('Success:', data);
            setLanguage(data.language);
        })
        .catch((error) => {
            console.error('Error:', error);
        });
}

const setLanguage = async (language) => {
    let greeting = await getGreeting();
    switch (language) {
        case 'Nederlands':
            recognition.lang = 'nl-NL';
            addMessage(greeting, false);
            document.getElementById('help-heading').innerText = 'Hulp';
            document.getElementById('help-message').innerText = 'Heb je een probleem met de chatbot? Maak dan een melding hier beneden en we proberen het zo snel mogelijk op te lossen.';
            document.getElementById('help-input').placeholder = 'Omschrijf het probleem';
            document.getElementById('help-submit').innerText = 'Verstuur';
            document.getElementById('thanks-heading').innerText = 'Bedankt!';
            document.getElementById('thanks-message').innerText = 'We hebben je bericht ontvangen en zullen het zo snel mogelijk behandelen.';
            document.getElementById('help').innerText = 'Hulp';
            document.getElementById('finish-conversation-text').innerText = 'Gesprek beëindigen';
            document.getElementById('input').placeholder = 'Typ hier uw vraag...';
            break;
        case 'English':
            recognition.lang = 'en-US';
            addMessage('Hello, how can I help you today?', false);
            document.getElementById('help-heading').innerText = 'Help';
            document.getElementById('help-message').innerText = 'Having an issue with the chatbot? Make a report below and we will try to fix it as soon as possible.';
            document.getElementById('help-input').placeholder = 'Describe the issue';
            document.getElementById('help-submit').innerText = 'Submit';
            document.getElementById('thanks-heading').innerText = 'Thank you!';
            document.getElementById('thanks-message').innerText = 'We have received your message and will address it as soon as possible.';
            document.getElementById('help').innerText = 'Help';
            document.getElementById('finish-conversation-text').innerText = 'End conversation';
            document.getElementById('input').placeholder = 'Type your question here...';
            break;
        case 'Deutsch':
            recognition.lang = 'de-DE';
            addMessage('Hallo, wie kann ich Ihnen heute helfen?', false);
            document.getElementById('help-heading').innerText = 'Hilfe';
            document.getElementById('help-message').innerText = 'Haben Sie ein Problem mit dem Chatbot? Melden Sie es unten und wir werden versuchen, es so schnell wie möglich zu beheben.';
            document.getElementById('help-input').placeholder = 'Beschreiben Sie das Problem';
            document.getElementById('help-submit').innerText = 'Senden';
            document.getElementById('thanks-heading').innerText = 'Danke!';
            document.getElementById('thanks-message').innerText = 'Wir haben Ihre Nachricht erhalten und werden sie so schnell wie möglich bearbeiten.';
            document.getElementById('help').innerText = 'Hilfe';
            document.getElementById('finish-conversation-text').innerText = 'Gespräch beenden';
            document.getElementById('input').placeholder = 'Geben Sie hier Ihre Frage ein...';
            break;
        case 'Français':
            recognition.lang = 'fr-FR';
            addMessage('Bonjour, comment puis-je vous aider aujourd\'hui?', false);
            document.getElementById('help-heading').innerText = 'Aide';
            document.getElementById('help-message').innerText = 'Vous avez un problème avec le chatbot? Signalez-le ci-dessous et nous essaierons de le résoudre dès que possible.';
            document.getElementById('help-input').placeholder = 'Décrivez le problème';
            document.getElementById('help-submit').innerText = 'Soumettre';
            document.getElementById('thanks-heading').innerText = 'Merci!';
            document.getElementById('thanks-message').innerText = 'Nous avons reçu votre message et le traiterons dès que possible.';
            document.getElementById('help').innerText = 'Aide';
            document.getElementById('finish-conversation-text').innerText = 'Terminer la conversation';
            document.getElementById('input').placeholder = 'Tapez votre question ici...';
            break;
        case 'Español':
            recognition.lang = 'es-ES';
            addMessage('Hola, ¿cómo puedo ayudarte hoy?', false);
            document.getElementById('help-heading').innerText = 'Ayuda';
            document.getElementById('help-message').innerText = '¿Tienes un problema con el chatbot? Informa a continuación e intentaremos solucionarlo lo antes posible.';
            document.getElementById('help-input').placeholder = 'Describe el problema';
            document.getElementById('help-submit').innerText = 'Enviar';
            document.getElementById('thanks-heading').innerText = '¡Gracias!';
            document.getElementById('thanks-message').innerText = 'Hemos recibido tu mensaje y lo abordaremos lo antes posible.';
            document.getElementById('help').innerText = 'Ayuda';
            document.getElementById('finish-conversation-text').innerText = 'Finalizar conversación';
            document.getElementById('input').placeholder = 'Escribe tu pregunta aquí...';
            break;
    }

}