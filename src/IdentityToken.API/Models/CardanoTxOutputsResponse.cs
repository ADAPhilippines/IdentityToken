using System.Text.Json.Serialization;

namespace IdentityToken.API.Models;

public class CardanoTxOutputsResponse
{
    [JsonPropertyName("hash")]
    public string? Hash { get; set; }

    [JsonPropertyName("inputs")]
    public IEnumerable<CardanoTxOutput>? Inputs { get; set; }

    [JsonPropertyName("outputs")]
    public IEnumerable<CardanoTxOutput>? Outputs { get; set; }
}