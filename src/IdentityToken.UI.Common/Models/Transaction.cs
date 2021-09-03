using System;
using System.Collections.Generic;

namespace IdentityToken.UI.Common.Models
{
    public record Transaction
    {
        public string Hash { get; init; } = string.Empty;
        public string Block { get; init; } = string.Empty;
        public string Fees { get; init; } = string.Empty;
        public string Deposit { get; init; } = string.Empty;
        public string InvalidHereafter { get; init; } = string.Empty;
        public List<QuantifiedObject> OutputAmount { get; init; }
        public long BlockHeight { get; init; }
        public long Slot { get; init; }
        public long Index { get; init; }
        public long Size { get; init; }
        public long UtxoCount { get; init; }
        public long WithdrawalCount { get; init; }
        public long MirCertCount { get; init; }
        public long DelegationCount { get; init; }
        public long StakeCertCount { get; init; }
        public long PoolUpdateCount { get; init; }
        public long PoolRetireCount { get; init; }
        public long AssetMintOrBurnCount { get; init; }
    }
}