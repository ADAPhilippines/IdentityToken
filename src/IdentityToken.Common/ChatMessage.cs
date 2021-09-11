using System;

namespace IdentityToken.Common.Models;

public record ChatMessage
{
    public Guid Id { get; set; }
    public AuthenticatedIdentity? Sender { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Sent { get; set; } = DateTime.UtcNow;
}