using IdentityToken.API.Data;
using IdentityToken.Common.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace IdentityToken.API.Hubs;

public class ChatHub : Hub
{
    private readonly IdentityDbContextFactory _identityDbContextFactory;

    public ChatHub(IdentityDbContextFactory identityDbContextFactory)
    {
        _identityDbContextFactory = identityDbContextFactory;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var _identityDbContext = _identityDbContextFactory.CreateDbContext();
        if (_identityDbContext is not null && _identityDbContext.ChatUsers is not null)
        {
            var chatUser = await _identityDbContext.ChatUsers
                .Where(c => c.ConnectionId == Context.ConnectionId).FirstOrDefaultAsync();

            if (chatUser is not null)
            {
                chatUser.IsOnline = false;
                await _identityDbContext.SaveChangesAsync();
            }
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task Authenticate(string authToken)
    {
        var _identityDbContext = _identityDbContextFactory.CreateDbContext();
        if (_identityDbContext is not null && _identityDbContext.AuthenticatedIdentities is not null)
        {
            var authenticatedIdentity = await _identityDbContext.AuthenticatedIdentities.FirstOrDefaultAsync(x => x.Key == authToken);

            if (authenticatedIdentity is not null)
            {
                var newChatUser = new ChatUser
                {
                    ConnectionId = Context.ConnectionId,
                    Identity = authenticatedIdentity,
                    IsOnline = true
                };

                _identityDbContext.Add(newChatUser);
                await Clients.Caller.SendAsync("Authenticated", authenticatedIdentity);
                await _identityDbContext.SaveChangesAsync();
                await Clients.All.SendAsync("ReceiveOnlineUsers",  await GetOnlineUsersAsync());
            }
            else
            {
                await Clients.Caller.SendAsync("AuthenticationFailed");
            }
        }
    }

    public async Task SendMessage(string message)
    {
        var _identityDbContext = _identityDbContextFactory.CreateDbContext();
        if (_identityDbContext is not null && _identityDbContext.ChatUsers is not null)
        {
            var chatUser = await _identityDbContext.ChatUsers
                .Include(u => u.Identity)
                .FirstOrDefaultAsync(x => x.ConnectionId == Context.ConnectionId);
            
            if (chatUser is not null)
            {
                var userLastActivity = chatUser.LastActivity;
                chatUser.LastActivity = DateTime.UtcNow;
                chatUser.IsOnline = true;
                var chatMessage = new ChatMessage
                {
                    Sender = chatUser.Identity,
                    Message = message
                };

                _identityDbContext.Add(chatMessage);
                await Clients.All.SendAsync("ReceiveMessage", chatMessage);
                await _identityDbContext.SaveChangesAsync();

                if(userLastActivity < DateTime.UtcNow.AddMinutes(-5))
                {
                    await Clients.All.SendAsync("ReceiveOnlineUsers",  await GetOnlineUsersAsync());
                }
            }
        }
    }

    public async Task GetChatHistory(ChatHistoryRequest request)
    {
        var _identityDbContext = _identityDbContextFactory.CreateDbContext();
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

                if (fromDate is not null)
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

    private async Task<IEnumerable<ChatUser>> GetOnlineUsersAsync()
    {
        var _identityDbContext = _identityDbContextFactory.CreateDbContext();
        if (_identityDbContext is not null && _identityDbContext.ChatUsers is not null)
        {
            var chatUsers = await _identityDbContext.ChatUsers
                .Include(u => u.Identity)
                .Where(u => u.IsOnline)
                .Where(u => u.LastActivity > DateTime.UtcNow.AddMinutes(-5))
                .OrderByDescending(u => u.LastActivity)
                .ToListAsync();

            var uniqueChatUsers = chatUsers
                   .GroupBy(u => u?.Identity?.PolicyId + u?.Identity?.AssetName, u => u)
                   .Select(g => g.First())
                   .ToList();

            return uniqueChatUsers;
        }
        throw new Exception("Could not get online users");
    }
}