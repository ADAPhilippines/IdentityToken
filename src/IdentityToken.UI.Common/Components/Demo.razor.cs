using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using IdentityToken.UI.Common.Services;

namespace IdentityToken.UI.Common.Components
{
    public partial class Demo : IDisposable
    {
        [Inject] private AuthService? AuthService { get; set; }
        [Inject] private ILocalStorageService? LocalStorageService { get; set; }
        public bool IsLoggedIn { get; set; } = false;

        public void Dispose()
        {
            if (AuthService is null) return;
            AuthService.Authenticated -= OnAuthenticated;
            AuthService.LoggedOut -= OnLoggedOut;
        }

        protected override void OnInitialized()
        {
            if (AuthService is not null)
            {
                AuthService.Authenticated += OnAuthenticated;
                AuthService.LoggedOut += OnLoggedOut;
            }

            base.OnInitialized();
        }

        private void OnLoggedOut(object? sender, EventArgs e)
        {
            IsLoggedIn = false;
            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (LocalStorageService is null) return;
                IsLoggedIn = await LocalStorageService.ContainKeyAsync("identity");
                await InvokeAsync(StateHasChanged);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private void OnAuthenticated(object? sender, EventArgs e)
        {
            IsLoggedIn = true;
            StateHasChanged();
        }
    }
}