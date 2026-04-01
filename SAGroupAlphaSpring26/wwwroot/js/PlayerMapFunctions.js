// PlayerMapFunctions.js

/**
 * Takes the broadcast data and positions tokens on player map accordingly. 
 * Additionally, if the token doesn't exist it now creates it.
 * @param {any} broadCastData the data received from broadcast channel post.
 */
export const repositionToken = (broadCastData) => {
    let token = document.getElementById(broadCastData.tokenId);

    // Added Creation event for new Tokens.
    // If no existing token -> Create player view token.
    if (!token && broadCastData.tokenImgSrc) {
        token = document.createElement('img');
        token.id = broadCastData.tokenId;
        token.src = broadCastData.tokenImgSrc;
        token.classList.add('draggable-token');
        token.style.position = 'absolute';
        token.style.zIndex = 99;
        document.getElementById('map-board').appendChild(token);
    }

    if (token) {
        // % based positioning.
        token.style.left = `${broadCastData.tokenLeftPerc}%`;
        token.style.top = `${broadCastData.tokenTopPerc}%`;
        token.style.width = '5%';
        token.style.height = 'auto';
    }
};

// Toggles visibility of the tokens.
export const toggleTokenInvisibility = (broadCastData) => {
    const tokenGettingToggled = document.getElementById(broadCastData.tokenId);
    if (tokenGettingToggled) {
        // toggles the hidden css.
        tokenGettingToggled.classList.toggle('hidden');
    }
};