using System;

namespace IdentityToken.UI.Common.Models
{
    public record QuantifiedObject
    {
        public string Unit { get; init; } = string.Empty;
        public string Amount { get; init; } = string.Empty;
    }
}