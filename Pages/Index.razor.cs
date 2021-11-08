using EventulaEntranceClient.Services;
using System.Threading.Tasks;

namespace EventulaEntranceClient.Pages
{
    public partial class Index
    {
        string accessCode;

        bool ErrorHidden = true;

        protected async void CheckAccessCodeSettings()
        {
            await CheckAccessCodeAndNavigate("settings").ConfigureAwait(false);
        }

        protected async void CheckAccessCodeManagement()
        {
            await CheckAccessCodeAndNavigate("management").ConfigureAwait(false);
        }

        private async Task CheckAccessCodeAndNavigate(string route)
        {
            ErrorHidden = true;

            var hash = ProtectionService.CalculateHash(accessCode);

            if (ProtectionService.CheckPrivateAccessCodeHash(hash))
            {
                NavigationManager.NavigateTo($"{route}?ac={hash}");
            }
            else
            {
                accessCode = string.Empty;
                ErrorHidden = false;
            }
        }

        protected async void CloseAlert()
        {
            ErrorHidden = true;
        }

        private void Enter()
        {
            CheckAccessCodeManagement();
        }
    }
}