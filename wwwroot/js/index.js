document.querySelectorAll('.language').forEach(language => {
    language.addEventListener('click', (event) => {
        document.querySelectorAll('.language').forEach(lang => {
            lang.classList.remove('active');
            lang.classList.add('inactive');
        });
        event.currentTarget.classList.remove('inactive');
        event.currentTarget.classList.add('active');
        const selectedLanguage = event.currentTarget.id;
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
            console.log('Success:', data);
            startAnimation();
        })
        .catch((error) => {
            console.error('Error:', error);
        });
    });
});

const startAnimation = () => {
    document.querySelector('.overlay').classList.add('animate');
    setTimeout(() => {
        document.querySelector('.overlay').classList.remove('animate');
    }, 5000);
    setTimeout(() => {
        window.location.href = '/Home/Conversation';
    }, 2000);
}

document.addEventListener('DOMContentLoaded', () => {
    const audio = document.getElementById('audio');
    const startButton = document.querySelector('.start-button');

    startButton.addEventListener('click', () => {
        audio.play().catch(error => {
            console.error('Audio play failed:', error);
        });

        document.body.classList.add('start-animations');

        startButton.style.display = 'none';
    });
});
