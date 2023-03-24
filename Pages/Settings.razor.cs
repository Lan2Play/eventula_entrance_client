using EventulaEntranceClient.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace EventulaEntranceClient.Pages;

public partial class Settings
{
    #region

    [Inject]
    private ILogger<Settings> _Logger { get; set; }
    [Inject]
    private ProtectionService _ProtectionService { get; set; }
    [Inject]
    private NavigationManager _NavigationManager { get; set; }
    [Inject]
    private SettingsService _SettingsService { get; set; }
    [Inject]
    private IDataStore _DataStore { get; set; }

    #endregion

    public bool ActiveParticipants;
    public string EventulaToken;
    public string EventulaApiBaseAddress;
    public bool EnableCovidTest;
    public bool EnableTwoGVerification;
    public bool EnableTermsChecked;
    public int TestTimeInMinutes;
    public int SignInPlaceCount;
    public string CustomBackgroundImage;
    public string AdminPin;
    public string UserPin;


    protected void Save()
    {
        if (ActiveParticipants)
        {
            throw new Exception("Saving the settings is disabled while active participants are in place");
        }
        if (_SettingsService.SaveSettings(EventulaToken, EventulaApiBaseAddress, EnableCovidTest, EnableTwoGVerification, EnableTermsChecked, TestTimeInMinutes, CustomBackgroundImage, SignInPlaceCount, AdminPin, UserPin))
        {
            _NavigationManager.NavigateTo("");
        }
        else
        {
            throw new Exception("Error while saving settings");
        }
    }

    protected void Submit()
    {
        Save();
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        var uri = _NavigationManager.ToAbsoluteUri(_NavigationManager.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("ac", out var accessCode))
        {
            if (!_ProtectionService.CheckAdminAccessCodeHash(accessCode))
            {
                _NavigationManager.NavigateTo("");
            }
        }
        else
        {
            _NavigationManager.NavigateTo("");
        }
        await base.SetParametersAsync(parameters);
    }

    protected override void OnInitialized()
    {
        var filtered = _DataStore.Load<ParticipantSignInPlace>().Where(e => e.Participant !=  null).ToArray();
        ActiveParticipants = filtered.Count() > 0 ? true : false ;
    }

    protected override void OnParametersSet()
    {
        EventulaToken = _SettingsService.RetrieveToken();
        EventulaApiBaseAddress = _SettingsService.RetrieveEventulaApiBaseAddress();
        EnableCovidTest = _SettingsService.RetrieveEnableCovidTest();
        EnableTwoGVerification = _SettingsService.RetrieveEnableTwoGVerification();
        EnableTermsChecked = _SettingsService.RetrieveEnableTermsChecked();
        TestTimeInMinutes = _SettingsService.RetrieveTestTimeInMinutes();
        CustomBackgroundImage = _SettingsService.RetrieveCustomBackgroundImage();
        SignInPlaceCount = _SettingsService.RetrieveSignInPlaceCount();
        AdminPin = _SettingsService.RetrieveAdminPin();
        UserPin = _SettingsService.RetrieveUserPin();
    }

    protected void Cancel()
    {
        _NavigationManager.NavigateTo("");
    }
}