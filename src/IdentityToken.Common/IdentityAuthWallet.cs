using System;

namespace IdentityToken.Common.Models;

public record IdentityAuthWallet
{
    public Guid Id { get; set; }
    public string? Address { get; set; }
    public string? Mnemonic { get; set; }
    public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime? DateUpdated { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}