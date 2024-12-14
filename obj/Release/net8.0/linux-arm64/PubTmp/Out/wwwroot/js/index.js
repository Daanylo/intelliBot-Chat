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
        fetch('/Home/SetLanguage', {
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
            window.location.href = '/Home/Conversation';
        })
        .catch((error) => {
            console.error('Error:', error);
        });
    });

    const fetchBotData = () => {
        fetch('/api/Bot/GetBot')
            .then(async response => {
                if (!response.ok) {
                    const contentType = response.headers.get('content-type');
                    let errorData;
                    if (contentType && contentType.includes('application/json')) {
                        errorData = await response.json();
                    } else {
                        errorData = await response.text();
                    }
                    errorMessage.style.display = 'flex';
                    errorMessage.querySelector('p').textContent = errorData;
                    throw new Error('Failed to fetch bot data');
                }
                return response.json();
            })
            .then(data => {
                console.log('Bot data:', data);
            })
            .catch(error => {
                console.error('Error:', error);
                setTimeout(fetchBotData, 5000);
            });
    };

    fetchBotData();
    listenForLanguageSelection(submitButton);
});

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
