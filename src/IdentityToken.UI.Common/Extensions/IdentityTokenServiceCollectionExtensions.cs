using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityToken.UI.Common.Services.JSInterop;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace IdentityToken.UI.Common.Extensions;

public static class IdentityTokenServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityToken(this IServiceCollection services, WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile($"{Environment.CurrentDirectory}../../IdentityToken.UI.Common/wwwroot/config.json");
        services.AddScoped<BootstrapInteropService>();
        services.AddScoped<CardanoWalletInteropService>();
        services.AddScoped<HelperInteropService>();
        return services;
    }
    
    public static async Task<IServiceCollection> AddIdentityTokenAsync(this IServiceCollection services,  WebAssemblyHostBuilder builder)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
        };
        var jsonString = await httpClient.GetStreamAsync($"_content/IdentityToken.UI.Common/config.json");

        builder.Configuration.AddJsonStream(jsonString);
        services.AddScoped<BootstrapInteropService>();
        services.AddScoped<CardanoWalletInteropService>();
        services.AddScoped<HelperInteropService>();
        return services;
    }
}