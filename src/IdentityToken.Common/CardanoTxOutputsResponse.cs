using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IdentityToken.Common.Models;

public record CardanoTxOutputsResponse
{
    [JsonPropertyName("hash")]
    public string? Hash { get; set; }

    [JsonPropertyName("inputs")]
    public IEnumerable<CardanoTxOutput>? Inputs { get; set; }

    [JsonPropertyName("outputs")]
    public IEnumerable<CardanoTxOutput>? Outputs { get; set; }
}