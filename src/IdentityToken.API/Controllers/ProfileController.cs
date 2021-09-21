using IdentityToken.API.Data;
using IdentityToken.API.Helpers;
using IdentityToken.Common.Models;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> CreateProfileAsync([FromBody]string paymentAddress)
    {
        // Create HttpClient
        using var client = _httpClientFactory.CreateClient("blockfrost");
        
        // Inspect Wallet Address
        var address = await client.GetFromJsonAsync<CardanoAddressResponse>($"addresses/{paymentAddress}");
        if (address?.StakeAddress is null) return BadRequest();

        // Inspect Assets
        var accountAssetsPage = 1;

        Profile? profile = null;

        while(true) 
        {
            var addressAssets = await client
                .GetFromJsonAsync<IEnumerable<CardanoAddressAssetResponse>>($"accounts/{address.StakeAddress}/addresses/assets?order=desc&page={accountAssetsPage++}");

            if(addressAssets is null) return BadRequest();

            var responseIdentityToken = addressAssets?.Where(x => CardanoHelper.IsIdentityToken(x.Unit)).FirstOrDefault();

            if(responseIdentityToken?.Unit is not null)
            {
                var assetHistory = await client
                    .GetFromJsonAsync<IEnumerable<CardanoAssetHistoryResponse>>($"assets/{responseIdentityToken.Unit}/history?order=desc");
                
                if (assetHistory is null || !assetHistory.Any()) continue;

                foreach(var assetHistoryEntry in assetHistory)
                {
                    if(assetHistoryEntry is null) continue;

                    //IdentityToken Found!
                    var assetName = CardanoHelper.HexToAscii(responseIdentityToken.Unit[60..]);
                    var checkSum = CardanoHelper.GetShortHash($"{address.StakeAddress}{assetName}");
                    var username = $"{assetName}-{checkSum}";
                    profile = new ()
                    {
                        Username = username,
                        PaymentAddress = paymentAddress,
                        StakeAddress = address.StakeAddress
                    };
                }
            }

            if(addressAssets.Count() < 100) break;
        }

        if(profile is null) return BadRequest("No IdentityToken found.");

        return Ok(profile);
    }
}