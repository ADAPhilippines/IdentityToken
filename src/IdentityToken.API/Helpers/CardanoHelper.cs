using System.Text;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using IdentityToken.API.Models;

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

    public static (string, string) GenerateAuthWalletAddress()
    {
        var keyService = new KeyService();
        var addressService = new AddressService();

        Mnemonic mnemonic = keyService.Generate(24, WordLists.English);

        // The masterKey is a PrivateKey made of up of the 
        //  - byte[] Key
        //  - byte[] Chaincode
        PrivateKey masterKey = mnemonic.GetRootKey();

        // This path will give us our Payment Key on index 0
        string paymentPath = $"m/1852'/1815'/0'/0/0";
        // The paymentPrv is another Tuple with the Private Key and Chain Code
        PrivateKey paymentPrv = masterKey.Derive(paymentPath);
        // Get the Public Key from the Payment Private Key
        PublicKey paymentPub = paymentPrv.GetPublicKey(false);

        // This path will give us our Stake Key on index 0
        string stakePath = $"m/1852'/1815'/0'/2/0";
        // The stakePrv is another Tuple with the Private Key and Chain Code
        var stakePrv = masterKey.Derive(stakePath);
        // Get the Public Key from the Stake Private Key
        var stakePub = stakePrv.GetPublicKey(false);

        Address baseAddr = addressService
            .GetAddress(
                paymentPub,
                stakePub,
                NetworkType.Mainnet,
                AddressType.Enterprise
            );
        
        return (mnemonic.Words, baseAddr.ToString());
    }

    public static bool IsSystemWalletAddress(string walletAddress)
    {
        return File.Exists(Path.Combine(Path.GetTempPath(), walletAddress));
    }

    public static long CalculateTotalFromOutputs(IEnumerable<CardanoTxOutput> outputs)
    {
        return outputs.Select(x => x != null && x.Amount != null ? x.Amount.Where(y => y.Unit == "lovelace").Sum(y => y.Quantity) : 0).Sum();
    }
}