using System.Text.Json.Serialization;

namespace IdentityToken.API.Models;


public record CardanoAssetResponse
{
    [JsonPropertyName("policy_id")]
    public string? PolicyId { get; set; }

    [JsonPropertyName("asset_name")]
    public string? AssetName { get; set; }

    [JsonPropertyName("initial_mint_tx_hash")]
    public string? MintTxHash { get; set; }
}