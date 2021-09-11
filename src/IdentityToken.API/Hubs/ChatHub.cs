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
                .Include(u => u.Identity)
                .FirstOrDefaultAsync(x => x.ConnectionId == Context.ConnectionId);

            if (chatUser is not null)
            {
                var chatMessage = new ChatMessage
                {
                    Sender = chatUser.Identity,
                    Message = message
                };

                _identityDbContext.Add(chatMessage);
                await Clients.All.SendAsync("ReceiveMessage", chatMessage);
                await _identityDbContext.SaveChangesAsync();
            }
        }
    }

    public async Task GetChatHistory(ChatHistoryRequest request)
    {
        var fromDate = request.From;
        var limit = request.Limit ?? 5;

        if (_identityDbContext is not null && _identityDbContext.ChatMessages is not null && _identityDbContext.ChatUsers is not null)
        {
            var chatUser = await _identityDbContext.ChatUsers
                .Include(u => u.Identity)
                .FirstOrDefaultAsync(x => x.ConnectionId == Context.ConnectionId);

            if (chatUser is not null)
            {
                var chatMessagesQuery = _identityDbContext.ChatMessages
                    .Include(m => m.Sender).AsQueryable();

                if(fromDate is not null)
                    chatMessagesQuery = chatMessagesQuery.Where(m => m.Sent < fromDate);
                
                var chatMessages = await chatMessagesQuery
                    .OrderByDescending(m => m.Sent)
                    .Take(limit)
                    .ToListAsync();
                
                chatMessages.Reverse();

                await Clients.Caller.SendAsync("ReceiveChatHistory", chatMessages);
            }
        }
    }
}