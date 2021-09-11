using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common.Services.JSInterop;

public class BootstrapInteropService
{
    private readonly Lazy<Task<IJSObjectReference>>? _moduleTask;

    public BootstrapInteropService(IJSRuntime jsRuntime)
    {
        _moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/IdentityToken.UI.Common/bootstrap.js").AsTask());
    }

    public async ValueTask InjectStyleSheetAsync(string path)
    {
        if (_moduleTask != null)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("injectStyleSheetAsync", path);
        }
    }

    public async ValueTask InjectGoogleFontAsync(string url)
    {
        if (_moduleTask != null)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("injectGoogleFontAsync", url);
        }
    }
}
