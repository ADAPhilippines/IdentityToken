using System.Text.Json;
using System.Text.Json.Serialization;

namespace IdentityToken.API.Models;

public record CardanoTxMetadataResponse
{

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("json_metadata")]
    public JsonElement JsonMetadata { get; set; }
}