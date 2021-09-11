using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common.Services.JSInterop;

public class HelperInteropService
{
    private readonly IJSRuntime? _jsRuntime;

    public HelperInteropService(IJSRuntime? jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task ScrollToElementBottomAsync(string selector)
    {
        if (_jsRuntime is null) return;
        await _jsRuntime.InvokeVoidAsync("window.ScrollToElementBottom", selector);
    }

    public async Task<int> GetElementScrollTopAsync(string selector)
    {
        if (_jsRuntime is null) return -1;
        return await _jsRuntime.InvokeAsync<int>("window.GetElementScrollTop", selector);
    }

    public async Task ScrollToMessageId(Guid id)
    {
        if (_jsRuntime is null) return;
        await _jsRuntime.InvokeVoidAsync("window.ScrollToMessageId", id);
    }
    
    public async Task<string> GenerateQrDataUrlAsync(string data)
    {
        if (_jsRuntime is null) return string.Empty;
        return await _jsRuntime.InvokeAsync<string>("window.GenerateQRDataUrlAsync", data);
    }
    
    public async Task<string> CopyToClipboardAsync(string data)
    {
        if (_jsRuntime is null) return string.Empty;
        return await _jsRuntime.InvokeAsync<string>("window.CopyToClipboardAsync", data);
    }
}