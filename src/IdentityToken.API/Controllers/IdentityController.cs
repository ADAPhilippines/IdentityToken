using IdentityToken.API.Helpers;
using IdentityToken.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IdentityToken.API.Controllers;


[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    private readonly ILogger<IdentityController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public IdentityController(ILogger<IdentityController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("{walletAddress}")]
    public async Task<IEnumerable<CardanoIdentityToken>> Get(string walletAddress)
    {
        // Create HttpClient
        using var client = _httpClientFactory.CreateClient("blockfrost");

        // Inspect Wallet Address
        using var addressResp = await client.GetAsync($"addresses/{walletAddress}");
        var address = await addressResp.Content.ReadFromJsonAsync<CardanoAddressResponse>();

        // Inspect Assets
        using var assetsResponse = await client.GetAsync($"accounts/{address.StakeAddress}/addresses/assets");
        var addressAssets = await assetsResponse.Content.ReadFromJsonAsync<IEnumerable<CardanoAddressAssetResponse>>();
        var tempIdentityTokens = addressAssets.Where(x => CardanoHelper.IsIdentityToken(x.Unit)).ToList();

        // Initialize Identity Tokens
        var identityTokens = new List<CardanoIdentityToken>();

        // Construct Identity Tokens from Assets
        foreach (var tempIdentityToken in tempIdentityTokens)
        {
            using var assetResponse = await client.GetAsync($"assets/{tempIdentityToken.Unit}");
            var asset = await assetResponse.Content.ReadFromJsonAsync<CardanoAssetResponse>();
            var assetName = CardanoHelper.HexToAscii(asset.AssetName);
            var identityToken = new CardanoIdentityToken
            {
                PolicyId = asset.PolicyId,
                AssetName = assetName
            };

            // Query Asset Metadata
            using var metadataResponse = await client.GetAsync($"txs/{asset.MintTxHash}/metadata");
            var metadata = await metadataResponse.Content.ReadFromJsonAsync<IEnumerable<CardanoTxMetadataResponse>>();

            // Check if metadata contains IdentityToken definition
            foreach (var meta in metadata)
            {
                if (meta.Label == "7368")
                {
                    try
                    {
                        identityToken.Avatar = meta.JsonMetadata
                            .GetProperty(asset.PolicyId)
                            .GetProperty(assetName)
                            .GetProperty("avatar")
                            .GetString();
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Information, ex, "IdentityToken definition not found in metadata");
                    }
                }
            }


            identityTokens.Add(identityToken);
        }

        return identityTokens;
    }
}