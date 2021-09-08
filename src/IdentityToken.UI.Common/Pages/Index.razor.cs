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

        private string GetTabUnderlineClass(string link)
        {
            return link == Page ? "bg-idt-indigo" : "bg-transparent";
        }
        
        private string GetTabFontClass(string link)
        {
            return link == Page ? "text-idt-indigo" : "text-secondary";
        }
    }
}