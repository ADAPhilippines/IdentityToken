using IdentityToken.API.Helpers;
using IdentityToken.Common.Models;
using Microsoft.AspNetCore.Mvc;
using IdentityToken.API.Data;
using System.Net.Http.Headers;

namespace IdentityToken.API.Controllers;


[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    private readonly ILogger<IdentityController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IdentityDbContext _identityDbContext;

    public IdentityController(ILogger<IdentityController> logger, IHttpClientFactory httpClientFactory, IdentityDbContext identityDbContext)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _identityDbContext = identityDbContext;
    }

    [HttpGet("auth")]
    public async Task<IActionResult> AuthAsync()
    {
        // NOT THE BEST SECURITY PRACTICE ONLY FOR DEMO
        var (mnemonic, address) = CardanoHelper.GenerateAuthWalletAddress();
        _identityDbContext.IdentityAuthWallets?.Add(new()
        {
            Mnemonic = mnemonic,
            Address = address
        });
        await _identityDbContext.SaveChangesAsync();
        return Ok(address);
    }

    [HttpGet("token/{authCode}")]
    public async Task<IActionResult> GetAsync(string authCode)
    {
        // Check if authCode is system wallet address if not throw error
        var authWallet = _identityDbContext.IdentityAuthWallets?.Where(x => x.IsActive).FirstOrDefault(x => x.Address == authCode);
        if (authWallet is null) return BadRequest("Invalid auth code");

        // Create HttpClient
        using var client = _httpClientFactory.CreateClient("blockfrost");

        // Get System Wallet Address Transactions
        var systemWalletAddressTxsResponse = await client.GetAsync($"addresses/{authCode}/transactions?order=desc");
        if (!systemWalletAddressTxsResponse.IsSuccessStatusCode) return Unauthorized();
        var systemWalletAddressTxs = await systemWalletAddressTxsResponse.Content.ReadFromJsonAsync<IEnumerable<CardanoAddressTxsResponse>>();
        if (systemWalletAddressTxs is null) return BadRequest();

        var latestTx = systemWalletAddressTxs.FirstOrDefault();
        if (latestTx is null) return BadRequest();

        // Get Latest Transaction UTXOs
        var utxos = await client.GetFromJsonAsync<CardanoTxOutputsResponse>($"txs/{latestTx.TxHash}/utxos");
        if (utxos is null || utxos.Outputs is null || utxos.Inputs is null) return Unauthorized();

        var txOutput = utxos.Outputs.Where(x => x.Address == authCode).FirstOrDefault();
        if (txOutput is null) return Unauthorized();

        var txHash = utxos.Hash;
        var txIndex = utxos.Outputs.ToList().IndexOf(txOutput);

        var getTotalLovelace = (uint?)txOutput?.Amount?.Where(y => y.Unit == "lovelace").Sum(y => y.Quantity);

        if (getTotalLovelace is null || getTotalLovelace < 1200000) return Unauthorized();

        var txInput = utxos.Inputs.FirstOrDefault();
        var userWalletAddress = txInput?.Address;
        if (userWalletAddress is null) return BadRequest();

        // Inspect Wallet Address
        var address = await client.GetFromJsonAsync<CardanoAddressResponse>($"addresses/{userWalletAddress}");
        if (address is null) return BadRequest();

        // Inspect Assets
        var addressAssets = await client.GetFromJsonAsync<IEnumerable<CardanoAddressAssetResponse>>($"accounts/{address.StakeAddress}/addresses/assets");
        var tempIdentityTokens = addressAssets?.Where(x => CardanoHelper.IsIdentityToken(x.Unit)).ToList() ?? new List<CardanoAddressAssetResponse>();

        // Initialize Identity Tokens
        var identityTokens = new List<CardanoIdentityToken>();

        // Construct Identity Tokens from Assets
        foreach (var tempIdentityToken in tempIdentityTokens)
        {
            var asset = await client.GetFromJsonAsync<CardanoAssetResponse>($"assets/{tempIdentityToken.Unit}");

            if (asset is null || asset.AssetName is null || asset.PolicyId is null) continue;

            var assetName = CardanoHelper.HexToAscii(asset.AssetName);
            var identityToken = new CardanoIdentityToken
            {
                PolicyId = asset.PolicyId,
                AssetName = assetName
            };

            // Query Asset Metadata
            var metadata = await client.GetFromJsonAsync<IEnumerable<CardanoTxMetadataResponse>>($"txs/{asset.MintTxHash}/metadata");

            // Check if metadata contains IdentityToken definition
            if (metadata is null) continue;

            foreach (var meta in metadata)
            {
                if (meta.Label == "7368")
                {
                    try
                    {
                        identityToken.Avatar = new IdentityAvatar
                        {
                            Source = meta.JsonMetadata
                                .GetProperty(asset.PolicyId)
                                .GetProperty(assetName)
                                .GetProperty("avatar")
                                .GetProperty("src")
                                .GetString(),
                            Protocol = meta.JsonMetadata
                                .GetProperty(asset.PolicyId)
                                .GetProperty(assetName)
                                .GetProperty("avatar")
                                .GetProperty("protocol")
                                .GetString()
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Information, ex, "IdentityToken definition not found in metadata");
                    }
                }
            }


            identityTokens.Add(identityToken);
        }

        // Get Cardano Protocol Params
        var protocolParams = await client.GetFromJsonAsync<CardanoProtocolParamResponse>("epochs/latest/parameters");
        if (protocolParams is null
            || authWallet.Mnemonic is null
            || txOutput is null
            || txHash is null) return StatusCode(500);

        // Get Latest Cardano Block
        var block = await client.GetFromJsonAsync<CardanoBlockResponse>("blocks/latest");
        if (block is null) return StatusCode(500);


        // Pick the first IdentityToken
        var firstIDToken = identityTokens.FirstOrDefault();
        if (firstIDToken is null || firstIDToken.PolicyId is null || firstIDToken.AssetName is null) return StatusCode(500);
        var authenticatedIdentity = new AuthenticatedIdentity
        {
            PolicyId = firstIDToken.PolicyId,
            AssetName = firstIDToken.AssetName,
            Avatar = firstIDToken.Avatar,
            Key = CardanoHelper.Sha265($"{firstIDToken.PolicyId}{firstIDToken.AssetName}{CardanoHelper.GenerateRandomString(128)}"),
            ExpiresIn = 60 * 60 * 24 * 365,
        };
        _identityDbContext.AuthenticatedIdentities?.Add(authenticatedIdentity);

        // Return Change ADA to Address
        var txBytes = CardanoHelper.BuildTxWithMneomnic(authWallet.Mnemonic, txHash, (uint)txIndex, userWalletAddress, (uint)getTotalLovelace, block.Slot + 1000, protocolParams);
        var byteContent = new ByteArrayContent(txBytes);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/cbor");
        var txResponse = await client.PostAsync("tx/submit", byteContent);
        var txId = await txResponse.Content.ReadAsStringAsync();

        // Make sure no one can use the same auth code again
        authWallet.IsActive = false;
        _identityDbContext.Update(authWallet);

        // Commit database updates
        await _identityDbContext.SaveChangesAsync();

        return Ok(authenticatedIdentity);
    }
}