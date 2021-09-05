using System.Text.Json.Serialization;

namespace IdentityToken.API.Models;

public class CardanoAsset
{
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("quantity")]
    public long Quantity { get; set; }
}