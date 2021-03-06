using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityToken.API.Data;
using IdentityToken.Common.Helpers;
using IdentityToken.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityToken.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProfileController : ControllerBase
{
    private readonly ILogger<IdentityController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IdentityDbContext _identityDbContext;

    public ProfileController(ILogger<IdentityController> logger, IHttpClientFactory httpClientFactory, IdentityDbContextFactory identityDbContextFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _identityDbContext = identityDbContextFactory.CreateDbContext();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfileAsync([FromBody] CreateProfileRequestBody body)
    {
        var paymentAddress = body.PaymentAddress;

        // Create HttpClient
        using var client = _httpClientFactory.CreateClient("blockfrost_mainnet");

        // Inspect Wallet Address
        var stakeAddress = string.Empty;
        try
        {
            using var poolPmWalletClient = _httpClientFactory.CreateClient();
            var poolPmWalletResponse = await poolPmWalletClient.GetFromJsonAsync<JsonElement>($"https://api.pool.pm/wallet/{paymentAddress}");
            stakeAddress = poolPmWalletResponse.GetProperty("addr").GetString();
        }
        catch
        {
            return BadRequest("Address may be invalid.");
        }

        // Inspect Assets
        Profile? profile = null;
        var accountAssetsPage = 1;
        var IsIdentityTokenFound = false;
        while (!IsIdentityTokenFound)
        {
            var addressAssets = await client
                .GetFromJsonAsync<IEnumerable<CardanoAddressAssetResponse>>($"accounts/{stakeAddress}/addresses/assets?order=desc&page={accountAssetsPage++}");

            if (addressAssets is null) return BadRequest();

            var responseIdentityTokens = addressAssets?.Where(x => CardanoHelper.IsIdentityToken(x.Unit)).ToList();

            if (responseIdentityTokens is not null)
            {
                foreach (var identityToken in responseIdentityTokens)
                {
                    if (identityToken?.Unit is null) continue;

                    //Search through asset history to find the latest valid IdentityToken metadata
                    var assetHistoryPage = 1;
                    while (true)
                    {
                        var assetHistory = await client
                            .GetFromJsonAsync<IEnumerable<CardanoAssetHistoryResponse>>($"assets/{identityToken.Unit}/history?order=desc&page={assetHistoryPage++}");

                        if (assetHistory is null) continue;

                        foreach (var entry in assetHistory.Where(x => x.Action == "minted").ToList())
                        {
                            if (entry is null) continue;

                            var metadata = await client.GetFromJsonAsync<IEnumerable<CardanoTxMetadataResponse>>($"txs/{entry.TxHash}/metadata");
                            if (metadata is null) continue;

                            foreach (var meta in metadata)
                            {
                                if (meta.Label == "7368")
                                {
                                    var assetName = CardanoHelper.HexToAscii(identityToken.Unit[56..]);
                                    var isMetadataValid = false;
                                    try
                                    {
                                        var identityTokenMeta = meta.JsonMetadata
                                                .GetProperty(identityToken.Unit[..56])
                                                .GetProperty(assetName)
                                                .GetProperty("avatar")
                                                .GetProperty("src")
                                                .ToString();

                                        identityTokenMeta = meta.JsonMetadata
                                                .GetProperty(identityToken.Unit[..56])
                                                .GetProperty(assetName)
                                                .GetProperty("avatar")
                                                .GetProperty("protocol")
                                                .ToString();

                                        isMetadataValid = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Log(LogLevel.Information, ex, "IdentityToken definition not found in metadata");
                                    }

                                    if (isMetadataValid)
                                    {
                                        assetName = assetName[2..];
                                        var checkSum = CardanoHelper.GetShortHash($"{stakeAddress}{assetName}");
                                        var username = $"{assetName}-{checkSum.ToLower()}";

                                        if (_identityDbContext.Profiles is null) return StatusCode(500);
                                        profile = await _identityDbContext.Profiles.FirstOrDefaultAsync(p => p.Username == username);
                                        if (profile is null)
                                        {
                                            profile = new()
                                            {
                                                Username = username,
                                                PaymentAddress = paymentAddress,
                                                StakeAddress = stakeAddress ?? string.Empty,
                                            };
                                            _identityDbContext.Profiles?.Add(profile);
                                            await _identityDbContext.SaveChangesAsync();
                                        }
                                        IsIdentityTokenFound = true;
                                    }
                                }
                                if (IsIdentityTokenFound) break;
                            }
                            if (IsIdentityTokenFound) break;
                        }
                        if (assetHistory.Count() < 100 || IsIdentityTokenFound) break;
                    }
                    if (IsIdentityTokenFound) break;
                }
            }
            if (addressAssets is not null && addressAssets.Count() < 100) break;
        }

        if (profile is null) return BadRequest("No IdentityToken found.");

        return Ok(profile);
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetProfileAsync(string username)
    {
        if (_identityDbContext.Profiles is null) return StatusCode(500);
        var profile = await _identityDbContext.Profiles.FirstOrDefaultAsync(p => p.Username == username);

        if (profile is null) return NotFound();

        // Create HttpClient
        using var client = _httpClientFactory.CreateClient("blockfrost_mainnet");

        var accountAssetsPage = 1;
        var IsIdentityTokenFound = false;
        CardanoIdentityToken? identityToken = null;
        while (!IsIdentityTokenFound)
        {
            var addressAssets = await client
                .GetFromJsonAsync<IEnumerable<CardanoAddressAssetResponse>>($"accounts/{profile.StakeAddress}/addresses/assets?order=desc&page={accountAssetsPage++}");

            if (addressAssets is null) return BadRequest();

            var responseIdentityTokens = addressAssets?.Where(x => CardanoHelper.IsIdentityToken(x.Unit)).ToList();

            if (responseIdentityTokens is not null)
            {
                foreach (var responseIdentityToken in responseIdentityTokens)
                {
                    if (responseIdentityToken?.Unit is null) continue;

                    //Search through asset history to find the latest valid IdentityToken metadata
                    var assetHistoryPage = 1;
                    while (true)
                    {
                        var assetHistory = await client
                        .GetFromJsonAsync<IEnumerable<CardanoAssetHistoryResponse>>($"assets/{responseIdentityToken.Unit}/history?order=desc&page={assetHistoryPage++}");

                        if (assetHistory is null || !assetHistory.Any()) continue;

                        foreach (var entry in assetHistory.Where(x => x.Action == "minted").ToList())
                        {
                            if (entry is null) continue;

                            var metadata = await client.GetFromJsonAsync<IEnumerable<CardanoTxMetadataResponse>>($"txs/{entry.TxHash}/metadata");
                            if (metadata is null) continue;

                            foreach (var meta in metadata)
                            {
                                if (meta.Label == "7368")
                                {
                                    var assetName = CardanoHelper.HexToAscii(responseIdentityToken.Unit[56..]);
                                    try
                                    {
                                        identityToken = new()
                                        {
                                            PolicyId = responseIdentityToken.Unit[..56],
                                            AssetName = assetName,
                                            Avatar = new()
                                            {
                                                Source = meta.JsonMetadata
                                                    .GetProperty(responseIdentityToken.Unit[..56])
                                                    .GetProperty(assetName)
                                                    .GetProperty("avatar")
                                                    .GetProperty("src")
                                                    .ToString(),
                                                Protocol = meta.JsonMetadata
                                                    .GetProperty(responseIdentityToken.Unit[..56])
                                                    .GetProperty(assetName)
                                                    .GetProperty("avatar")
                                                    .GetProperty("protocol")
                                                    .ToString(),
                                            },
                                            Metadata = meta.JsonMetadata
                                                .GetProperty(responseIdentityToken.Unit[..56])
                                                .GetProperty(assetName)
                                        };

                                        IsIdentityTokenFound = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Log(LogLevel.Information, ex, "IdentityToken definition not found in metadata");
                                    }
                                }
                                if (IsIdentityTokenFound) break;
                            }
                            if (IsIdentityTokenFound) break;
                        }
                        if (assetHistory.Count() < 100 || IsIdentityTokenFound) break;
                    }
                    if (IsIdentityTokenFound) break;
                }
            }
            if (addressAssets is not null && addressAssets.Count() < 100) break;
        }

        if (identityToken is null) return BadRequest("No IdentityToken found.");


        var accountAddressResponse = await client.GetFromJsonAsync<CardanoAccountAddressResponse>($"accounts/{profile.StakeAddress}");
        if (accountAddressResponse is null) return BadRequest("No Account Found.");

        CardanoPoolMetadataResponse? pool = null;
        if (accountAddressResponse.PoolId is not null)
        {
            pool = await client.GetFromJsonAsync<CardanoPoolMetadataResponse>($"pools/{accountAddressResponse.PoolId}/metadata");
        }

        return Ok(new IdentityProfile
        {
            IdentityToken = identityToken,
            StakeAddress = profile.StakeAddress,
            PaymentAddress = profile.PaymentAddress,
            Balance = accountAddressResponse.ControlledAmount,
            Pool = pool
        });
    }

    [HttpGet("{username}/assets")]
    public async Task<IActionResult> GetProfileAssetsAsync(string username, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        if (limit > 100 || page < 1) return BadRequest("Limit must be between 1-100 and page must be greater than 0.");

        if (_identityDbContext.Profiles is null) return StatusCode(500);
        var profile = await _identityDbContext.Profiles.FirstOrDefaultAsync(p => p.Username == username);

        if (profile is null) return NotFound();

        // Create HttpClient
        using var client = _httpClientFactory.CreateClient("blockfrost_mainnet");

        var addressAssets = await client
               .GetFromJsonAsync<IEnumerable<CardanoAddressAssetResponse>>($"accounts/{profile.StakeAddress}/addresses/assets?order=desc&count={limit}&page={page}");

        if (addressAssets is null) return BadRequest();

        var txMetadataCache = new Dictionary<string, IEnumerable<CardanoTxMetadataResponse>?>();
        var identityProfileAssets = new List<CardanoAssetResponse>();
        foreach (var asset in addressAssets)
        {
            if (asset is null) continue;

            var assetInformation = await client
                .GetFromJsonAsync<CardanoAssetResponse>($"assets/{asset.Unit}");

            if (assetInformation?.MintTxHash is null || assetInformation.MintOrBurnCount == 0) continue;

            assetInformation.Quantity = asset.Quantity;

            identityProfileAssets.Add(assetInformation);
        }

        if (addressAssets is null) return BadRequest();

        return Ok(identityProfileAssets);
    }
}