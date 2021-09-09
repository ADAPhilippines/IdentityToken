using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityToken.UI.Common.Models;
using IdentityToken.UI.Common.Services.JSInterop;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityToken.UI.Common
{
    public static class IdentityTokenServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityToken(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile($"{Environment.CurrentDirectory}../../IdentityToken.UI.Common/wwwroot/config.json");
            services.AddScoped<BootstrapInteropService>();
            services.AddScoped<CardanoWalletInterop>();
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
            services.AddScoped<CardanoWalletInterop>();
            return services;
        }
    }
}