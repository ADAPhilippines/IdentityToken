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
    }
}