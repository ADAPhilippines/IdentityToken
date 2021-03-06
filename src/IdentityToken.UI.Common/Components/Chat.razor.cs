using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using IdentityToken.Common.Models;
using IdentityToken.UI.Common.Services;
using IdentityToken.UI.Common.Services.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace IdentityToken.UI.Common.Components
{
    public partial class Chat
    {
        
        [Inject] private HelperInteropService? HelperInteropService { get; set; }
        [Inject] private IConfiguration? Config { get; set; }
        [Inject] private ILocalStorageService? LocalStorageService { get; set; }
        [Inject] private AuthService? AuthService { get; set; }
        private HubConnection? HubConnection { get; set; }
        private List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        private List<ChatUser> Users { get; set; } = new List<ChatUser>();
        private string CurrentMessage { get; set; } = string.Empty;
        private AuthenticatedIdentity? CurrentUser { get; set; }
        private bool IsLoadingHistoryScroll { get; set; } = false;
        private ChatMessage? CurrentFirstHistoryMessage { get; set; }
        private bool IsLoading { get; set; } = true;
        private bool IsEmojiOpen { get; set; } = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                HubConnection = new HubConnectionBuilder()
                    .WithUrl($"{Config.GetValue<string>("APIUrl")}/chat")
                    .Build();

                HubConnection.On<ChatMessage>("ReceiveMessage", OnReceiveMessage);
                HubConnection.On<AuthenticatedIdentity>("Authenticated", OnAuthenticated);
                HubConnection.On<IEnumerable<ChatMessage>>("ReceiveChatHistory", OnReceiveChatHistory);
                HubConnection.On<IEnumerable<ChatUser>>("ReceiveOnlineUsers", OnReceiveChatUsers);
                if (LocalStorageService is null) return;
                var identity = await LocalStorageService.GetItemAsync<AuthenticatedIdentity>("identity");
                
                await HubConnection.StartAsync();
                await HubConnection.SendAsync("Authenticate", identity.Key);

                if (HelperInteropService is null) return;
                await HelperInteropService.AttachEmojiHandler(DotNetObjectReference.Create(this), "OnEmojiClicked");

            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable]
        public void OnEmojiClicked(string emoji)
        {
            CurrentMessage += emoji;
            StateHasChanged();
        }

        private void OnReceiveChatUsers(IEnumerable<ChatUser> chatUsers)
        {
            Users = chatUsers.ToList();
            StateHasChanged();
        }

        private async void OnReceiveMessage(ChatMessage message)
        {
            Messages.Add(message);
            await InvokeAsync(StateHasChanged);
            
            if (HelperInteropService is null) return;
            await HelperInteropService.ScrollToElementBottomAsync("#message-container");
        }

        private async void OnAuthenticated(AuthenticatedIdentity authenticatedIdentity)
        {
            // Update Current User
            CurrentUser = authenticatedIdentity;
            await InvokeAsync(StateHasChanged);
            
            // Get Chat History
            if(HubConnection is not null)
                await HubConnection.SendAsync("GetChatHistory", new ChatHistoryRequest());
        }

        private async void OnReceiveChatHistory(IEnumerable<ChatMessage> messages)
        {
            Messages.InsertRange(0, messages);
            IsLoading = false;
            await InvokeAsync(StateHasChanged);
            
            if (HelperInteropService is null) return;
            
            if(IsLoadingHistoryScroll is not true)
                await HelperInteropService.ScrollToElementBottomAsync("#message-container");
            else if (IsLoadingHistoryScroll is true && CurrentFirstHistoryMessage is not null)
            {
                await HelperInteropService.ScrollToMessageId(CurrentFirstHistoryMessage.Id);
                IsLoadingHistoryScroll = false;
                await InvokeAsync(StateHasChanged);
            }
            
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

        private void OnBtnLogoutClicked()
        {
            AuthService?.Logout();
        }

        private void OnBtnEmojiClicked()
        {
            IsEmojiOpen = !IsEmojiOpen;
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

        private async void OnMessageHistoryScroll(EventArgs e)
        {
            if (HelperInteropService is null) return;
            var scrollTop = await HelperInteropService.GetElementScrollTopAsync("#message-container");
            if (scrollTop > 0) return;

            if (HubConnection is null) return;
            IsLoadingHistoryScroll = true;
            await InvokeAsync(StateHasChanged);
            CurrentFirstHistoryMessage = Messages.FirstOrDefault();
            await HubConnection.SendAsync("GetChatHistory", new ChatHistoryRequest
            {
                From = CurrentFirstHistoryMessage?.Sent
            });
        }

        private bool IsUserOnline(AuthenticatedIdentity? user)
        {
            if (user is null) return false;
            return Users.Any(u =>
                u.Identity is not null &&
                u.Identity.PolicyId == user.PolicyId &&
                u.Identity.AssetName == user.AssetName);
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