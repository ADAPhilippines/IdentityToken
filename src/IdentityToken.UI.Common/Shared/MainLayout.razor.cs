using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityToken.UI.Common.Services.JSInterop;
using Microsoft.AspNetCore.Components;

namespace IdentityToken.UI.Common.Shared
{
    public partial class MainLayout
    {
        [Inject] private BootstrapInteropService? BootstrapInteropService { get; set; }
        private bool IsLoading { get; set; } = true;
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                if (BootstrapInteropService is not null)
                {
                    var taskList = new List<Task>
                    {
                        Task.Run(() =>
                            BootstrapInteropService.InjectScriptAsync("https://cdn.jsdelivr.net/npm/emoji-picker-element@^1/index.js", true)),
                        Task.Run(() =>
                            BootstrapInteropService.InjectGoogleFontAsync(
                                "https://fonts.googleapis.com/css2?family=Poppins:wght@100;200;300;400;500;600;700;800;900&display=swap")),
                        Task.Run(() =>
                            BootstrapInteropService.InjectStyleSheetAsync("/_content/IdentityToken.UI.Common/dist/app.css")),
                        Task.Run(() =>
                            BootstrapInteropService.InjectStyleSheetAsync("https://cdnjs.cloudflare.com/ajax/libs/prism/1.24.1/themes/prism-tomorrow.min.css")),
                        Task.Run(() =>
                            BootstrapInteropService.InjectPrismJsAsync()),
                        Task.Run(() =>
                            BootstrapInteropService.InjectApplicationScriptAsync())
                    };

                    await Task.WhenAll(taskList);
                    IsLoading = false;
                    await Task.Delay(1000);
                    await InvokeAsync(StateHasChanged);
                }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}