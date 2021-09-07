using System.Text;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
//For CBOR Utilities
using PeterO.Cbor2;

using IdentityToken.API.Models;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.TransactionBuilding;
using CardanoSharp.Wallet.Extensions;

namespace IdentityToken.API.Helpers;

public class CardanoHelper
{
    public static bool IsIdentityToken(string unit)
    {
        return unit.Length > 56 && unit[56..].StartsWith("4944");
    }

    public static string HexToAscii(string hex)
    {
        var bytes = Enumerable.Range(0, hex.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            .ToArray();

        return Encoding.UTF8.GetString(bytes);
    }

    public static string ByteToHex(byte[] bytes)
    {
        var hex = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }


    public static (string, string) GenerateAuthWalletAddress()
    {
        var keyService = new KeyService();
        var mnemonic = keyService.Generate(24, WordLists.English);
        var (address, _, _) = GetWalletWithMneomnic(mnemonic.Words);
        return (mnemonic.Words, address.ToString());
    }

    public static (Address, PrivateKey, PublicKey) GetWalletWithMneomnic(string mnemonic, string paymentPath = "m/1852'/1815'/0'/0/0", string stakePath = "m/1852'/1815'/0'/2/0")
    {
        var keyService = new KeyService();
        var addressService = new AddressService();

        var mnemonicObj = keyService.Restore(mnemonic, WordLists.English);
        // The masterKey is a PrivateKey made of up of the 
        //  - byte[] Key
        //  - byte[] Chaincode
        var masterKey = mnemonicObj.GetRootKey();

        // This path will give us our Payment Key on index 0
        // The paymentPrv is another Tuple with the Private Key and Chain Code
        var paymentPrv = masterKey.Derive(paymentPath);
        // Get the Public Key from the Payment Private Key
        var paymentPub = paymentPrv.GetPublicKey(false);

        // The stakePrv is another Tuple with the Private Key and Chain Code
        var stakePrv = masterKey.Derive(stakePath);
        // Get the Public Key from the Stake Private Key
        var stakePub = stakePrv.GetPublicKey(false);

        var address = addressService
            .GetAddress(
                paymentPub,
                stakePub,
                NetworkType.Mainnet,
                AddressType.Enterprise
            );

        return (address, paymentPrv, paymentPub);
    }

    public static byte[] BuildTxWithMneomnic(string mnemonic, string inputTxHash, uint inputTxId, string toWalletAddress, uint amount)
    {
        var (_, paymentPrv, paymentPub) = GetWalletWithMneomnic(mnemonic);
        var toAddrObj = new Address(toWalletAddress);

        var bodyBuilder = TransactionBodyBuilder.Create
                .AddInput(inputTxHash.HexToByteArray(), inputTxId)
                .AddOutput(toAddrObj.GetBytes(), amount - 200000)
                .SetTtl(1000)
                .SetFee(200000);

        var witnesses = TransactionWitnessSetBuilder.Create
           .AddVKeyWitness(paymentPub, paymentPrv);

        var transaction = TransactionBuilder.Create
                .SetBody(bodyBuilder)
                .SetWitnesses(witnesses)
                .Build();

        return transaction.Serialize();
    }

    public static long CalculateTotalFromOutputs(IEnumerable<CardanoTxOutput> outputs)
    {
        return outputs.Select(x => x != null && x.Amount != null ? x.Amount.Where(y => y.Unit == "lovelace").Sum(y => y.Quantity) : 0).Sum();
    }
}