namespace IdentityToken.Common.Models;

public record CreateProfileRequestBody
{
    public string PaymentAddress { get; set; } = string.Empty;
}