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

// For syncing the player board to the DM board. Runs on initial load and after every save so that the player board stays up to date with the DM board.
export const syncBoard = (allTokens) => {
    // Loop through the dictionary the DM sent
    Object.entries(allTokens).forEach(([htmlId, token]) => {

        let tokenImg = document.getElementById(htmlId);

        // Creates the token if it doesn't exist
        if (!tokenImg && token.src) {
            tokenImg = document.createElement('img');
            tokenImg.id = htmlId;
            tokenImg.src = token.src;
            tokenImg.classList.add('draggable-token');
            tokenImg.style.position = 'absolute';
            tokenImg.style.zIndex = 99;
            document.getElementById('map-board').appendChild(tokenImg);
        }

        if (tokenImg) {
            // sets positioning of the token.
            tokenImg.style.left = `${token.x}%`;
            tokenImg.style.top = `${token.y}%`;
            tokenImg.style.width = '5%';
            tokenImg.style.height = 'auto';

            // sets the visibility of the token.
            if (!token.isVisible) {
                tokenImg.classList.add('hidden');
            } else {
                tokenImg.classList.remove('hidden');
            }
        }
    });
};