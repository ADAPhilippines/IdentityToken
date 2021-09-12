using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common.Services.JSInterop;

public class BootstrapInteropService
{
    private readonly Lazy<Task<IJSObjectReference>>? _moduleTask;
    private readonly IConfiguration _configuration;
    
    public BootstrapInteropService(IJSRuntime jsRuntime, IConfiguration config)
    {
        _configuration = config;
        _moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/IdentityToken.UI.Common/bootstrap.js").AsTask());
    }

    public async ValueTask InjectScriptAsync(string url, bool isModule = false)
    {
        if (_moduleTask is not null)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("injectScriptAsync", url, isModule);
        }
    }

    public async ValueTask InjectStyleSheetAsync(string url)
    {
        if (_moduleTask is not null)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("injectStyleSheetAsync", url);
        }
    }

    public async ValueTask InjectGoogleFontAsync(string url)
    {
        if (_moduleTask is not null)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("injectGoogleFontAsync", url);
        }
    }
    
    public async ValueTask InjectPrismJsAsync()
    {
        if (_moduleTask is not null)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("injectPrismJSAsync");
        }
    }
    
    public async ValueTask InjectApplicationScriptAsync()
    {
        if (_moduleTask is not null)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("injectApplicationScriptAsync", _configuration["BlockfrostProjectId"]);
        }
    }
}