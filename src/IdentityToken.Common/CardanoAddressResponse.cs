using System.Text.Json.Serialization;

namespace IdentityToken.Common.Models;


public record CardanoAddressResponse
{
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("stake_address")]
    public string? StakeAddress { get; set; }
}