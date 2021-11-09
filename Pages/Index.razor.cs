using EventulaEntranceClient.Services;
using System.Threading.Tasks;

namespace EventulaEntranceClient.Pages
{
    public partial class Index
    {
        string AccessCode { get; set; } = string.Empty;

        bool ErrorHidden = true;

        protected async void CheckAccessCodeSettings()
        {
            await CheckAccessCodeAndNavigate("settings").ConfigureAwait(false);
        }

        protected void AddToCode(int code)
        {
            AccessCode += code;
        }

        protected async void CheckAccessCodeManagement()
        {
            await CheckAccessCodeAndNavigate("management").ConfigureAwait(false);
        }

        private async Task CheckAccessCodeAndNavigate(string route)
        {
            if(AccessCode.Length < 8)
			{
                return;
			}

            ErrorHidden = true;

            var hash = ProtectionService.CalculateHash(AccessCode);

            if (ProtectionService.CheckPrivateAccessCodeHash(hash))
            {
                NavigationManager.NavigateTo($"{route}?ac={hash}");
            }
            else
            {
                AccessCode = string.Empty;
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