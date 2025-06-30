document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('review-form');
    const closeButton = document.getElementById('close-grattitude');
    const skipButton = document.getElementById('skip');
    closeButton.addEventListener('click', navigateHome);
    skipButton.addEventListener('click', navigateHome);


    chooseRandomGif();

    form.addEventListener('submit', (event) => {
        event.preventDefault();
        submitReview();
    });

    document.querySelectorAll('input[name="feedback"]').forEach(item => {
        item.addEventListener('click', () => {
            document.getElementById('submit-review').removeAttribute('disabled');
        });
    });

    getLanguage();
});

function submitReview() {
    const selectedRadio = document.querySelector('input[name="feedback"]:checked');
    const feedbackValue = selectedRadio ? selectedRadio.value : null;
    if (!feedbackValue) {
        return
    }
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
        botId = data.botId;
        console.log('BotId:', botId);
    })
    .catch((error) => {
        console.error('Error:', error);
    });
}

const showGrattitude = () => {
    const grattitudeContainer = document.getElementById('grattitude-container');
    const reviewContainer = document.getElementById('review-container');
    reviewContainer.style.display = 'none';
    grattitudeContainer.style.display = 'flex';
};

const navigateHome = () => {
    window.location.href = `/Index`;
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
        '/resources/reporter.gif',
        '/resources/suicide.gif',
        '/resources/trump.gif'
    ];

    function getRandomGif() {
        const randomIndex = Math.floor(Math.random() * gifs.length);
        return gifs[randomIndex];
    }

    const gifElement = document.getElementById('grattitude-gif');
    gifElement.src = getRandomGif();

};

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

const setLanguage = (language) => {
    switch (language) {
        case 'Nederlands':
            document.getElementById('help-heading').innerText = 'Hulp';
            document.getElementById('help-message').innerText = 'Heb je een probleem met de chatbot? Maak dan een melding hier beneden en we proberen het zo snel mogelijk op te lossen.';
            document.getElementById('help-input').placeholder = 'Omschrijf het probleem';
            document.getElementById('help-submit').innerText = 'Verstuur';
            document.getElementById('thanks-heading').innerText = 'Bedankt!';
            document.getElementById('thanks-message').innerText = 'We hebben je bericht ontvangen en zullen het zo snel mogelijk behandelen.';
            document.getElementById('help-us-h1').innerText = 'Help ons';
            document.getElementById('help-us-p').innerText = 'Wij willen graag weten hoe je de chatbot hebt ervaren. Laat ons weten wat je van de chatbot vindt.';
            document.getElementById('how-satisfied').innerText = 'Hoe tevreden ben je over de antwoorden die je hebt gekregen?';
            document.getElementById('comments-request').innerText = 'Heb je nog opmerkingen?';
            document.getElementById('skip').innerText = 'Overslaan';
            document.getElementById('submit-review').innerText = 'Verstuur';
            document.getElementById('thanks-h1').innerText = 'Bedankt voor je feedback!';
            document.getElementById('thanks-p').innerText = 'Wij waarderen je feedback enorm. Bedankt voor het helpen verbeteren van de chatbot.';
            document.getElementById('review-text').placeholder = 'Laat hier een opmerking achter.';
            document.getElementById('close-grattitude').innerText = 'Sluiten';
            break;
        case 'English':
            document.getElementById('help-heading').innerText = 'Help';
            document.getElementById('help-message').innerText = 'Having an issue with the chatbot? Make a report below and we will try to fix it as soon as possible.';
            document.getElementById('help-input').placeholder = 'Describe the issue';
            document.getElementById('help-submit').innerText = 'Submit';
            document.getElementById('thanks-heading').innerText = 'Thank you!';
            document.getElementById('thanks-message').innerText = 'We have received your message and will address it as soon as possible.';
            document.getElementById('help-us-h1').innerText = 'Help Us';
            document.getElementById('help-us-p').innerText = 'We would like to know how you experienced the chatbot. Let us know what you think about the chatbot.';
            document.getElementById('how-satisfied').innerText = 'How satisfied are you with the answers you received?';
            document.getElementById('comments-request').innerText = 'Do you have any comments?';
            document.getElementById('skip').innerText = 'Skip';
            document.getElementById('submit-review').innerText = 'Submit';
            document.getElementById('thanks-h1').innerText = 'Thank you for your feedback!';
            document.getElementById('thanks-p').innerText = 'We greatly appreciate your feedback. Thank you for helping to improve the chatbot.';
            document.getElementById('review-text').placeholder = 'Leave a comment here.';
            document.getElementById('close-grattitude').innerText = 'Close';
            break;
        case 'Deutsch':
            document.getElementById('help-heading').innerText = 'Hilfe';
            document.getElementById('help-message').innerText = 'Haben Sie ein Problem mit dem Chatbot? Melden Sie es unten und wir werden versuchen, es so schnell wie möglich zu beheben.';
            document.getElementById('help-input').placeholder = 'Beschreiben Sie das Problem';
            document.getElementById('help-submit').innerText = 'Senden';
            document.getElementById('thanks-heading').innerText = 'Danke!';
            document.getElementById('thanks-message').innerText = 'Wir haben Ihre Nachricht erhalten und werden sie so schnell wie möglich bearbeiten.';
            document.getElementById('help-us-h1').innerText = 'Helfen Sie uns';
            document.getElementById('help-us-p').innerText = 'Wir möchten wissen, wie Sie den Chatbot erlebt haben. Lassen Sie uns wissen, was Sie vom Chatbot halten.';
            document.getElementById('how-satisfied').innerText = 'Wie zufrieden sind Sie mit den erhaltenen Antworten?';
            document.getElementById('comments-request').innerText = 'Haben Sie Kommentare?';
            document.getElementById('skip').innerText = 'Überspringen';
            document.getElementById('submit-review').innerText = 'Absenden';
            document.getElementById('thanks-h1').innerText = 'Vielen Dank für Ihr Feedback!';
            document.getElementById('thanks-p').innerText = 'Wir schätzen Ihr Feedback sehr. Vielen Dank, dass Sie zur Verbesserung des Chatbots beitragen.';
            document.getElementById('review-text').placeholder = 'Hinterlassen Sie hier einen Kommentar.';
            document.getElementById('close-grattitude').innerText = 'Schließen';
            break;
        case 'Français':
            document.getElementById('help-heading').innerText = 'Aide';
            document.getElementById('help-message').innerText = 'Vous avez un problème avec le chatbot? Signalez-le ci-dessous et nous essaierons de le résoudre dès que possible.';
            document.getElementById('help-input').placeholder = 'Décrivez le problème';
            document.getElementById('help-submit').innerText = 'Soumettre';
            document.getElementById('thanks-heading').innerText = 'Merci!';
            document.getElementById('thanks-message').innerText = 'Nous avons reçu votre message et le traiterons dès que possible.';
            document.getElementById('help-us-h1').innerText = 'Aidez-Nous';
            document.getElementById('help-us-p').innerText = 'Nous aimerions savoir comment vous avez vécu le chatbot. Faites-nous savoir ce que vous pensez du chatbot.';
            document.getElementById('how-satisfied').innerText = 'Dans quelle mesure êtes-vous satisfait des réponses que vous avez reçues ?';
            document.getElementById('comments-request').innerText = 'Avez-vous des commentaires ?';
            document.getElementById('skip').innerText = 'Passer';
            document.getElementById('submit-review').innerText = 'Soumettre';
            document.getElementById('thanks-h1').innerText = 'Merci pour vos commentaires !';
            document.getElementById('thanks-p').innerText = 'Nous apprécions grandement vos commentaires. Merci d\'aider à améliorer le chatbot.';
            document.getElementById('review-text').placeholder = 'Laissez un commentaire ici.';
            document.getElementById('close-grattitude').innerText = 'Fermer';
            break;
        case 'Español':
            document.getElementById('help-heading').innerText = 'Ayuda';
            document.getElementById('help-message').innerText = '¿Tienes un problema con el chatbot? Informa a continuación e intentaremos solucionarlo lo antes posible.';
            document.getElementById('help-input').placeholder = 'Describe el problema';
            document.getElementById('help-submit').innerText = 'Enviar';
            document.getElementById('thanks-heading').innerText = '¡Gracias!';
            document.getElementById('thanks-message').innerText = 'Hemos recibido tu mensaje y lo abordaremos lo antes posible.';
            document.getElementById('help-us-h1').innerText = 'Ayúdanos';
            document.getElementById('help-us-p').innerText = 'Nos gustaría saber cómo experimentaste el chatbot. Déjanos saber qué piensas del chatbot.';
            document.getElementById('how-satisfied').innerText = '¿Qué tan satisfecho estás con las respuestas que has recibido?';
            document.getElementById('comments-request').innerText = '¿Tienes algún comentario?';
            document.getElementById('skip').innerText = 'Saltar';
            document.getElementById('submit-review').innerText = 'Enviar';
            document.getElementById('thanks-h1').innerText = '¡Gracias por tus comentarios!';
            document.getElementById('thanks-p').innerText = 'Apreciamos enormemente tus comentarios. Gracias por ayudar a mejorar el chatbot.';
            document.getElementById('review-text').placeholder = 'Deja un comentario aquí.';
            document.getElementById('close-grattitude').innerText = 'Cerrar';
            break;
    }
}