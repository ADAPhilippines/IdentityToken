using System.Text.Json.Serialization;

namespace IdentityToken.API.Models;

public class CardanoBlockResponse
{
    [JsonPropertyName("slot")]
    public uint Slot { get; set; }
}