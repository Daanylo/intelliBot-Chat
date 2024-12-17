let selectedLanguage;

document.addEventListener('DOMContentLoaded', () => {
    const audio = document.getElementById('audio');
    const startButton = document.getElementById('start-button');
    const errorMessage = document.getElementById('error-message');
    const closeErrorButton = document.getElementById('close-error');
    const submitButton = document.getElementById('submit-language');

    startButton.addEventListener('click', () => {
        audio.play().catch(error => {
            console.error('Audio play failed:', error);
        });

        document.body.classList.add('start-animations');

        startButton.style.display = 'none';
    });

    closeErrorButton.addEventListener('click', () => {
        errorMessage.style.display = 'none';
    });

    submitButton.addEventListener('click', () => {
        if (!selectedLanguage) {
            return;
        }
        fetch('/Index/SetLanguage', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ language: selectedLanguage })
        })
        .then(async response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const text = await response.text();
            return text ? JSON.parse(text) : {};
        })
        .then(data => {
            window.location.href = '/Conversation/Conversation';
        })
        .catch((error) => {
            console.error('Error:', error);
        });
    });
    initializeBot();
    listenForLanguageSelection(submitButton);
});

async function initializeBot() {
    try {
        const response = await fetch('/api/bot/initialize', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        const data = await response.json();
        console.log(data.message);
        console.log(data.data);
    } catch (error) {
        console.error('Error initializing bot:', error);
    }
}

const listenForLanguageSelection = (submitButton) => {
    document.querySelectorAll('.language').forEach(language => {
        language.addEventListener('click', (event) => {
            document.querySelectorAll('.language').forEach(lang => {
                lang.classList.remove('active');
            });
            event.currentTarget.classList.add('active');
            selectedLanguage = event.currentTarget.id;
            submitButton.classList.add('active');
        });
    });
};
