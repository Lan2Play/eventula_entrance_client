using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using EventulaEntranceClient.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Components;

namespace EventulaEntranceClient.Pages
{
    public partial class Management
    {
        [Inject]
        BackgroundTrigger BackgroundTrigger { get; set; }

        protected override void OnInitialized()
        {
            BackgroundTrigger.Trigger += Trigger;
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


            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("startVideo", "videoFeed");
            }
        }

        private async void Trigger(object sender, EventArgs eventArgs)
        {
            await CaptureFrame();
        }

        private async Task CaptureFrame()
        {
            await JSRuntime.InvokeAsync<String>("getFrame", "videoFeed", "currentFrame", DotNetObjectReference.Create(this)).ConfigureAwait(false);
        }

        [JSInvokable]
        public void ProcessImage(string imageString)
        {
            byte[] imageData = Convert.FromBase64String(imageString.Split(',')[1]);
            var txt = BarcodeService.BarcodeTextFromImage(imageData);
        }

        void IDisposable.Dispose()
        {
            BackgroundTrigger.Trigger -= Trigger;
        }
    }
}