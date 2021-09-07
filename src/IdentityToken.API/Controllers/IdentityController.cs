using IdentityToken.API.Helpers;
using IdentityToken.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.IO;
using IdentityToken.API.Data;
using CardanoSharp.Wallet.Extensions;
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
        if (authWallet == null) return BadRequest("Invalid auth code");

        // Create HttpClient
        using var client = _httpClientFactory.CreateClient("blockfrost");

        // Get System Wallet Address Transactions
        var systemWalletAddressTxsResponse = await client.GetAsync($"addresses/{authCode}/transactions?order=desc");
        if (!systemWalletAddressTxsResponse.IsSuccessStatusCode) return Unauthorized();
        var systemWalletAddressTxs = await systemWalletAddressTxsResponse.Content.ReadFromJsonAsync<IEnumerable<CardanoAddressTxsResponse>>();
        if (systemWalletAddressTxs == null) return BadRequest();

        var latestTx = systemWalletAddressTxs.FirstOrDefault();
        if (latestTx == null) return BadRequest();

        // Get Latest Transaction UTXOs
        var utxos = await client.GetFromJsonAsync<CardanoTxOutputsResponse>($"txs/{latestTx.TxHash}/utxos");
        if (utxos == null || utxos.Outputs == null || utxos.Inputs == null) return Unauthorized();

        var txOutput = utxos.Outputs.Where(x => x.Address == authCode).FirstOrDefault();
        if (txOutput == null) return Unauthorized();

        var txHash = utxos.Hash;
        var txIndex = utxos.Outputs.ToList().IndexOf(txOutput);

        var getTotalLovelace = (uint?)txOutput?.Amount?.Where(y => y.Unit == "lovelace").Sum(y => y.Quantity);

        if (getTotalLovelace == null || getTotalLovelace < 1200000) return Unauthorized();

        var txInput = utxos.Inputs.FirstOrDefault();
        var userWalletAddress = txInput?.Address;
        if (userWalletAddress == null) return BadRequest();

        // Inspect Wallet Address
        var address = await client.GetFromJsonAsync<CardanoAddressResponse>($"addresses/{userWalletAddress}");

        // Inspect Assets
        if (address == null) return BadRequest();

        var addressAssets = await client.GetFromJsonAsync<IEnumerable<CardanoAddressAssetResponse>>($"accounts/{address.StakeAddress}/addresses/assets");
        var tempIdentityTokens = addressAssets?.Where(x => CardanoHelper.IsIdentityToken(x.Unit)).ToList() ?? new List<CardanoAddressAssetResponse>();

        // Initialize Identity Tokens
        var identityTokens = new List<CardanoIdentityToken>();

        // Construct Identity Tokens from Assets
        foreach (var tempIdentityToken in tempIdentityTokens)
        {
            var asset = await client.GetFromJsonAsync<CardanoAssetResponse>($"assets/{tempIdentityToken.Unit}");

            if (asset == null || asset.AssetName == null || asset.PolicyId == null) continue;

            var assetName = CardanoHelper.HexToAscii(asset.AssetName);
            var identityToken = new CardanoIdentityToken
            {
                PolicyId = asset.PolicyId,
                AssetName = assetName
            };

            // Query Asset Metadata
            var metadata = await client.GetFromJsonAsync<IEnumerable<CardanoTxMetadataResponse>>($"txs/{asset.MintTxHash}/metadata");

            // Check if metadata contains IdentityToken definition
            if (metadata == null) continue;

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

        // Get Cardano Protocol Params
        var protocolParams = await client.GetFromJsonAsync<CardanoProtocolParamResponse>("epochs/latest/parameters");
        if (protocolParams == null
            || authWallet.Mnemonic == null
            || txOutput == null
            || txHash == null) return StatusCode(500);

        // Get Latest Cardano Block
        var block = await client.GetFromJsonAsync<CardanoBlockResponse>("blocks/latest");
        if (block == null) return StatusCode(500);

        var txBytes = CardanoHelper.BuildTxWithMneomnic(authWallet.Mnemonic, txHash, (uint)txIndex, userWalletAddress, (uint)getTotalLovelace, block.Slot + 1000, protocolParams);
        
        var byteContent = new ByteArrayContent(txBytes);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/cbor");
        var txResponse = await client.PostAsync("tx/submit", byteContent);
        var txId = await txResponse.Content.ReadAsStringAsync();

        authWallet.IsActive = false;
        _identityDbContext.Update(authWallet);
        await _identityDbContext.SaveChangesAsync();

        return Ok(identityTokens);
    }
}