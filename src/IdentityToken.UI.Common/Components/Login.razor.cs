using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using IdentityToken.UI.Common.Models;
using Microsoft.AspNetCore.Components;
using IdentityToken.UI.Common.Services;
using IdentityToken.UI.Common.Services.JSInterop;

namespace IdentityToken.UI.Common.Components
{
    public partial class Login : IDisposable
    {
        [Inject] private AuthService? AuthService { get; set; }
        [Inject] private HelperInteropService? HelperInteropService { get; set; }
        [Inject] private ILocalStorageService? LocalStorageService { get; set; }
        [Inject] private CardanoWalletInteropService? CardanoWalletInteropService { get; set; }
        private string WalletAddress { get; set; } = string.Empty;
        private string WalletAddressQr { get; set; } = string.Empty;
        private bool HasJustCopied { get; set; } = false;
        private bool ShouldAuthorizeAttempt { get; set; } = true;
        private string LoadingMessage { get; set; } = string.Empty;
        private bool IsLoading { get; set; }
        private bool IsTxFailed { get; set; }
        
        public void Dispose()
        {
            ShouldAuthorizeAttempt = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                if (AuthService is not null && HelperInteropService is not null)
                {
                    WalletAddress = await AuthService.RequestLoginAsync();
                    WalletAddressQr = await HelperInteropService.GenerateQrDataUrlAsync(WalletAddress);
                    await InvokeAsync(StateHasChanged);
                    _ = StartAuthorizeAttemptAsync();
                }

            await base.OnAfterRenderAsync(firstRender);
        }

        private async void OnBtnCopyAddressClicked()
        {
            if (HelperInteropService is null) return;
            await HelperInteropService.CopyToClipboardAsync(WalletAddress);
            HasJustCopied = true;
            await InvokeAsync(StateHasChanged);
        }

        private async void OnBtnConnectWithCardanoClicked()
        {
            if (CardanoWalletInteropService is null) return;
            ShouldAuthorizeAttempt = false;
            IsLoading = true;
            LoadingMessage = "Signing and Submitting Transaction...";
            await InvokeAsync(StateHasChanged);
            var txHash = await CardanoWalletInteropService.SendAdaAsync(new[]
            {
                new TxOutput()
                {
                    Address = WalletAddress,
                    Amount = 1200000
                }
            });

            if (txHash is not null && AuthService is not null && LocalStorageService is not null)
            {
                LoadingMessage = $"Transaction Submitted to the Blockchain! Confirming Tx: {txHash}";
                await InvokeAsync(StateHasChanged);
                await CardanoWalletInteropService.GetTransactionAsync(txHash);
                var identity = await AuthService.Authorize(WalletAddress);
                await LocalStorageService.SetItemAsync("identity", identity);
                AuthService.Authorized();
            }
            else
            {
                IsTxFailed = true;
            }
            
            IsLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        private async Task StartAuthorizeAttemptAsync()
        {
            while (ShouldAuthorizeAttempt)
            {
                await Task.Delay(10000);
                try
                {
                    if (AuthService is not null && LocalStorageService is not null)
                    {
                        var identity = await AuthService.Authorize(WalletAddress);
                        await LocalStorageService.SetItemAsync("identity", identity);
                        ShouldAuthorizeAttempt = false;
                        AuthService.Authorized();
                        await InvokeAsync(StateHasChanged);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}