using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityToken.UI.Common.JSInterop;
using IdentityToken.UI.Common.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common.Pages
{
    public partial class MintUI : IAsyncDisposable
    {
        [Inject] private IJSRuntime? JsRuntime { get; set; }
        private CardanoWalletInterop? CardanoWalletInterop { get; set; }
        private string Username { get; set; } = string.Empty;
        private string Avatar { get; set; } = string.Empty;
        private bool IsWalletConnected { get; set; }

        public async ValueTask DisposeAsync()
        {
            if (CardanoWalletInterop != null)
                await CardanoWalletInterop.DisposeAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                if (JsRuntime != null)
                {
                    CardanoWalletInterop = new CardanoWalletInterop(JsRuntime);
                    await CardanoWalletInterop.InitializeAsync("3Ojodngr06BReeSN9lhsow0hypKf8gu5");
                    IsWalletConnected = await CardanoWalletInterop.IsWalletConnectedAsync();
                    await InvokeAsync(StateHasChanged);
                    CardanoWalletInterop.Error += CardanoWalletInteropOnError;
                }

            await base.OnAfterRenderAsync(firstRender);
        }

        private void CardanoWalletInteropOnError(object? sender, object e)
        {
            Console.WriteLine(e.ToString());
        }

        private async void OnMintBtnClicked()
        {
            var metadata = new Models.IdentityToken
            {
                Avatar = Avatar
            };

            var metadataString = JsonSerializer.Serialize(metadata, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            if (CardanoWalletInterop != null)
            {
                var tx = await CardanoWalletInterop.MintIdentityTokenAsync($"ID{Username}", metadataString);
            }
        }

        private async void OnConnectWalletBtnClicked()
        {
            if (CardanoWalletInterop == null) return;
            CardanoWalletInterop.Error -= CardanoWalletInteropOnError;
            await CardanoWalletInterop.ConnectWalletAsync();
            IsWalletConnected = await CardanoWalletInterop.IsWalletConnectedAsync();
            await InvokeAsync(StateHasChanged);
        }
    }
}