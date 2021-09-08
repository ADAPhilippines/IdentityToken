using System.Text.Json.Serialization;

namespace IdentityToken.API.Models;

public record CardanoBlockResponse
{
    [JsonPropertyName("slot")]
    public uint Slot { get; set; }
}