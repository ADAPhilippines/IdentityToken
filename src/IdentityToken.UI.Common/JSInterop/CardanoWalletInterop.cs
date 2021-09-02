using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common.JSInterop
{
    public class CardanoWalletInterop
    {
        public CardanoWalletInterop(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        private IJSRuntime JsRuntime { get; }

        public async ValueTask InitializeAsync(string blockfrostProjectId)
        {
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