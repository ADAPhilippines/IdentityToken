using System;

namespace IdentityToken.UI.Common.Models
{
    public record TxOutput
    {
        public string Address { get; init; } = string.Empty;
        public long Amount { get; init; }
    }
}