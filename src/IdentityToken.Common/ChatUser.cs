using System;

namespace IdentityToken.Common.Models;

public record ChatUser
{
    public Guid Id { get; set; }
    public string ConnectionId { get; set; } = string.Empty;
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public AuthenticatedIdentity? Identity { get; set; }
    public bool IsOnline { get; set; } = false;
}