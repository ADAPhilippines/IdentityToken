using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using IdentityToken.UI.Common.Services.JSInterop;

namespace IdentityToken.UI.Common.Pages
{
    partial class Index
    {
        [Inject] private HelperInteropService? HelperInteropService { get; set; }
        private string ActivePanel { get; set; } = "mint";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && HelperInteropService is not null)
            {
                await HelperInteropService.HighlightAllCodeElementsAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private string GetTabTextColorClass(string tabName)
        {
            return tabName == ActivePanel ? "text-idt-indigo" : "text-idt-gray";
        }
        private string GetTabUnderlineColorClass(string tabName)
        {
            return tabName == ActivePanel ? "bg-idt-indigo" : "bg-transparent";
        }

        private void SwitchActivePanel(string panel)
        {
            ActivePanel = panel;
        }

        private void OnMintSuccess(EventArgs args)
        {
            ActivePanel = "demo";
        }
    }
}