export async function injectCardanoWalletInteropAsync() {
    return new Promise((resolve, reject) => {
        if (!window.CardanoWalletInterop) {
            let customScript = document.createElement('script');
            customScript.setAttribute('src', './_content/IdentityToken.UI.Common/dist/app.js');
            customScript.onload = () => resolve();
            document.head.appendChild(customScript);
        }
    });
}
