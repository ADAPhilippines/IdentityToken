using System;
using IdentityToken.UI.Common.Services.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common
{
    public static class IdentityTokenServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityTokenBootstrapInteropService(this IServiceCollection services)
        {
            services.AddScoped<BootstrapInteropService>();
            return services;
        }
        
        public static IServiceCollection AddIdentityTokenCardanoWalletInteropService(this IServiceCollection services, string blockfrostProjectId)
        {
            services.AddScoped<CardanoWalletInterop>(provider => new CardanoWalletInterop(provider.GetService<IJSRuntime>(), blockfrostProjectId));
            return services;
        }
    }
}