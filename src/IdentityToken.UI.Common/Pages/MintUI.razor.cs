using System;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityToken.UI.Common.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common.Pages
{
    public partial class MintUI : IDisposable
    {
        [Inject] private IJSRuntime? JsRuntime { get; set; }
        private CardanoWalletInterop? CardanoWalletInterop { get; set; }
        private string Username { get; set; } = string.Empty;
        private string IPFSHash { get; set; } = string.Empty;
        private bool IsWalletConnected { get; set; }

        public void Dispose()
        {
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                if (JsRuntime != null)
                {
                    CardanoWalletInterop = new CardanoWalletInterop(JsRuntime);
                    await CardanoWalletInterop.InitializeAsync("");
                    IsWalletConnected = await CardanoWalletInterop.IsWalletConnectedAsync();
                    await InvokeAsync(StateHasChanged);
                }

            await base.OnAfterRenderAsync(firstRender);
        }

        private async void OnMintBtnClicked()
        {
            var metadata = new Models.IdentityToken
            {
                Avatar = IPFSHash
            };

            var metadataString = JsonSerializer.Serialize(metadata, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (CardanoWalletInterop != null)
                await CardanoWalletInterop.MintIdentityTokenAsync($"ID{Username}", metadataString);
        }

        private async void OnConnectWalletBtnClicked()
        {
            if (CardanoWalletInterop == null) return;
            await CardanoWalletInterop.ConnectWalletAsync();
            IsWalletConnected = await CardanoWalletInterop.IsWalletConnectedAsync();
            await InvokeAsync(StateHasChanged);
        }
    }
}