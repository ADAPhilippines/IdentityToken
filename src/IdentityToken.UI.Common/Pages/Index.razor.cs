using System;
using Microsoft.AspNetCore.Components;

namespace IdentityToken.UI.Common.Pages
{
    partial class Index
    {
        [Parameter] public string Page { get; set; } = "mint";

        protected override void OnInitialized()
        {
            Page ??= "mint";
            base.OnInitialized();
        }

        private string GetTabTextColorClass(string tabName)
        {
            return tabName == Page ? "text-idt-indigo" : "text-idt-gray";
        }
        private string GetTabUnderlineColorClass(string tabName)
        {
            return tabName == Page ? "bg-idt-indigo" : "bg-transparent";
        }
    }
}