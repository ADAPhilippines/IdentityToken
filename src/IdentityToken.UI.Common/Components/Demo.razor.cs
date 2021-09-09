using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using IdentityToken.Common.Models;


namespace IdentityToken.UI.Common.Components
{
    public partial class Demo
    {
        private HubConnection? HubConnection { get; set; }
        private List<string> Messages { get; set; } = new List<string>();
        private string CurrentMessage { get; set; } = string.Empty;
        private AuthenticatedIdentity? CurrentUser { get; set; }

        protected override async Task OnInitializedAsync()
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:6000/chat")
                .Build();

            HubConnection.On<AuthenticatedIdentity, string>("ReceiveMessage", OnReceiveMessage);
            HubConnection.On<AuthenticatedIdentity>("Authenticated", OnAuthenticated);
            
            await HubConnection.StartAsync();
            await HubConnection.SendAsync("Authenticate",
                "2845ea4b4b0f388a60b92d25f14fab95c5677ee09837de9a14650f1872942075");
        }

        private void OnReceiveMessage(AuthenticatedIdentity user, string message)
        {
            var encodedMsg = $"{user.Username}: {message}";
            Messages.Add(encodedMsg);
            StateHasChanged();
        }

        private void OnAuthenticated(AuthenticatedIdentity authenticatedIdentity)
        {
            CurrentUser = authenticatedIdentity;
            StateHasChanged();
        }

        private async void OnBtnSendClicked()
        {
            if (HubConnection is not null)
            {
                await HubConnection.SendAsync("SendMessage", CurrentMessage);
                CurrentMessage = string.Empty;
            }
            StateHasChanged();
        }

        public async ValueTask DisposeAsync()
        {
            if (HubConnection is not null && HubConnection.State == HubConnectionState.Connected)
            {
                await HubConnection.DisposeAsync();
            }
        }
    }
}