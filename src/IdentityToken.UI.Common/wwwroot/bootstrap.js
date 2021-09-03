export async function injectCardanoWalletInterop() {
    if(!window.CardanoWalletInterop)
    {
        var customScript = document.createElement('script');
        customScript.setAttribute('src', './_content/IdentityToken.UI.Common/dist/app.js');
        document.head.appendChild(customScript);
        
        await new Promise((resolve => {
            setInterval(() => {
                if(window.CardanoWalletInterop) resolve();
            }, 10);
        }));
    }
}
