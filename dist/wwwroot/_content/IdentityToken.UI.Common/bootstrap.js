export async function injectApplicationScriptAsync(blockfrostProjectId) {
    return new Promise((resolve, reject) => {
        if (!window.CardanoWalletInterop) {
            let customScript = document.createElement('script');
            customScript.setAttribute('src', './_content/IdentityToken.UI.Common/dist/app.js');
            customScript.onload = async () => {
                await window.CardanoWalletInterop.InitializeAsync(blockfrostProjectId);
                resolve();
            }
            document.head.appendChild(customScript);
        }
    });
}

export async function injectScriptAsync(url, isModule = false) {
    return new Promise((resolve, reject) => {
        let script = document.createElement('script');
        script.setAttribute('src', url);
        script.setAttribute('type', isModule ? 'module' : 'text/javascript');
        script.onload = () => resolve();
        document.head.appendChild(script);
    });
}

export async function injectStyleSheetAsync(url) {
    return new Promise((resolve, reject) => {
        let styleSheet = document.createElement('link');
        styleSheet.setAttribute('href', url);
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
        preconnectLink1.onload = () => {
            let preconnectLink2 = document.createElement('link');
            preconnectLink2.setAttribute('href', 'https://fonts.gstatic.com');
            preconnectLink2.setAttribute('rel', 'preconnect');
            preconnectLink2.crossOrigin = true;
            preconnectLink2.onload = () => {
                let styleSheet = document.createElement('link');
                styleSheet.setAttribute('href', url);
                styleSheet.setAttribute('rel', 'stylesheet');
                styleSheet.onload = () => resolve();
                document.head.appendChild(styleSheet);
            };
            document.head.appendChild(preconnectLink2);
        };
        document.head.appendChild(preconnectLink1);
    });
}

export async function injectPrismJSAsync() {
    return new Promise((resolve, reject) => {
        let prismJs = document.createElement('script');
        prismJs.setAttribute('src', 'https://cdnjs.cloudflare.com/ajax/libs/prism/1.24.1/prism.min.js');
        prismJs.setAttribute('data-manual', "true");
        prismJs.onload = () => {
            let prismJsAutoloader = document.createElement('script');
            prismJsAutoloader.setAttribute('src', 'https://cdnjs.cloudflare.com/ajax/libs/prism/1.24.1/plugins/autoloader/prism-autoloader.min.js');
            prismJsAutoloader.onload = () => {
                resolve();
            };
            document.head.appendChild(prismJsAutoloader);
        };
        document.head.appendChild(prismJs);
    });
}