using System.Text.Json.Serialization;

namespace IdentityToken.Common.Models;

public record CardanoAsset
{
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("quantity")]
    public ulong Quantity { get; set; }
}