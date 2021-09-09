using IdentityToken.API.Data;
using IdentityToken.Common.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace IdentityToken.API.Hubs;

public class ChatHub : Hub
{
    private readonly IdentityDbContext _identityDbContext;
    private Dictionary<string, AuthenticatedIdentity> Users { get; set; } = new Dictionary<string, AuthenticatedIdentity>();

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
                Users.Add(Context.ConnectionId, authenticatedIdentity);
                await Clients.Caller.SendAsync("Authenticated", authenticatedIdentity);
            }
            else
            {
                await Clients.Caller.SendAsync("AuthenticationFailed");
            }
        }
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}