namespace IdentityToken.API.Models;

public record CardanoIdentityToken
{
    public string PolicyId { get; set; }
    public string AssetName {get;set;}
    public string Username { get { return AssetName[2..]; } }
    public string Avatar { get; set; }
}