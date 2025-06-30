let selectedLanguage;
let dropDownOpen = false;

document.addEventListener('DOMContentLoaded', async () => {
    const submitButton = document.getElementById('submit-language');
    selectedLanguage = 'Nederlands';
    setLanguage(selectedLanguage);
    showQR();

    document.querySelector('.language.cover').addEventListener('click', () => {
        if (dropDownOpen) {
            closeDropDown(document.querySelector('.language.cover'));
        } else {
            openDropDown();
        }
    });
    listenForLanguageSelection(submitButton);

    submitButton.addEventListener('click', () => {
        submitLanguage();
    });

});

const submitLanguage = () => {
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
    .then(() => {
        window.location.href = '/Conversation';
    })
    .catch((error) => {
        console.error('Error:', error);
    });
};

const showQR = async () => {
    if (await getIsBot()) {
        document.querySelector('.qr-container').style.display = 'flex';
        const generateQRCode = async () => {
            const qrCodeElement = document.getElementById("qrcode");
            if (qrCodeElement.firstChild) {
                qrCodeElement.innerHTML = '';
            }
            const authToken = await getAuthToken();
            new QRCode(qrCodeElement, {
                text: "https://chat.intelliguide.nl/" + authToken,
                width: 128,
                height: 128,
                colorDark: "#000000",
                colorLight: "#ffffff",
                correctLevel: QRCode.CorrectLevel.H
            });
            setTimeout(generateQRCode, 10000);
        };
        generateQRCode();
    }
};

const openDropDown = () => {
    dropDownOpen = true;
    let languagesList = document.querySelector('.languages-list');
    languagesList.style.maxHeight = '300px';
    document.getElementById('dropdown-icon').style.transform = 'rotate(-180deg)';
};

const closeDropDown = (target) => {
    dropDownOpen = false;
    document.querySelector('.languages-list').classList.remove('active');
    document.getElementById('dropdown-icon').style.transform = 'rotate(0deg)';
    document.querySelector('.languages-list').style.maxHeight = '0';
    if (target.id === selectedLanguage) {
        return;
    }
    document.querySelectorAll('.language').forEach(lang => {
        lang.classList.remove('current');
    });
    target.classList.add('current');
    selectedLanguage = target.id;
    setLanguageCover(selectedLanguage);
};

const listenForLanguageSelection = (submitButton) => {
    document.querySelectorAll('.languages-list .language').forEach(language => {
        language.addEventListener('click', (event) => {
            closeDropDown(event.currentTarget);
            setLanguage(selectedLanguage);
        });
    });
};

const setLanguageCover = (selectedLanguage) => {
    let cover = document.querySelector('.language.cover');
    cover.id = selectedLanguage;
    switch (selectedLanguage) {
        case 'Nederlands':
            cover.innerHTML =
            `
                <img src="/resources/dutch_flag.png" alt="dutch_flag">
                <p>${selectedLanguage}</p>
                <i class="fa-solid fa-chevron-down" id="dropdown-icon"></i>
            `
            break;
        case 'English':
            cover.innerHTML =
            `
                <img src="/resources/uk_flag.png" alt="uk_flag">
                <p>${selectedLanguage}</p>
                <i class="fa-solid fa-chevron-down" id="dropdown-icon"></i>
            `
            break;
        case 'Deutsch':
            cover.innerHTML =
            `
                <img src="/resources/german_flag.png" alt="german_flag">
                <p>${selectedLanguage}</p>
                <i class="fa-solid fa-chevron-down" id="dropdown-icon"></i>
            `
            break;
        case 'Français':
            cover.innerHTML =
            `
                <img src="/resources/french_flag.png" alt="french_flag">
                <p>${selectedLanguage}</p>
                <i class="fa-solid fa-chevron-down" id="dropdown-icon"></i>
            `
            break;
        case 'Español':
            cover.innerHTML =
            `
                <img src="/resources/spanish_flag.png" alt="spanish_flag">
                <p>${selectedLanguage}</p>
                <i class="fa-solid fa-chevron-down" id="dropdown-icon"></i>
            `
            break;
    }
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
            document.getElementById('qr-info').innerText = 'Neem je mij liever mee op je smartphone? Scan dan de QR-code.';
            document.getElementById('help').innerText = 'Hulp';
            document.getElementById('heading').innerText = 'Selecteer een taal';
            document.getElementById('paragraph').innerText = 'De chatbot zal in deze taal antwoorden.';
            document.getElementById('start-conversation').innerText = 'Start gesprek';
            document.getElementById('agreement').innerHTML = 'Door op "Start gesprek" te klikken, ga je akkoord met de <a href="https://www.intelliguide.nl/voorwaarden.php" id="terms">gebruiksvoorwaarden</a>.';
        break;
        case 'English':
            document.getElementById('help-heading').innerText = 'Help';
            document.getElementById('help-message').innerText = 'Having an issue with the chatbot? Make a report below and we will try to fix it as soon as possible.';
            document.getElementById('help-input').placeholder = 'Describe the issue';
            document.getElementById('help-submit').innerText = 'Submit';
            document.getElementById('thanks-heading').innerText = 'Thank you!';
            document.getElementById('thanks-message').innerText = 'We have received your message and will address it as soon as possible.';
            document.getElementById('qr-info').innerText = 'Prefer to take me with you on your phone? Scan the QR code.';
            document.getElementById('help').innerText = 'Help';
            document.getElementById('heading').innerText = 'Select a language';
            document.getElementById('paragraph').innerText = 'The chatbot will respond in this language.';
            document.getElementById('start-conversation').innerText = 'Start conversation';
            document.getElementById('agreement').innerHTML = 'By clicking "Start conversation", you agree to the <a href="https://www.intelliguide.nl/voorwaarden.php" id="terms">terms of use</a>.';
        break;
        case 'Deutsch':
            document.getElementById('help-heading').innerText = 'Hilfe';
            document.getElementById('help-message').innerText = 'Haben Sie ein Problem mit dem Chatbot? Melden Sie es unten und wir werden versuchen, es so schnell wie möglich zu beheben.';
            document.getElementById('help-input').placeholder = 'Beschreiben Sie das Problem';
            document.getElementById('help-submit').innerText = 'Senden';
            document.getElementById('thanks-heading').innerText = 'Danke!';
            document.getElementById('thanks-message').innerText = 'Wir haben Ihre Nachricht erhalten und werden sie so schnell wie möglich bearbeiten.';
            document.getElementById('qr-info').innerText = 'Möchten Sie mich lieber auf Ihrem Smartphone mitnehmen? Scannen Sie den QR-Code.';
            document.getElementById('help').innerText = 'Hilfe';
            document.getElementById('heading').innerText = 'Wählen Sie eine Sprache';
            document.getElementById('paragraph').innerText = 'Der Chatbot wird in dieser Sprache antworten.';
            document.getElementById('start-conversation').innerText = 'Gespräch beginnen';
            document.getElementById('agreement').innerHTML = 'Durch Klicken auf "Gespräch beginnen" stimmen Sie den <a href="https://www.intelliguide.nl/voorwaarden.php" id="terms">Nutzungsbedingungen</a> zu.';
        break;
        case 'Français':
            document.getElementById('help-heading').innerText = 'Aide';
            document.getElementById('help-message').innerText = 'Vous avez un problème avec le chatbot? Signalez-le ci-dessous et nous essaierons de le résoudre dès que possible.';
            document.getElementById('help-input').placeholder = 'Décrivez le problème';
            document.getElementById('help-submit').innerText = 'Soumettre';
            document.getElementById('thanks-heading').innerText = 'Merci!';
            document.getElementById('thanks-message').innerText = 'Nous avons reçu votre message et le traiterons dès que possible.';
            document.getElementById('qr-info').innerText = 'Préférez-vous m\'emmener sur votre téléphone? Scannez le code QR.';
            document.getElementById('help').innerText = 'Aide';
            document.getElementById('heading').innerText = 'Sélectionnez une langue';
            document.getElementById('paragraph').innerText = 'Le chatbot répondra dans cette langue.';
            document.getElementById('start-conversation').innerText = 'Commencer la conversation';
            document.getElementById('agreement').innerHTML = 'En cliquant sur "Commencer la conversation", vous acceptez les <a href="https://www.intelliguide.nl/voorwaarden.php" id="terms">conditions d\'utilisation</a>.';
        break;
        case 'Español':
            document.getElementById('help-heading').innerText = 'Ayuda';
            document.getElementById('help-message').innerText = '¿Tienes un problema con el chatbot? Informa a continuación e intentaremos solucionarlo lo antes posible.';
            document.getElementById('help-input').placeholder = 'Describe el problema';
            document.getElementById('help-submit').innerText = 'Enviar';
            document.getElementById('thanks-heading').innerText = '¡Gracias!';
            document.getElementById('thanks-message').innerText = 'Hemos recibido tu mensaje y lo abordaremos lo antes posible.';
            document.getElementById('qr-info').innerText = '¿Prefieres llevarme contigo en tu teléfono? Escanea el código QR.';
            document.getElementById('help').innerText = 'Ayuda';
            document.getElementById('heading').innerText = 'Selecciona un idioma';
            document.getElementById('paragraph').innerText = 'El chatbot responderá en este idioma.';
            document.getElementById('start-conversation').innerText = 'Iniciar conversación';
            document.getElementById('agreement').innerHTML = 'Al hacer clic en "Iniciar conversación", aceptas los <a href="https://www.intelliguide.nl/voorwaarden.php" id="terms">términos de uso</a>.';
        break;
    }
    
}
