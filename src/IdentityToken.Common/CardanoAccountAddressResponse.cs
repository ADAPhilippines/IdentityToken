using System.Text.Json.Serialization;

namespace IdentityToken.Common.Models;

public record CardanoAccountAddressResponse
{
    [JsonPropertyName("controlled_amount")]
    public ulong ControlledAmount { get; set; }

    [JsonPropertyName("pool_id")]
    public string? PoolId { get; set; }
}