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
        token.classList.add('ui-draggable');
        token.classList.add('ui-draggable-handle');
        token.style.position = 'absolute';
        token.style.zIndex = 99;
        document.getElementById('map-board').appendChild(token);
    }

    if (token) {
        // % based positioning.
        token.style.left = `${broadCastData.tokenLeftPerc}%`;
        token.style.top = `${broadCastData.tokenTopPerc}%`;
        token.classList.add('draggable-token');
        token.classList.add('ui-draggable');
        token.classList.add('ui-draggable-handle');
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

// Explicitly sets the visibility instead of toggling to avoid sync issues.
export const setTokenVisibility = (broadCastData) => {
    const tokenGettingToggled = document.getElementById(broadCastData.tokenId);
    if (tokenGettingToggled) {
        if (!broadCastData.isVisible) {
            tokenGettingToggled.classList.add('hidden');
        } else {
            tokenGettingToggled.classList.remove('hidden');
        }
    }
};

// Updates the Z-Index of a token
export const updateZIndex = (broadCastData) => {
    const token = document.getElementById(broadCastData.tokenId);
    if (token) {
        token.style.zIndex = broadCastData.zIndex;
    }
};

// For syncing the player board to the DM board. Runs on initial load and after every save so that the player board stays up to date with the DM board.
export const syncBoard = (allTokens) => {
    const board = document.getElementById('map-board');
    // Get the tokens from the player view, and the token IDs from the DM view, then compare them.
    const currentTokensOnBoard = board.querySelectorAll('.draggable-token');
    const incomingIdsFromDM = Object.keys(allTokens);

    // Call the removeToken function for any token that is on the player view but not in the incoming data from the DM view.
    currentTokensOnBoard.forEach(tokenElement => {
        if (!incomingIdsFromDM.includes(tokenElement.id)) {

            // Call Remove token in the same way that the BC channel would call it.
            removeToken({ tokenId: tokenElement.id });
        }
    });

    // Adds and updates tokens based on data from the DM view.
    Object.entries(allTokens).forEach(([htmlId, token]) => {
        let tokenImg = document.getElementById(htmlId);

        if (!tokenImg && token.src) {
            tokenImg = document.createElement('img');
            tokenImg.id = htmlId;
            tokenImg.src = token.src;
            tokenImg.classList.add('draggable-token');
            token.classList.add('ui-draggable');
            token.classList.add('ui-draggable-handle');
            tokenImg.style.position = 'absolute';
            tokenImg.style.zIndex = parseInt(token.zIndex) || 99;
            board.appendChild(tokenImg);
        }

        if (tokenImg) {
            tokenImg.style.left = `${token.x}%`;
            tokenImg.style.top = `${token.y}%`;
            tokenImg.classList.add('draggable-token');
            token.classList.add('ui-draggable');
            token.classList.add('ui-draggable-handle');
            tokenImg.style.zIndex = parseInt(token.zIndex) || 99;

            if (!token.isVisible) {
                tokenImg.classList.add('hidden');
            } else {
                tokenImg.classList.remove('hidden');
            }
        }
    });
};

export const removeToken = (broadCastData) => {
    // Find the token using tokenId from the passed in data, then remove it from the DOM.
    const token = document.getElementById(broadCastData.tokenId);

    console.log("Removing token with id:", broadCastData.tokenId);

    if (token) {
        token.remove();
        console.log("Token successfully removed from Player View.");
    } else {
        console.warn("Could not find token to remove:", broadCastData.tokenId);
    }
};