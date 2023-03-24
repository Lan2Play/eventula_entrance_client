using System.Security.Cryptography;
using System.Text;

namespace EventulaEntranceClient.Services;

public class ProtectionService
{
    private string _AdminAccessCodeHash;
    private string _UserAccessCodeHash;

    private readonly SettingsService _SettingsService;
    public ProtectionService(SettingsService settingsService)
    {
        _SettingsService = settingsService;

        

    }

    public bool CheckAdminAccessCodeHash(string accessCodeHash)
    {
        UpdatePins();
        return _AdminAccessCodeHash.Equals(accessCodeHash);
    }

    public bool CheckUserAccessCodeHash(string accessCodeHash)
    {
        UpdatePins();
        return _AdminAccessCodeHash.Equals(accessCodeHash) || _UserAccessCodeHash.Equals(accessCodeHash);
    }

    private void UpdatePins()
    {
        if (!string.IsNullOrEmpty(_SettingsService.RetrieveAdminPin()))
        {
            _AdminAccessCodeHash = _SettingsService.RetrieveAdminPin();
        }        
        else
        {
            _AdminAccessCodeHash = "c50281c3dd92d836d2ba7702fad19f778404cddd49059afc7b2e6e537f436ea7";
        }
        
        if (!string.IsNullOrEmpty(_SettingsService.RetrieveUserPin()))
        {
            _UserAccessCodeHash = _SettingsService.RetrieveUserPin();
        }
        else
        {
            _UserAccessCodeHash = "03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4";
        }      
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
