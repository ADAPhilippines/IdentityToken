using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using IdentityToken.UI.Common.Services;
using IdentityToken.Common.Models;
using IdentityToken.Common.Helpers;

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

        private int AssetPage { get; set; } = 1;
        private int AssetRequestLimit { get; set; } = 20;

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

                    IEnumerable<CardanoAssetResponse>? assetsResult = null;
                    do
                    { 
                        assetsResult = await AuthService.GetProfileAssetsAsync(Username, AssetRequestLimit, AssetPage++);
                        if (assetsResult is not null)
                        {
                            Assets.AddRange(assetsResult);
                        }

                        IsInitialAssetsLoading = false;
                        await InvokeAsync(StateHasChanged);
                    } while (assetsResult is not null && assetsResult.Count() >= AssetRequestLimit);
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private bool IsNFT(CardanoAssetResponse asset)
        {
            return asset.OnChainMetadata?
                .TryGetProperty("image", out _) ?? false;
        }

        private bool IsVideoNFT(CardanoAssetResponse asset)
        {
            try
            {
                var fileEnumerator = asset.OnChainMetadata?.GetProperty("files").EnumerateArray();
                if (fileEnumerator is null) return false;

                return (from JsonElement file in fileEnumerator
                        select file
                            .GetProperty("mediaType")
                            .GetString())
                    .Any(mediaTypeValue => mediaTypeValue is not null && mediaTypeValue == "video/mp4");
            }
            catch
            {
                Console.WriteLine($"Error Parsing Asset: {CardanoHelper.HexToAscii(asset.AssetName ?? string.Empty)}");
                return false;
            }
        }

        private string GetNFTVideoHash(CardanoAssetResponse asset)
        {
            try
            {
                var fileEnumerator = asset.OnChainMetadata?.GetProperty("files").EnumerateArray();
                if (fileEnumerator is null) return string.Empty;

                foreach (
                    var src in from JsonElement file
                        in fileEnumerator
                    select file.GetProperty("src").GetString()
                )
                {
                    return src?.Replace("ipfs://", string.Empty) ?? string.Empty;
                }
            }
            catch
            {
                Console.WriteLine($"Error Parsing Asset: {CardanoHelper.HexToAscii(asset.AssetName ?? string.Empty)}");
            }
            return string.Empty;
        }

        private string GetNFTImageHash(CardanoAssetResponse asset)
        {
            try
            {
                return (asset.OnChainMetadata?
                        .GetProperty("image")
                        .GetString() ?? string.Empty)
                    .Replace("ipfs://ipfs/", "")
                    .Replace("ipfs://", "");
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetAssetQuantity(CardanoAssetResponse asset)
        {
            var quantityString = asset.Quantity;
            if (quantityString == null) return string.Empty;
            var quantity = ulong.Parse(quantityString);
            return quantity.ToString("N");
        }
    }
}