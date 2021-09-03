using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common.JSInterop
{
    public class CardanoWalletInterop
    {
        private IJSRuntime JsRuntime { get; }
        
        private readonly Lazy<Task<IJSObjectReference>> _bootstrapModuleTask;
        
        public CardanoWalletInterop(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
            _bootstrapModuleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/IdentityToken.UI.Common/bootstrap.js").AsTask());
        }

        public async ValueTask InitializeAsync(string blockfrostProjectId)
        {
            var module = await _bootstrapModuleTask.Value;
            await module.InvokeVoidAsync("injectCardanoWalletInterop");
            await JsRuntime.InvokeVoidAsync("CardanoWalletInterop.InitializeAsync", blockfrostProjectId);
        }

        public async ValueTask<string> MintIdentityTokenAsync(string assetName, string metadata)
        {
            return await JsRuntime
                .InvokeAsync<string>("CardanoWalletInterop.MintIdentityTokenAsync", assetName, metadata);
        }

        public async ValueTask<bool> IsWalletConnectedAsync()
        {
            return await JsRuntime.InvokeAsync<bool>("CardanoWalletInterop.IsWalletConnectedAsync");
        }

        public async ValueTask<bool> ConnectWalletAsync()
        {
            return await JsRuntime.InvokeAsync<bool>("CardanoWalletInterop.ConnectWalletAsync");
        }
    }
}