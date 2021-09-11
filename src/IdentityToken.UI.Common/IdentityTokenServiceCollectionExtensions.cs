using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityToken.UI.Common.Models;
using IdentityToken.UI.Common.Services;
using IdentityToken.UI.Common.Services.JSInterop;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace IdentityToken.UI.Common
{
    public static class IdentityTokenServiceCollectionExtensions
    {
        
        public static IServiceCollection AddIdentityToken(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile($"{Environment.CurrentDirectory}../../IdentityToken.UI.Common/wwwroot/config.json");
            services.AddScoped<BootstrapInteropService>();
            services.AddScoped<CardanoWalletInterop>();
            services.AddScoped<AuthService>();
            AddApiHttpClient(services, builder.Configuration.GetValue<string>("APIUrl"));
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
            AddApiHttpClient(services, builder.Configuration.GetValue<string>("APIUrl"));
            return services;
        }

        private static void AddApiHttpClient(IServiceCollection services, string apiUrl)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
            };

            services.AddScoped(sp => httpClient);
        }
    }
}