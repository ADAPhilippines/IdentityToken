using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityToken.UI.Common.Models;
using IdentityToken.UI.Common.Services.JSInterop;
using Microsoft.AspNetCore.Components;

namespace IdentityToken.UI.Common.Components
{
    public partial class Mint
    {
        [Inject] private CardanoWalletInterop? CardanoWalletInterop { get; set; }
        private List<IdentityTokenMetadatum> TokenMetadata { get; set; } = TokenMetadataInitialState;
        private string Message { get; set; } = string.Empty;

        private static List<IdentityTokenMetadatum> TokenMetadataInitialState =>
            new()
            {
                new IdentityTokenMetadatum
                {
                    Key = "username",
                    Value = string.Empty
                },
                new IdentityTokenMetadatum
                {
                    Key = "avatar",
                    Value = string.Empty
                },
                new IdentityTokenMetadatum
                {
                    Key = "facebook_profile",
                    Value = string.Empty
                },
                new IdentityTokenMetadatum
                {
                    Key = "twitter_profile",
                    Value = string.Empty
                },
                new IdentityTokenMetadatum
                {
                    Key = "instagram_profile",
                    Value = string.Empty
                }
            };

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                if (CardanoWalletInterop != null)
                {
                    await CardanoWalletInterop.InjectScriptsAsync();
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
            if (TokenMetadata.GroupBy(m => m.Key.Trim()).Any(c => c.Key.Length > 0 && c.Count() > 1))
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
            var metadataString = JsonSerializer.Serialize(metadataDictionary);

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