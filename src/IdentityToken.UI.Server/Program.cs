using System;
using System.IO;
using System.Net.Http;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using IdentityToken.UI.Common.Services;
using IdentityToken.UI.Common.Services.JSInterop;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Configuration.AddJsonFile(Path.GetFullPath("../IdentityToken.UI.Common/wwwroot/config.json"));
builder.Services.AddScoped<BootstrapInteropService>();
builder.Services.AddScoped<CardanoWalletInteropService>();
builder.Services.AddScoped<HelperInteropService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddBlazoredLocalStorage();
var httpClient = new HttpClient
{
    BaseAddress = new Uri(builder.Configuration.GetValue<string>("APIUrl"))
};
builder.Services.AddSingleton(sp => httpClient);

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
