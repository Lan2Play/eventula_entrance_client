using EventulaEntranceClient.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace EventulaEntranceClient.Pages
{
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
        private EventulaTokenService _EventulaTokenService { get; set; }

        #endregion

        public string EventulaToken;

        protected void SaveToken()
        {
            if (_EventulaTokenService.SaveToken(EventulaToken))
            {
                _NavigationManager.NavigateTo("");
            }
            else
            {
                _NavigationManager.NavigateTo("Error");
            }
        }

        protected void Submit()
        {
            SaveToken();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            var uri = _NavigationManager.ToAbsoluteUri(_NavigationManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("ac", out var accessCode))
            {
                if (!_ProtectionService.CheckPrivateAccessCodeHash(accessCode))
                {
                    _NavigationManager.NavigateTo("");
                }
            }
            else
            {
                _NavigationManager.NavigateTo("");
            }
        }

        protected override void OnParametersSet()
        {
            EventulaToken = _EventulaTokenService.RetrieveToken();
        }

        protected void Cancel()
        {
            _NavigationManager.NavigateTo("");
        }
    }
}