using EventulaEntranceClient.Services;
using Microsoft.AspNetCore.Components;

namespace EventulaEntranceClient.Pages;

public partial class Index
{
    #region Injects

    [Inject]
    private ILogger<Index> _Logger { get; set; }

    [Inject]
    private NavigationManager _NavigationManager { get; set; }

    [Inject]
    private ProtectionService _ProtectionService { get; set; }

    [Inject]
    private SettingsService _SettingsService{ get; set; }

    #endregion

    private string _AccessCode = string.Empty;
    private string _Logo = string.Empty;

    public string AccessCode
    {
        get
        {
            return _AccessCode;
        }
        set
        {
            _AccessCode = value;
            CheckAccessCodeManagement();
        }
    }

    public bool ErrorHidden { get; set; } = true;


    protected void AddToCode(int code)
    {
        AccessCode += code;
        CheckAccessCodeManagement();
    }

    protected void CheckAccessCodeManagement()
    {
        CheckAccessCodeAndNavigate("management");
    }

    private void CheckAccessCodeAndNavigate(string route)
    {
        if (AccessCode.Length < 4)
        {
            return;
        }

        ErrorHidden = true;

        var hash = _ProtectionService.CalculateHash(AccessCode);

        if (_ProtectionService.CheckUserAccessCodeHash(hash))
        {
            _NavigationManager.NavigateTo($"{route}?ac={hash}");
        }
        else
        {
            AccessCode = string.Empty;
            ErrorHidden = false;
        }
    }

    protected void CloseAlert()
    {
        ErrorHidden = true;
    }


    protected override void OnInitialized()
    {
        if(string.IsNullOrEmpty(_SettingsService.RetrieveCustomBackgroundImage()))
        {
            _Logo = "../images/logo_main.webp";
        }
        else
        {
            _Logo = _SettingsService.RetrieveCustomBackgroundImage();
        }
    }
}