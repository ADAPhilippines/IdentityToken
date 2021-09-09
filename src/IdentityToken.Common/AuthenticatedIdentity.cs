namespace IdentityToken.Common.Models;

public record AuthenticatedIdentity : CardanoIdentityToken
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public uint ExpiresIn { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}