using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityToken.UI.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common.Services.JSInterop
{
    public class CardanoWalletInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>>? _bootstrapModuleTask;
        private readonly IJSRuntime? _jsRuntime;

        public CardanoWalletInterop(IJSRuntime? jsRuntime, IConfiguration config)
        {
            _jsRuntime = jsRuntime;
            var objRef = DotNetObjectReference.Create(this);
            var blockfrostProjectId = config["BlockfrostProjectId"];

            if (jsRuntime == null) return;
            _bootstrapModuleTask = new Lazy<Task<IJSObjectReference>>(() => Task.Run(async () =>
            {
                var module = await jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/IdentityToken.UI.Common/bootstrap.js");
                await module.InvokeVoidAsync("injectCardanoWalletInteropAsync", blockfrostProjectId, objRef);
                return module;
            }));
        }

        public async ValueTask DisposeAsync()
        {
            if (_bootstrapModuleTask is { IsValueCreated: true })
            {
                var module = await _bootstrapModuleTask.Value;
                await module.DisposeAsync();
            }
        }

        public event EventHandler<CardanoWalletInteropError>? Error;

        private async Task EnsureLoadedAsync()
        {
            if (_bootstrapModuleTask is null) return;
            var module = await _bootstrapModuleTask.Value;
        }
        
        public async ValueTask<string?> MintIdentityTokenAsync(string assetName, string avatar, string metadata)
        {
            await EnsureLoadedAsync();
            if (_jsRuntime is null) return null;
            return await _jsRuntime
                .InvokeAsync<string?>("CardanoWalletInterop.MintIdentityTokenAsync", assetName, avatar, metadata);
        }

        public async ValueTask<string?> SendAdaAsync(IEnumerable<TxOutput> outputs)
        {
            await EnsureLoadedAsync();
            if (_jsRuntime is null) return null;
            return await _jsRuntime
                .InvokeAsync<string?>("CardanoWalletInterop.SendAdaAsync", outputs);
        }

        public async ValueTask<Transaction?> GetTransactionAsync(string hash)
        {
            await EnsureLoadedAsync();
            if (_jsRuntime is null) return null;
            return await _jsRuntime
                .InvokeAsync<Transaction?>("CardanoWalletInterop.GetTransactionAsync", hash);
        }

        public async ValueTask<bool> IsWalletConnectedAsync()
        {
            await EnsureLoadedAsync();
            if (_jsRuntime is null) return false;
            return await _jsRuntime.InvokeAsync<bool>("CardanoWalletInterop.IsWalletConnectedAsync");
        }

        public async ValueTask<bool> ConnectWalletAsync()
        {
            await EnsureLoadedAsync();
            if (_jsRuntime is null) return false;
            return await _jsRuntime.InvokeAsync<bool>("CardanoWalletInterop.ConnectWalletAsync");
        }

        public async Task ScrollToElementBottomAsync(string selector)
        {
            await EnsureLoadedAsync();
            if (_jsRuntime is null) return;
            await _jsRuntime.InvokeVoidAsync("window.ScrollToElementBottom", selector);
        }
        
        public async Task<int> GetElementScrollTopAsync(string selector)
        {
            await EnsureLoadedAsync();
            if (_jsRuntime is null) return -1;
            return await _jsRuntime.InvokeAsync<int>("window.GetElementScrollTop", selector);
        }
        
        public async Task ScrollToMessageId(Guid id)
        {
            await EnsureLoadedAsync();
            if (_jsRuntime is null) return;
            await _jsRuntime.InvokeVoidAsync("window.ScrollToMessageId", id);
        }
        
        [JSInvokable]
        public void OnError(CardanoWalletInteropError error)
        {
            Error?.Invoke(this, error);
        }
    }
}