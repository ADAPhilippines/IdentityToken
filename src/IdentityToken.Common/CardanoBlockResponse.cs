using System.Text.Json.Serialization;

namespace IdentityToken.Common.Models;

public record CardanoBlockResponse
{
    [JsonPropertyName("slot")]
    public uint Slot { get; set; }
}