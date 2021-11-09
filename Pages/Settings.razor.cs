using System.Threading.Tasks;
using EventulaEntranceClient.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

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

        protected async Task SaveToken()
        {
            if (await _EventulaTokenService.SaveTokenAsync(EventulaToken).ConfigureAwait(false))
            {
                _NavigationManager.NavigateTo("");
            }
            else
            {
                _NavigationManager.NavigateTo("Error");
            }
        }

        protected async void Submit()
        {
            await SaveToken().ConfigureAwait(false);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
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

        protected override async Task OnParametersSetAsync()
        {
            EventulaToken = await _EventulaTokenService.RetrieveTokenAsync().ConfigureAwait(false);
        }

        protected void Cancel()
        {
            _NavigationManager.NavigateTo("");
        }
    }
}