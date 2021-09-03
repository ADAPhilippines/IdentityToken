using System;

namespace IdentityToken.UI.Common.Models
{
    public record CardanoWalletInteropError
    {
        public CardanoWalletInteropErrorType Type { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}