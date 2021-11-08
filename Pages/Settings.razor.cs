using System.Threading.Tasks;
using EventulaEntranceClient.Services;
using Microsoft.AspNetCore.WebUtilities;

namespace EventulaEntranceClient.Pages
{
    public partial class Settings
    {
        public string EventulaToken;

        protected async Task SaveToken()
        {
            if (await EventulaTokenService.SaveTokenAsync(EventulaToken).ConfigureAwait(false))
            {
                NavigationManager.NavigateTo("");
            }
            else
            {
                NavigationManager.NavigateTo("Error");
            }
        }

        protected async void Submit()
        {
            await SaveToken().ConfigureAwait(false);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("ac", out var accessCode))
            {
                if (!ProtectionService.CheckPrivateAccessCodeHash(accessCode))
                {
                    NavigationManager.NavigateTo("");
                }
            }
            else
            {
                NavigationManager.NavigateTo("");
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            EventulaToken = await EventulaTokenService.RetrieveTokenAsync().ConfigureAwait(false);
        }

        protected void Cancel()
        {
            NavigationManager.NavigateTo("");
        }
    }
}