using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Timers;
using Timer = System.Timers.Timer;

namespace IdentityToken.UI.Common.Shared
{
    partial class Toast
    {
        [Parameter] public string Message { get; set; } = string.Empty;
        [Parameter] public bool IsError { get; set; }
        [Parameter]public EventCallback<bool> ShouldShowChanged { get; set; }
        private bool _shouldShow;
        [Parameter] public bool ShouldShow
        {
            get => _shouldShow;
            set
            {
                if (_shouldShow == value ) return;
                _shouldShow = value;
                InvokeAsync(StateHasChanged);
                InvokeAsync(() => ShouldShowChanged.InvokeAsync(value));
                if (value)
                    StartCountdown();
            }
        }
        private string ColorClassPrefix => IsError ? "idt-danger" : "idt-success";
        private string ToastPosition => ShouldShow ? "transform -translate-y-60" : "";
        private Timer? _countdown;

        private void StartCountdown()
        {
            SetCountdown();

            if (_countdown == null) return;
            if (_countdown.Enabled)
            {
                _countdown.Stop();
                _countdown.Start();
            }
            else
            {
                _countdown.Start();
            }
        }
        
        private void SetCountdown()
        {
            if (_countdown != null) return;
            _countdown = new Timer(5000);
            _countdown.Elapsed += HideToast;
            _countdown.AutoReset = false;
        }

        private void HideToast(object? source, ElapsedEventArgs args)
        {
            ShouldShow = false;
        }

        private void OnBtnHideClicked()
        {
            ShouldShow = false;
        }

    }
}