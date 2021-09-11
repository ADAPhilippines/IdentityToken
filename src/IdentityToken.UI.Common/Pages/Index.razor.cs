using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace IdentityToken.UI.Common.Pages
{
    partial class Index
    {
        private string ActivePanel { get; set; } = "mint";

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