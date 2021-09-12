using System;
using System.Net.Http;
using IdentityToken.UI.Common;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using IdentityToken.UI.Common.Services;
using IdentityToken.UI.Common.Services.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<IdentityToken.UI.Common.App>("#app");

var httpClient = new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
};
var jsonString = await httpClient.GetStreamAsync($"_content/IdentityToken.UI.Common/config.json");

builder.Configuration.AddJsonStream(jsonString);
builder.Services.AddScoped<BootstrapInteropService>();
builder.Services.AddScoped<CardanoWalletInteropService>();
builder.Services.AddScoped<HelperInteropService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddBlazoredLocalStorage();

var apiHttpClient = new HttpClient
{
    BaseAddress = new Uri(builder.Configuration.GetValue<string>("APIUrl"))
};
builder.Services.AddSingleton(sp => apiHttpClient);

await builder.Build().RunAsync();
