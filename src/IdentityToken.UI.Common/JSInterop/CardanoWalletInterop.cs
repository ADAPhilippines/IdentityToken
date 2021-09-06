using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityToken.UI.Common.Models;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common.JSInterop
{
    public class CardanoWalletInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> _bootstrapModuleTask;
        private readonly DotNetObjectReference<CardanoWalletInterop> _objRef;
        private readonly IJSRuntime _jsRuntime;
        public event EventHandler<CardanoWalletInteropError>? Error;

        public CardanoWalletInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _objRef = DotNetObjectReference.Create(this);
            _bootstrapModuleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/IdentityToken.UI.Common/bootstrap.js").AsTask());
        }

        public async ValueTask InitializeAsync(string blockfrostProjectId)
        {
            var module = await _bootstrapModuleTask.Value;
            await module.InvokeVoidAsync("injectCardanoWalletInteropAsync");

            await _jsRuntime.InvokeVoidAsync("CardanoWalletInterop.InitializeAsync", blockfrostProjectId, _objRef);
        }

        public async ValueTask<Transaction?> MintIdentityTokenAsync(string assetName, string metadata)
        {
            return await _jsRuntime
                .InvokeAsync<Transaction>("CardanoWalletInterop.MintIdentityTokenAsync", assetName, metadata);
        }
        
        public async ValueTask<Transaction?> SendAdaAsync(IEnumerable<TxOutput> outputs)
        {
            return await _jsRuntime
                .InvokeAsync<Transaction>("CardanoWalletInterop.SendAdaAsync", outputs);
        }

        public async ValueTask<bool> IsWalletConnectedAsync()
        {
            return await _jsRuntime.InvokeAsync<bool>("CardanoWalletInterop.IsWalletConnectedAsync");
        }

        public async ValueTask<bool> ConnectWalletAsync()
        {
            return await _jsRuntime.InvokeAsync<bool>("CardanoWalletInterop.ConnectWalletAsync");
        }

        [JSInvokable]
        public void OnError(CardanoWalletInteropError error)
        {
            Error?.Invoke(this, error);
        }
        
        public async ValueTask DisposeAsync()
        {
            if (_bootstrapModuleTask.IsValueCreated)
            {
                var module = await _bootstrapModuleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}