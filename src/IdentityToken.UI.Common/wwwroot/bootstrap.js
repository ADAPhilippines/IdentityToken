﻿export async function injectCardanoWalletInteropAsync(blockfrostProjectId, objectRef) {
    return new Promise((resolve, reject) => {
        if (!window.CardanoWalletInterop) {
            let customScript = document.createElement('script');
            customScript.setAttribute('src', './_content/IdentityToken.UI.Common/dist/app.js');
            customScript.onload = async () =>
            {
                await window.CardanoWalletInterop.InitializeAsync(blockfrostProjectId, objectRef);
                resolve();
            }
            document.head.appendChild(customScript);
        }
    });
}

export async function injectStyleSheetAsync(path) {
    return new Promise((resolve, reject) => {
            let styleSheet = document.createElement('link');
            styleSheet.setAttribute('href', path);
            styleSheet.setAttribute('rel', 'stylesheet');
            styleSheet.onload = () => resolve();
            document.head.appendChild(styleSheet);
    });
}

export async function injectGoogleFontAsync(url) {
    return new Promise((resolve, reject) => {
        let preconnectLink1 = document.createElement('link');
        preconnectLink1.setAttribute('href', 'https://fonts.googleapis.com');
        preconnectLink1.setAttribute('rel', 'preconnect');
        preconnectLink1.onload = () => resolve();
        document.head.appendChild(preconnectLink1);

        let preconnectLink2 = document.createElement('link');
        preconnectLink2.setAttribute('href', 'https://fonts.gstatic.com');
        preconnectLink2.setAttribute('rel', 'preconnect');
        preconnectLink2.crossOrigin = true;
        preconnectLink2.onload = () => resolve();
        document.head.appendChild(preconnectLink2);
        
        let styleSheet = document.createElement('link');
        styleSheet.setAttribute('href', url);
        styleSheet.setAttribute('rel', 'stylesheet');
        styleSheet.onload = () => resolve();
        document.head.appendChild(styleSheet);
    });
}