window.SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;

const recognition = new SpeechRecognition();
recognition.interimResults = true;
recognition.lang = 'nl-NL';

recognition.addEventListener('result', (event) => {
    const transcript = Array.from(event.results)
        .map(result => result[0])
        .map(result => result.transcript)
        .join('');

    document.getElementById('question').innerHTML =
        `<p>${transcript}</p>`
        ;
});

recognition.addEventListener('end', () => {
    const transcript = document.getElementById('question').innerText;
    fetch('/Home/ProcessTranscript', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(transcript)
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
});

const displayAnswer = (answer) => {
    document.getElementById('output').innerHTML =
        `<p>${answer}</p>`
        ;
}

recognition.start();