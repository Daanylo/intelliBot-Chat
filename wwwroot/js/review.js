document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('review-form');
    const closeButton = document.getElementById('close-grattitude');
    const skipButton = document.getElementById('skip');
    closeButton.addEventListener('click', navigateHome);
    skipButton.addEventListener('click', navigateHome);


    chooseRandomGif();

    form.addEventListener('submit', (event) => {
        event.preventDefault();

        const selectedRadio = document.querySelector('input[name="feedback"]:checked');
        const feedbackValue = selectedRadio ? selectedRadio.value : null;

        const reviewText = document.getElementById('review-text').value;

        console.log('Feedback Value:', feedbackValue);
        console.log('Review Text:', reviewText);

        showGrattitude();

        fetch('/Review/ProcessReview', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ review: feedbackValue, comment: reviewText })
        })
        .then(response => response.json())
        .then(data => {
            console.log('Success:', data);
        })
        .catch((error) => {
            console.error('Error:', error);
        });
    });
});

const showGrattitude = () => {
    const grattitudeContainer = document.getElementById('grattitude-container');
    const reviewContainer = document.getElementById('review-container');
    reviewContainer.style.display = 'none';
    grattitudeContainer.style.display = 'flex';
};

const navigateHome = () => {
    window.location.href = '/Index/Index';
};

const chooseRandomGif = () => {
    const gifs = [
        '/resources/pedro.gif',
        '/resources/happy-bot.gif',
        '/resources/elmo.gif',
        '/resources/biden-smile.gif',
        '/resources/apple-dog.gif',
        '/resources/wazowski-mike.gif',
        '/resources/hawk-tuah.gif',
        '/resources/reporter.gif'
    ];

    function getRandomGif() {
        const randomIndex = Math.floor(Math.random() * gifs.length);
        return gifs[randomIndex];
    }

    const gifElement = document.getElementById('grattitude-gif');
    gifElement.src = getRandomGif();

};