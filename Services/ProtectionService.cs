using System.Security.Cryptography;
using System.Text;

namespace EventulaEntranceClient.Services;

public class ProtectionService
{
    private string _PrivateAccessCodeHash = "ac3fbb3474801233a338e0f27af3477773ad8772d35c87f70d9489837babb35a";

    public ProtectionService(string privateAccessCodeHash)
    {
        if(!string.IsNullOrEmpty(privateAccessCodeHash))
        {
            _PrivateAccessCodeHash = privateAccessCodeHash;
        }
    }

    public bool CheckPrivateAccessCodeHash(string accessCodeHash)
    {
        return _PrivateAccessCodeHash.Equals(accessCodeHash);
    }

    public string CalculateHash(string accessCode)
    {
        if (string.IsNullOrEmpty(accessCode))
        {
            return string.Empty;
        }

        using var mySha256 = SHA256.Create();

        var encodedAccessCode = Encoding.UTF8.GetBytes(accessCode);

        var hashValue = mySha256.ComputeHash(encodedAccessCode);

        // Convert byte array to a string
        var builder = new StringBuilder();
        for (var i = 0; i < hashValue.Length; i++)
        {
            builder.Append(hashValue[i].ToString("x2"));
        }
        return builder.ToString();
    }

}
