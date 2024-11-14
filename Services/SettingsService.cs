namespace EventulaEntranceClient.Services;

public class SettingsService
{
    private const int _SettingsIdentifier = 1337;
    private readonly IDataStore _DataStore;
    private readonly ILogger<SettingsService> _Logger;
    private Settings _CurrentSettings;

    public SettingsService(IDataStore dataStore, ILogger<SettingsService> logger)
    {
        _DataStore = dataStore;
        _Logger = logger;
    }

    public bool SaveSettings(string token, string eventulaApiBaseAddress, bool enableCovidTest, bool enableTwoGVerification, bool enableTermsChecked, int testTimeInMinutes, string customBackgroundImage, int signInPlaceCount, string adminPin, string userPin)
    {
        try
        {
            _CurrentSettings = new Settings()
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
            };
            _DataStore.AddOrUpdate(_CurrentSettings) ;

            return true;
        }
        catch(Exception ex)
        {
            _Logger.LogError(ex, "Error saving settings.");
            throw;
        }
    }

    public Settings LoadSettings()
    {
        if(_CurrentSettings == null)
        {
            _CurrentSettings = _DataStore.LoadById<Settings>(_SettingsIdentifier);
        }

        return _CurrentSettings;
    }

    public string RetrieveToken()
    {
        return LoadSettings()?.Token;
    }        
    public string RetrieveEventulaApiBaseAddress()
    {
        return LoadSettings()?.EventulaApiBaseAddress;
    }    
    public bool RetrieveEnableCovidTest()
    {
        return LoadSettings()?.EnableCovidTest ?? true;
    }
    public bool RetrieveEnableTwoGVerification()
    {
        return LoadSettings()?.EnableTwoGVerification ?? true;
    }
    public bool RetrieveEnableTermsChecked()
    {
        return LoadSettings()?.EnableTermsChecked ?? true;
    }
    public int RetrieveTestTimeInMinutes()
    {
        return LoadSettings()?.TestTimeInMinutes ?? 15;
    }    
    public int RetrieveSignInPlaceCount()
    {
        return LoadSettings()?.SignInPlaceCount ?? 12;
    }
    public string RetrieveCustomBackgroundImage()
    {
        return LoadSettings()?.CustomBackgroundImage;
    }    
    public string RetrieveAdminPin()
    {
        return LoadSettings()?.AdminPin;
    }     
    public string RetrieveUserPin()
    {
        return LoadSettings()?.UserPin;
    }    
}
