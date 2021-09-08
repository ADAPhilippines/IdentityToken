namespace IdentityToken.API.Models;

public record AuthenticatedIdentity : CardanoIdentityToken
{
    public Guid Id { get; init; }
    public string Key { get; init; } = string.Empty;
}