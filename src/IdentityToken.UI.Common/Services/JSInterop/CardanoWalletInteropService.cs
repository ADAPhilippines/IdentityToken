using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityToken.UI.Common.Models;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common.Services.JSInterop;

public class CardanoWalletInteropService
{
    private readonly IJSRuntime? _jsRuntime;
    private bool IsErrorHandlerSet { get; set; }

    public CardanoWalletInteropService(IJSRuntime? jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public event EventHandler<CardanoWalletInteropError>? Error;

    public async ValueTask<string?> MintIdentityTokenAsync(string assetName, string avatar, string metadata)
    {
        if (_jsRuntime is null) return null;
        await EnsureErrorHandlerIsSet();
        return await _jsRuntime
            .InvokeAsync<string?>("CardanoWalletInterop.MintIdentityTokenAsync", assetName, avatar, metadata);
    }

    public async ValueTask<string?> SendAdaAsync(IEnumerable<TxOutput> outputs)
    {
        if (_jsRuntime is null) return null;
        await EnsureErrorHandlerIsSet();
        return await _jsRuntime
            .InvokeAsync<string?>("CardanoWalletInterop.SendAdaAsync", outputs);
    }

    public async ValueTask<Transaction?> GetTransactionAsync(string hash)
    {
        if (_jsRuntime is null) return null;
        await EnsureErrorHandlerIsSet();
        return await _jsRuntime
            .InvokeAsync<Transaction?>("CardanoWalletInterop.GetTransactionAsync", hash);
    }

    public async ValueTask<bool> IsWalletConnectedAsync()
    {
        if (_jsRuntime is null) return false;
        await EnsureErrorHandlerIsSet();
        return await _jsRuntime.InvokeAsync<bool>("CardanoWalletInterop.IsWalletConnectedAsync");
    }

    public async ValueTask<bool> ConnectWalletAsync()
    {
        if (_jsRuntime is null) return false;
        await EnsureErrorHandlerIsSet();
        return await _jsRuntime.InvokeAsync<bool>("CardanoWalletInterop.ConnectWalletAsync");
    }

    public async ValueTask SetErrorHandlerAsync()
    {
        if (_jsRuntime is null) return;
        var objRef = DotNetObjectReference.Create(this);
        await _jsRuntime.InvokeVoidAsync("CardanoWalletInterop.SetErrorHandlerCallback", objRef, "OnError");
        IsErrorHandlerSet = true;
    }

    private async ValueTask EnsureErrorHandlerIsSet()
    {
        if (!IsErrorHandlerSet)
            await SetErrorHandlerAsync();
    }

    [JSInvokable]
    public void OnError(CardanoWalletInteropError error)
    {
        Error?.Invoke(this, error);
    }
}