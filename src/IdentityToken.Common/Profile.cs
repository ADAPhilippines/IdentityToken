using System;

namespace IdentityToken.Common.Models;

public record Profile
{
    public Guid Id { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public string StakeAddress { get; set; } = string.Empty;
    public string PaymentAddress { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}