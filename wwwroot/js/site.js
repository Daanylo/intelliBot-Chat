let bot;

document.addEventListener('DOMContentLoaded', async () => {
    document.getElementById('help-button').addEventListener('click', showHelp);
    document.getElementById('help-close').addEventListener('click', closeHelp);
    document.getElementById('thanks-close').addEventListener('click', closeThanks);
    document.getElementById('error-close').addEventListener('click', closeError);
    bot = await getBot();
    setInterval(reportStatus, 10000);

    document.getElementById('help-input').addEventListener('keydown', (event) => {
        if (event.target.value.length > 0 ) {
            document.getElementById('help-submit').disabled = false;
        } else {
            document.getElementById('help-submit').disabled = true;
        }
    });
    document.getElementById('help-submit').addEventListener('click', handleHelpSubmit);
});

function handleHelpSubmit() {
    const helpInput = document.getElementById('help-input');
    const helpSubmit = document.getElementById('help-submit');
    const helpContainer = document.getElementById('help-container');
    const helpMessage = helpInput.value;
    helpInput.value = '';
    helpSubmit.disabled = true;
    helpContainer.style.display = 'none';
    fetch('/Index/SubmitHelp', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ message: helpMessage })
    }).then(response => {
        if (response.ok) {
            console.log('Help message sent');
            showThanks();
        } else {
            console.error('Error sending help message:', response.statusText);
            showError('Er is iets misgegaan bij het versturen van uw bericht.');
        }
    });
}

function showThanks() {
    const thanksContainer = document.getElementById('thanks-container');
    const backgroundFilter = document.getElementById('background-filter');
    backgroundFilter.style.display = 'block';
    thanksContainer.style.display = 'block';
}

function closeThanks() {
    const thanksContainer = document.getElementById('thanks-container');
    const backgroundFilter = document.getElementById('background-filter');
    backgroundFilter.style.display = 'none';
    thanksContainer.style.display = 'none';
}

function showHelp() {
    const helpContainer = document.getElementById('help-container');
    const backgroundFilter = document.getElementById('background-filter');
    backgroundFilter.style.display = 'block';
    helpContainer.style.display = 'block';
}

function closeHelp() {
    const helpContainer = document.getElementById('help-container');
    const backgroundFilter = document.getElementById('background-filter');
    backgroundFilter.style.display = 'none';
    helpContainer.style.display = 'none';
}

const getAuthToken = async () => {
    try {
        const response = await fetch('/Index/GetAuthToken');
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        const data = await response.json();
        return data.authToken;
    } catch (error) {
        console.error('Error fetching auth token:', error);
        return null;
    }
}

const getIsBot = async () => {
    try {
        const response = await fetch('/Index/IsBot');
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        const data = await response.json();
        return data.isBot;
    } catch (error) {
        console.error('Error fetching auth token:', error);
        return null;
    }
}

async function reportStatus() {
    let isBot = await getIsBot();
    if (isBot) {
        fetch('/Index/ReportStatus', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then(response => {
            if (response.ok) {
                console.log('Status reported');
            } else {
                console.error('Error reporting status:', response.statusText);
            }
        });
    }
}

function showError(message) {
    const errorContainer = document.getElementById('error-container');
    const errorMessage = document.getElementById('error-message');
    errorMessage.innerText = message;
    errorContainer.style.display = 'block';
}

function closeError() {
    const errorContainer = document.getElementById('error-container');
    errorContainer.style.display = 'none';
}

const getBot = async () => {
    try {
        const response = await fetch('/Index/GetBot');
        const data = await response.json();
        console.log('Success:', data);
        return data;
    } catch (error) {
        console.error('Error:', error);
    }
}
