using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityToken.UI.Common.Models;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common.Services.JSInterop
{
    public class CardanoWalletInterop : IAsyncDisposable
    {
        private readonly string _blockfrostProjectId;
        private readonly Lazy<Task<IJSObjectReference>>? _bootstrapModuleTask;
        private readonly IJSRuntime? _jsRuntime;
        private readonly DotNetObjectReference<CardanoWalletInterop> _objRef;

        public CardanoWalletInterop(IJSRuntime? jsRuntime, string blockfrostProjectId)
        {
            _jsRuntime = jsRuntime;
            _objRef = DotNetObjectReference.Create(this);
            _blockfrostProjectId = blockfrostProjectId;

            if (jsRuntime == null) return;
            _bootstrapModuleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/IdentityToken.UI.Common/bootstrap.js").AsTask());
        }

        public async ValueTask DisposeAsync()
        {
            if (_bootstrapModuleTask is {IsValueCreated: true})
            {
                var module = await _bootstrapModuleTask.Value;
                await module.DisposeAsync();
            }
        }

        public event EventHandler<CardanoWalletInteropError>? Error;

        public async ValueTask InjectScriptsAsync()
        {
            if (_bootstrapModuleTask != null && _jsRuntime != null)
            {
                var module = await _bootstrapModuleTask.Value;
                await module.InvokeVoidAsync("injectCardanoWalletInteropAsync", _blockfrostProjectId, _objRef);
            }
        }

        public async ValueTask<Transaction?> MintIdentityTokenAsync(string assetName, string metadata)
        {
            if (_jsRuntime == null) return null;
            return await _jsRuntime
                .InvokeAsync<Transaction>("CardanoWalletInterop.MintIdentityTokenAsync", assetName, metadata);
        }

        public async ValueTask<Transaction?> SendAdaAsync(IEnumerable<TxOutput> outputs)
        {
            if (_jsRuntime == null) return null;
            return await _jsRuntime
                .InvokeAsync<Transaction>("CardanoWalletInterop.SendAdaAsync", outputs);
        }

        public async ValueTask<bool> IsWalletConnectedAsync()
        {
            if (_jsRuntime == null) return false;
            return await _jsRuntime.InvokeAsync<bool>("CardanoWalletInterop.IsWalletConnectedAsync");
        }

        public async ValueTask<bool> ConnectWalletAsync()
        {
            if (_jsRuntime == null) return false;
            return await _jsRuntime.InvokeAsync<bool>("CardanoWalletInterop.ConnectWalletAsync");
        }

        [JSInvokable]
        public void OnError(CardanoWalletInteropError error)
        {
            Error?.Invoke(this, error);
        }
    }
}