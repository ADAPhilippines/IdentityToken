namespace IdentityToken.Common.Models;

public record CardanoIdentityToken
{
    public string? PolicyId { get; set; }
    public string? AssetName {get;set;}
    public string Username => AssetName != null ? AssetName[2..] : string.Empty;
    public string? Avatar { get; set; }
}