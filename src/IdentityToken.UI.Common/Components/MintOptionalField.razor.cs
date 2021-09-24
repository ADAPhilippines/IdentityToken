using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace IdentityToken.UI.Common.Components
{
    partial class MintOptionalField
    {
        [Parameter] public string Class { get; set; } = string.Empty;
        [Parameter] public string KeyPlaceholder { get; set; } = string.Empty;
        [Parameter] public string ValuePlaceholder { get; set; } = string.Empty;
        [Parameter] public bool IsDefault { get; set; }
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]public EventCallback<string> ValueChanged { get; set; }
        private string _value = string.Empty;
        [Parameter] public string Value
        {
            get => _value;
            set
            {
                if (_value == value ) return;
                _value = value;
                ValueChanged.InvokeAsync(value);
            }
        }
        
        [Parameter]public EventCallback<string> KeyChanged { get; set; }
        private string _key = string.Empty;
        [Parameter] public string Key
        {
            get => _key;
            set
            {
                if (_key == value ) return;
                _key = value;
                KeyChanged.InvokeAsync(value);
            }
        }
        [Parameter]public EventCallback<MouseEventArgs> OnDeleteBtnClickCallback { get; set; }
        
        private bool IsFocused { get; set; }

        private void OnFocusIn()
        {
            IsFocused = true;
        }
        
        private void OnFocusOut()
        {
            IsFocused = false;
        }

        private string GetBorderColorClass()
        {
            return IsFocused ? "border-idt-purple" : "border-idt-gray-light";
        }
        
        private string GetIconBgColorClass()
        {
            return IsFocused ? "bg-gradient-to-r from-[#7A60C9] to-[#4C4CE8]" : "bg-idt-gray-light";
        }

        private string GetRightPaddingClass()
        {
            return IsDefault ? "pr-5" : String.Empty;
        }
    }
}