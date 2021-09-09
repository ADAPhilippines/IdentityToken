using System.Text.Json.Serialization;

namespace IdentityToken.Common.Models;


public record CardanoAddressTxsResponse
{
    [JsonPropertyName("tx_hash")]
    public string? TxHash { get; set; }

    [JsonPropertyName("tx_index")]
    public int? TxIndex { get; set; }

    [JsonPropertyName("block_height")]
    public int? BlockHeight { get; set; }
}