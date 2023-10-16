namespace EventulaEntranceClient.Services;

public class SettingsService
{
    private const int _SettingsIdentifier = 1337;
    private readonly IDataStore _DataStore;
    private readonly ILogger<SettingsService> _Logger;

    public SettingsService(IDataStore dataStore, ILogger<SettingsService> logger)
    {
        _DataStore = dataStore;
        _Logger = logger;
    }

    public bool SaveSettings(string token, string eventulaApiBaseAddress, bool enableCovidTest, bool enableTwoGVerification, bool enableTermsChecked, int testTimeInMinutes, string customBackgroundImage, int signInPlaceCount, string adminPin, string userPin)
    {
        try
        {
            _DataStore.AddOrUpdate(new Settings()
            {
                Id = _SettingsIdentifier,
                Token = token,
                EventulaApiBaseAddress = eventulaApiBaseAddress,
                EnableCovidTest = enableCovidTest,
                EnableTwoGVerification = enableTwoGVerification,
                EnableTermsChecked = enableTermsChecked,
                TestTimeInMinutes = testTimeInMinutes,
                CustomBackgroundImage = customBackgroundImage,
                SignInPlaceCount = signInPlaceCount,
                AdminPin = adminPin,
                UserPin = userPin

            }) ;

            return true;
        }
        catch(Exception ex)
        {
            _Logger.LogError(ex, "Error saving settings.");
            throw;
        }
    }

    public string RetrieveToken()
    {
        return _DataStore.LoadById<Settings>(_SettingsIdentifier)?.Token;
    }        
    public string RetrieveEventulaApiBaseAddress()
    {
        return _DataStore.LoadById<Settings>(_SettingsIdentifier)?.EventulaApiBaseAddress;
    }    
    public bool RetrieveEnableCovidTest()
    {
        return _DataStore.LoadById<Settings>(_SettingsIdentifier)?.EnableCovidTest ?? true;
    }
    public bool RetrieveEnableTwoGVerification()
    {
        return _DataStore.LoadById<Settings>(_SettingsIdentifier)?.EnableTwoGVerification ?? true;
    }
    public bool RetrieveEnableTermsChecked()
    {
        return _DataStore.LoadById<Settings>(_SettingsIdentifier)?.EnableTermsChecked ?? true;
    }
    public int RetrieveTestTimeInMinutes()
    {
        return _DataStore.LoadById<Settings>(_SettingsIdentifier)?.TestTimeInMinutes ?? 15;
    }    
    public int RetrieveSignInPlaceCount()
    {
        return _DataStore.LoadById<Settings>(_SettingsIdentifier)?.SignInPlaceCount ?? 12;
    }
    public string RetrieveCustomBackgroundImage()
    {
        return _DataStore.LoadById<Settings>(_SettingsIdentifier)?.CustomBackgroundImage;
    }    
    public string RetrieveAdminPin()
    {
        return _DataStore.LoadById<Settings>(_SettingsIdentifier)?.AdminPin;
    }     
    public string RetrieveUserPin()
    {
        return _DataStore.LoadById<Settings>(_SettingsIdentifier)?.UserPin;
    }    
}
