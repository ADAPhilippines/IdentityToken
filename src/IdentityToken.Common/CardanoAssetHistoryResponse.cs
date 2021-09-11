using System.Text.Json.Serialization;

namespace IdentityToken.Common.Models;

public record CardanoAssetHistoryResponse
{
    [JsonPropertyName("tx_hash")]
    public string? TxHash { get; set; }

    [JsonPropertyName("Action")]
    public string? Action { get; set; }
}