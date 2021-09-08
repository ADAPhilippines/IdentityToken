using System.Text.Json.Serialization;

namespace IdentityToken.API.Models;

public record CardanoAsset
{
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("quantity")]
    public uint Quantity { get; set; }
}