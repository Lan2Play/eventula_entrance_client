using EventulaEntranceClient.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace EventulaEntranceClient.Pages;

public partial class Settings
{
    #region

    [Inject]
    private ProtectionService ProtectionService { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; }

    [Inject]
    private SettingsService SettingsService { get; set; }

    [Inject]
    private IDataStore DataStore { get; set; }

    #endregion

    private bool _ActiveParticipants;
    private string _EventulaToken;
    private string _EventulaApiBaseAddress;
    private bool _EnableCovidTest;
    private bool _EnableTwoGVerification;
    private bool _EnableTermsChecked;
    private int _TestTimeInMinutes;
    private int _SignInPlaceCount;
    private string _CustomBackgroundImage;
    private string _AdminPin;
    private string _UserPin;

    protected void Save()
    {
        if (_ActiveParticipants)
        {
            throw new Exception("Saving the settings is disabled while active participants are in place");
        }

        if (SettingsService.SaveSettings(_EventulaToken, _EventulaApiBaseAddress, _EnableCovidTest, _EnableTwoGVerification, _EnableTermsChecked, _TestTimeInMinutes, _CustomBackgroundImage, _SignInPlaceCount, _AdminPin, _UserPin))
        {
            NavigationManager.NavigateTo("");
        }
        else
        {
            throw new Exception("Error while saving settings");
        }
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("ac", out var accessCode))
        {
            if (!ProtectionService.CheckAdminAccessCodeHash(accessCode))
            {
                NavigationManager.NavigateTo("");
            }
        }
        else
        {
            NavigationManager.NavigateTo(string.Empty);
        }
        await base.SetParametersAsync(parameters);
    }

    protected override void OnInitialized()
    {
        var filtered = DataStore.Load<ParticipantSignInPlace>().Where(e => e.Participant != null).ToArray();
        _ActiveParticipants = filtered.Length > 0;
    }

    protected override void OnParametersSet()
    {
        _EventulaToken = SettingsService.RetrieveToken();
        _EventulaApiBaseAddress = SettingsService.RetrieveEventulaApiBaseAddress();
        _EnableCovidTest = SettingsService.RetrieveEnableCovidTest();
        _EnableTwoGVerification = SettingsService.RetrieveEnableTwoGVerification();
        _EnableTermsChecked = SettingsService.RetrieveEnableTermsChecked();
        _TestTimeInMinutes = SettingsService.RetrieveTestTimeInMinutes();
        _CustomBackgroundImage = SettingsService.RetrieveCustomBackgroundImage();
        _SignInPlaceCount = SettingsService.RetrieveSignInPlaceCount();
        _AdminPin = SettingsService.RetrieveAdminPin();
        _UserPin = SettingsService.RetrieveUserPin();
    }

    protected void Cancel()
    {
        NavigationManager.NavigateTo(string.Empty);
    }
}