using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common.JSInterop
{
    public class BootstrapInterop
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
        
        public BootstrapInterop(IJSRuntime jsRuntime)
        {
            _moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/IdentityToken.UI.Common/bootstrap.js").AsTask());
        }
        
        public async ValueTask InjectStyleSheetAsync(string path)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("injectStyleSheetAsync", path);
        }
        
        public async ValueTask InjectGoogleFontAsync(string url)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("injectGoogleFontAsync", url);
        }
    }
}