using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IdentityToken.Common.Models;

public record CardanoTxOutput
{
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("amount")]
    public IEnumerable<CardanoAsset>? Amount { get; set; }

    [JsonPropertyName("tx_hash")]
    public string? TxHash { get; set; }

    [JsonPropertyName("output_index")]
    public ulong? Index { get; set; }
}