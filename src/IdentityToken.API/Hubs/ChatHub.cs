using IdentityToken.API.Data;
using IdentityToken.Common.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace IdentityToken.API.Hubs;

public class ChatHub : Hub
{
    private readonly IdentityDbContext _identityDbContext;

    public ChatHub(IdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public async Task Authenticate(string authToken)
    {
        if (_identityDbContext is not null && _identityDbContext.AuthenticatedIdentities is not null)
        {
            var authenticatedIdentity = await _identityDbContext.AuthenticatedIdentities.FirstOrDefaultAsync(x => x.Key == authToken);

            if (authenticatedIdentity is not null)
            {
                var newChatUser = new ChatUser
                {
                    ConnectionId = Context.ConnectionId,
                    Identity = authenticatedIdentity
                };

                _identityDbContext.Add(newChatUser);
                await Clients.Caller.SendAsync("Authenticated", authenticatedIdentity);
                await _identityDbContext.SaveChangesAsync();
            }
            else
            {
                await Clients.Caller.SendAsync("AuthenticationFailed");
            }
        }
    }

    public async Task SendMessage(string message)
    {
        if (_identityDbContext is not null && _identityDbContext.ChatUsers is not null)
        {
            var chatUser = await _identityDbContext.ChatUsers
                .Include( u => u.Identity)
                .FirstOrDefaultAsync(x => x.ConnectionId == Context.ConnectionId);

            if (chatUser is not null)
            {
                var chatMessage = new ChatMessage
                {
                    Sender = chatUser.Identity,
                    Message = message
                };

                _identityDbContext.Add(chatMessage);
                await Clients.All.SendAsync("ReceiveMessage", chatUser.Identity, message);
                await _identityDbContext.SaveChangesAsync();
            }
        }
    }
}