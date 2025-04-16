using System.Security.Cryptography;
using System.Text;

namespace Api;

public static class CryptographyHelper
{
    public static string Sha256(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashedBytes = SHA256.HashData(bytes);
        return Convert.ToBase64String(hashedBytes);
    }
}
