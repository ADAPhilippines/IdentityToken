using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using IdentityToken.Common.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;


namespace IdentityToken.UI.Common.Components
{
    public partial class Demo
    {
        private HubConnection? HubConnection { get; set; }
        private List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        private string CurrentMessage { get; set; } = string.Empty;
        private AuthenticatedIdentity? CurrentUser { get; set; }
        [Inject] private IConfiguration? Config { get; set; }

        protected override async Task OnInitializedAsync()
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl($"{Config.GetValue<string>("APIUrl")}/chat")
                .Build();

            HubConnection.On<ChatMessage>("ReceiveMessage", OnReceiveMessage);
            HubConnection.On<AuthenticatedIdentity>("Authenticated", OnAuthenticated);
            HubConnection.On<IEnumerable<ChatMessage>>("ReceiveChatHistory", OnReceiveChatHistory);
            
            await HubConnection.StartAsync();
            await HubConnection.SendAsync("Authenticate",
                "2845ea4b4b0f388a60b92d25f14fab95c5677ee09837de9a14650f1872942075");
        }

        private void OnReceiveMessage(ChatMessage message)
        {
            Messages.Add(message);
            StateHasChanged();
        }

        private async void OnAuthenticated(AuthenticatedIdentity authenticatedIdentity)
        {
            // Update Current User
            CurrentUser = authenticatedIdentity;
            await InvokeAsync(StateHasChanged);
            
            // Get Chat History
            if(HubConnection is not null)
                await HubConnection.SendAsync("GetChatHistory", null, null);
        }

        private void OnReceiveChatHistory(IEnumerable<ChatMessage> messages)
        {
            Messages.AddRange(messages);
            StateHasChanged();
        }

        private async void OnBtnSendClicked()
        {
            if (HubConnection is not null && CurrentMessage != string.Empty)
            {
                await HubConnection.SendAsync("SendMessage", CurrentMessage);
                CurrentMessage = string.Empty;
            }
            StateHasChanged();
        }

        private async void OnCurrentMessageKeyup(KeyboardEventArgs e)
        {
            if (HubConnection is not null && e.Key == "Enter" && CurrentMessage != string.Empty)
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