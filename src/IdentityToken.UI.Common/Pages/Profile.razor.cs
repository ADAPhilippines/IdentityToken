using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using IdentityToken.UI.Common.Services;
using IdentityToken.Common.Models;

namespace IdentityToken.UI.Common.Pages
{
    public partial class Profile
    {
        [Parameter] public string Username { get; set; } = string.Empty;
        [Inject] private AuthService? AuthService { get; set; }

        private IdentityProfile? CurrentProfile { get; set; }

        private List<CardanoAssetResponse> Assets { get; set; } =
            new List<CardanoAssetResponse>();

        private bool IsInitialAssetsLoading { get; set; } = true;

        private string FormattedTotalAda
        {
            get
            {
                var balance = CurrentProfile?.Balance ?? 0;
                var result = ((decimal)balance / (decimal)1000000);
                return result.ToString("N", CultureInfo.CurrentCulture);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (AuthService is not null)
                {
                    CurrentProfile = await AuthService.GetProfileAsync(Username);
                    await InvokeAsync(StateHasChanged);
                    
                    var assetsResult = await AuthService.GetProfileAssetsAsync(Username);
                    if (assetsResult is not null)
                    {
                        Assets.AddRange(assetsResult);
                    }

                    IsInitialAssetsLoading = false;
                    await InvokeAsync(StateHasChanged);
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        
    }
}