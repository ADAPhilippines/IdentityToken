using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace IdentityToken.UI.Common.Services;

public class AuthService
{
    private readonly HttpClient _apiClient;
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration, HttpClient apiClient)
    {
        _configuration = configuration;
        _apiClient = apiClient;
    }
}