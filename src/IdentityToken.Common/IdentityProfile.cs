namespace IdentityToken.Common.Models;

public record IdentityProfile
{
    public CardanoIdentityToken? IdentityToken { get; set; }
    public string StakeAddress { get; set; } = string.Empty;
    public string PaymentAddress { get; set; } = string.Empty;
    public CardanoPoolMetadataResponse? Pool { get; set; }
    public ulong Balance { get; set; }
}