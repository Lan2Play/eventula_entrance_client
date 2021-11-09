using EventulaEntranceClient.Services;

namespace EventulaEntranceClient.Pages
{
    public partial class Index
    {
        string AccessCode { get; set; } = string.Empty;

        bool ErrorHidden = true;

        protected void AddToCode(int code)
        {
            AccessCode += code;
        }

        protected void CheckAccessCodeManagement()
        {
            CheckAccessCodeAndNavigate("management");
        }

        private void CheckAccessCodeAndNavigate(string route)
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