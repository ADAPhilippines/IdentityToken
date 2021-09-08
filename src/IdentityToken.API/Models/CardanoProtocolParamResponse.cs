using System.Text.Json.Serialization;

namespace IdentityToken.API.Models;

public record CardanoProtocolParamResponse
{
    [JsonPropertyName("min_fee_a")]
    public uint? TxFeePerByte { get; set; }

    [JsonPropertyName("min_fee_b")]
    public uint? TxFeeFixed { get; set; }
}