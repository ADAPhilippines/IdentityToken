using System;

namespace IdentityToken.Common.Models;

public record AuthenticatedIdentity : CardanoIdentityToken
{
    public Guid Id { get; init; }
    public string Key { get; init; } = string.Empty;
    public uint ExpiresIn { get; init; }
    public DateTime DateCreated { get; init; } = DateTime.UtcNow;
}