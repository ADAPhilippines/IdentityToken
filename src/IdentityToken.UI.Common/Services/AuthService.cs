using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using IdentityToken.Common.Models;

namespace IdentityToken.UI.Common.Services;

public class AuthService
{
    private readonly HttpClient _apiClient;
    private readonly IConfiguration _configuration;
    private readonly ILocalStorageService _localStorageService;
    
    public event EventHandler? Authenticated;
    public event EventHandler? LoggedOut;
    
    public AuthService(IConfiguration configuration, HttpClient apiClient,ILocalStorageService localStorageService)
    {
        _configuration = configuration;
        _apiClient = apiClient;
        _localStorageService = localStorageService;
    }

    public async Task<string> RequestLoginAsync()
    {
        return await _apiClient.GetStringAsync("/identity/auth");
    }

    public async Task<AuthenticatedIdentity?> Authorize(string walletAddress)
    {
       return await _apiClient.GetFromJsonAsync<AuthenticatedIdentity>($"/identity/token/{walletAddress}");
    }

    public void Authorized()
    {
        Authenticated?.Invoke(this, EventArgs.Empty);
    }

    public async Task<IdentityProfile?> GetProfileAsync(string username)
    {
        return await _apiClient.GetFromJsonAsync<IdentityProfile>($"profile/{username}");
    }

    public async Task<IEnumerable<CardanoAssetResponse>?> GetProfileAssetsAsync(string username, int limit = 20, int page = 1)
    {
        return await _apiClient.GetFromJsonAsync<IEnumerable<CardanoAssetResponse>>($"profile/{username}/assets?limit={limit}&page={page}");
    }

    public async Task<Profile?> CreateProfileAsync(string walletAddress)
    {
        var body = new CreateProfileRequestBody
        {
            PaymentAddress = walletAddress
        };

        var result = await _apiClient.PostAsJsonAsync("/profile", body);
        return await result.Content.ReadFromJsonAsync<Profile>();
    }

    public async void Logout()
    {
        await _localStorageService.RemoveItemAsync("identity");
        LoggedOut?.Invoke(this, EventArgs.Empty);
    }
}