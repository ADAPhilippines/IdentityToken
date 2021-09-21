using System.Text.Json.Serialization;

namespace IdentityToken.Common.Models;

public record CardanoPoolMetadataResponse
{
    [JsonPropertyName("hex")]
    public string? Hex { get; set; }
    
    [JsonPropertyName("ticker")]
    public string? Ticker { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}