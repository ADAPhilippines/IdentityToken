using System.Text.Json;
using System.Text.Json.Serialization;

namespace IdentityToken.Common.Models;

public record CardanoAssetResponse
{
    [JsonPropertyName("policy_id")]
    public string? PolicyId { get; set; }

    [JsonPropertyName("asset_name")]
    public string? AssetName { get; set; }

    [JsonPropertyName("fingerprint")]
    public string? Fingerprint { get; set; }

    [JsonPropertyName("quantity")]
    public string? Quantity { get; set; }

    [JsonPropertyName("initial_mint_tx_hash")]
    public string? MintTxHash { get; set; }

    [JsonPropertyName("mint_or_burn_count")]
    public int MintOrBurnCount { get; set; }

    [JsonPropertyName("metadata")]
    public JsonElement Metadata { get; set; }

    [JsonPropertyName("onchain_metadata")]
    public JsonElement OnChainMetadata { get; set; }
}