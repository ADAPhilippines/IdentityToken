using System.Text.Json.Serialization;

namespace IdentityToken.API.Models;


public record CardanoAddressAssetResponse
{
    [JsonPropertyName("unit")]
    public string Unit { get; set; }

    [JsonPropertyName("quantity")]
    public string Quantity { get; set; }
}