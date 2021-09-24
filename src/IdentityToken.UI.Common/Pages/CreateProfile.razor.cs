using System;
using Microsoft.AspNetCore.Components;
using IdentityToken.UI.Common.Services;

namespace IdentityToken.UI.Common.Pages
{
    public partial class CreateProfile
    {
        [Inject] private AuthService? AuthService { get; set; }
        [Inject] private NavigationManager? NavigationManager { get; set; }

        private string WalletAddress { get; set; } = string.Empty;
        private bool IsError { get; set; } = false;
        
        private async void OnBtnCreateClicked()
        {
            try
            {
                if (AuthService is null) return;
                var profile = await AuthService.CreateProfileAsync(WalletAddress);

                if (profile is null || NavigationManager is null) return;
                NavigationManager.NavigateTo($"/{profile.Username}");
            }
            catch
            {
                IsError = true;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}