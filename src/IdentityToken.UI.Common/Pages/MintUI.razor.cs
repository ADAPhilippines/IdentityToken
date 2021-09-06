using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityToken.UI.Common.JSInterop;
using IdentityToken.UI.Common.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common.Pages
{
    public partial class MintUI
    {
        [Inject] private IJSRuntime? JsRuntime { get; set; }
        private CardanoWalletInterop? CardanoWalletInterop { get; set; }
        private List<IdentityTokenMetadatum>? TokenMetadata { get; set; }
        private string Message { get; set; } = string.Empty;
        private static List<IdentityTokenMetadatum> TokenMetadataInitialState =>
            new ()
            {
                new()
                {
                    Key = "username",
                    Value = string.Empty
                },
                new()
                {
                    Key = "avatar",
                    Value = string.Empty
                },
                new()
                {
                    Key = "twitter_profile",
                    Value = string.Empty
                },
                new()
                {
                    Key = "facebook_profile",
                    Value = string.Empty
                },
                new()
                {
                    Key = "instagram_profile",
                    Value = string.Empty
                }
            };

        protected override void OnInitialized()
        {
            TokenMetadata = TokenMetadataInitialState;
            base.OnInitialized();
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                if (JsRuntime != null)
                {
                    CardanoWalletInterop = new CardanoWalletInterop(JsRuntime);
                    await CardanoWalletInterop.InitializeAsync("Fg86lNzv47LnrMQ6o03YvXcfQ9t2whDf");
                    CardanoWalletInterop.Error += CardanoWalletInteropOnError;
                }

            await base.OnAfterRenderAsync(firstRender);
        }

        private void CardanoWalletInteropOnError(object? sender, CardanoWalletInteropError e)
        {
            Message = e.Message;
            StateHasChanged();
        }

        private async void OnMintBtnClicked()
        {
            if (TokenMetadata == null) return;

            if (TokenMetadata.GroupBy(m => m.Key.Trim()).Any(c => c.Count() > 1))
            {
                Message = "Duplicate Keys Detected!";
                return;
            }

            if (TokenMetadata[0].Value.Length == 0 || TokenMetadata[1].Value.Length == 0)
            {
                Message = "Username and Avatar fields are required.";
                return;
            }

            var username = TokenMetadata[0].Value;

            var metadataDictionary = new Dictionary<string, string>();
            foreach (var metadata in TokenMetadata)
            {
                var key = metadata.Key.Trim();
                var value = metadata.Value.Trim();

                if (key.Length > 0 && value.Length > 0 && !key.Contains("username")) metadataDictionary.Add(key, value);
            }

            metadataDictionary["avatar"] = $"ipfs://{metadataDictionary["avatar"]}";
            var metadataString = JsonSerializer.Serialize(metadataDictionary, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (CardanoWalletInterop == null) return;

            if (!await CardanoWalletInterop.IsWalletConnectedAsync())
                await CardanoWalletInterop.ConnectWalletAsync();
            
            Message = "Submitting Transaction...";
            await InvokeAsync(StateHasChanged);
            
            var tx = await CardanoWalletInterop.MintIdentityTokenAsync($"ID{username}", metadataString);
            if (tx == null) return;
            
            TokenMetadata = TokenMetadataInitialState;
            Message = $"Minting Transaction Successful! txId: {tx.Hash}";
            await InvokeAsync(StateHasChanged);
        }

        private void OnAddNewMetadataBtnClicked()
        {
            TokenMetadata?.Add(new IdentityTokenMetadatum());
        }

        private void OnRemoveMetadataBtnClicked(IdentityTokenMetadatum metadata)
        {
            TokenMetadata?.Remove(metadata);
        }
    }
}