using Microsoft.AspNetCore.Mvc;

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
    public async Task<object> Get(string walletAddress)
    {   
        /*
        https://cardano-mainnet.blockfrost.io/api/v0/addresses/
        https://cardano-mainnet.blockfrost.io/api/v0/accounts//addresses/assets
        https://cardano-mainnet.blockfrost.io/api/v0/assets/
        https://cardano-mainnet.blockfrost.io/api/v0/txs//metadata
        */
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("project_id", "DytenUGdsxY8NTrJVCFI49H9oQsHTz2t");
        var response = await client.GetAsync($"https://cardano-mainnet.blockfrost.io/api/v0/addresses/{walletAddress}");
        return response.Content.ReadFromJsonAsync<object>();
    }   
}