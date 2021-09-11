using System;
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
        [Inject] private CardanoWalletInteropService? CardanoWalletInteropService { get; set; }
        private List<IdentityTokenMetadatum> TokenMetadata { get; set; } = TokenMetadataInitialState;
        private string ToastMessage { get; set; } = string.Empty;
        private bool IsToastError { get; set; }
        private bool ShouldShowToast { get; set; }
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

        private bool IsLoading { get; set; }
        private string LoadingMessage { get; set; } = string.Empty;
        
        [Parameter]public EventCallback<EventArgs> MintSuccess { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                if (CardanoWalletInteropService is not null)
                    CardanoWalletInteropService.Error += CardanoWalletInteropOnError;

            await base.OnAfterRenderAsync(firstRender);
        }

        private void CardanoWalletInteropOnError(object? sender, CardanoWalletInteropError e)
        {
            IsLoading = false;
            ShowToast(true, e.Message);
            StateHasChanged();
        }

        private async void OnMintBtnClicked()
        {
            IsLoading = true;
            LoadingMessage = "Validating Inputs...";
            if (TokenMetadata[0].Value.Length == 0 || TokenMetadata[1].Value.Length == 0)
            {
                IsLoading = false;
                ShowToast(true, "Username and Avatar fields are required.");
                return;
            }
            
            if (TokenMetadata.Where(m => m.Key.Length > 0).GroupBy(m => m.Key.Trim())
                .Any(c => c.Key.Length > 0 && c.Count() > 1))
            {
                IsLoading = false;
                ShowToast(true, "Duplicate Keys Detected!");
                return;
            }
            
            var username = TokenMetadata[0].Value;
            var avatar = TokenMetadata[1].Value;
            
            var metadataDictionary = new Dictionary<string, string>();
            foreach (var metadata in TokenMetadata)
            {
                var key = metadata.Key.Trim();
                var value = metadata.Value.Trim();
            
                if (key.Length > 0 
                    && value.Length > 0 
                    && !key.Contains("username") 
                    && !key.Contains("avatar"))
                    metadataDictionary.Add(key, value);
            }
            
            var metadataString = JsonSerializer.Serialize(metadataDictionary);
            
            LoadingMessage = "Building Transaction...";
            if (CardanoWalletInteropService is null) return;
            
            var isWalletConnected = await CardanoWalletInteropService.IsWalletConnectedAsync();
            if (!isWalletConnected)
                isWalletConnected = await CardanoWalletInteropService.ConnectWalletAsync();
            
            if (!isWalletConnected) return;
            
            LoadingMessage = "Signing and Submitting Transaction...";
            await InvokeAsync(StateHasChanged);
            
            var txHash = await CardanoWalletInteropService.MintIdentityTokenAsync($"ID{username}", avatar, metadataString);
            if (txHash is null)
            {
                IsLoading = false;
                ShowToast(true, "Transaction Submission failed!");
                return;
            }
            
            LoadingMessage = $"Transaction Submitted to the Blockchain! Waiting for Confirmation. TxID: {txHash}";
            await InvokeAsync(StateHasChanged);
            
            var tx = await CardanoWalletInteropService.GetTransactionAsync(txHash);
            if (tx is null)
            {
                IsLoading = false;
                ShowToast(true, "Unable to confirm transaction!");
                return;
            }
            
            IsLoading = false;
            TokenMetadata = TokenMetadataInitialState;
            ShowToast(false, $"Minting Transaction Successful!");
            await InvokeAsync(StateHasChanged);

            await Task.Delay(2000);
            await MintSuccess.InvokeAsync();
        }

        private void OnAddNewMetadataBtnClicked()
        {
            TokenMetadata?.Add(new IdentityTokenMetadatum());
        }

        private void OnRemoveMetadataBtnClicked(IdentityTokenMetadatum metadata)
        {
            TokenMetadata?.Remove(metadata);
        }

        private void ShowToast(bool isError, string message)
        {
            IsToastError = isError;
            ToastMessage = message;
            ShouldShowToast = true;
        }
    }
}