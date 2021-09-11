using System.Text.Json.Serialization;

namespace IdentityToken.Common.Models;

public class IdentityAvatar
{
    [JsonPropertyName("src")]
    public string? Source { get; set; }

    [JsonPropertyName("protocol")]
    public string? Protocol { get; set; }
}
