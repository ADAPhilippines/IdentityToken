using System.Text.Json.Serialization;

namespace IdentityToken.Common.Models;

public record IdentityProfileAsset : CardanoAssetResponse
{
    public CardanoTxMetadataResponse? Metadata { get; set; }
}