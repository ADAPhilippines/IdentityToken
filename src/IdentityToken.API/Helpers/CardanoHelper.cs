using System.Text;
using CliWrap;
using CliWrap.Buffered;
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

    public static async Task<string> GenerateWalletAddressAsync()
    {
        var mnemonic = (await Cli.Wrap("cardano-address").WithArguments("recovery-phrase generate").ExecuteBufferedAsync()).StandardOutput;

        var cmd = mnemonic
                | Cli.Wrap("cardano-address").WithArguments(string.Join(" ",
                    "key",
                    "from-recovery-phrase",
                    "Shelley"))
                | Cli.Wrap("cardano-address").WithArguments(string.Join(" ",
                    "key",
                    "child",
                    "1852H/1815H/0H/0/0"))
                | Cli.Wrap("cardano-address").WithArguments(string.Join(" ",
                    "key",
                    "public",
                    "--with-chain-code"))
                | Cli.Wrap("cardano-address").WithArguments(string.Join(" ",
                    "address",
                    "payment",
                    "--network-tag", "mainnet"));

        var result = await cmd.ExecuteBufferedAsync();
        var walletAddress = result.StandardOutput;
        await File.WriteAllTextAsync(Path.Combine(Path.GetTempPath(), walletAddress), mnemonic);
        return walletAddress;
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