using Microsoft.AspNetCore.Components;

namespace IdentityToken.UI.Common.Components
{
    partial class MintRequiredField
    {
        [Parameter] public string Class { get; set; } = string.Empty;
        [Parameter] public string Placeholder { get; set; } = string.Empty;
        [Parameter] public string Label { get; set; } = string.Empty;
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

        private bool IsFocused { get; set; }

        private void OnFocusIn()
        {
            IsFocused = true;
        }
        
        private void OnFocusOut()
        {
            IsFocused = false;
        }
    }
}