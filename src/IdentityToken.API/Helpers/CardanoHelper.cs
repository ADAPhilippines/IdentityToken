using System.Text;

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
}