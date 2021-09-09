using System;
using System.Net.Http;
using IdentityToken.UI.Common;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<IdentityToken.UI.Common.App>("#app");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
await builder.Services.AddIdentityTokenAsync(builder);

await builder.Build().RunAsync();
