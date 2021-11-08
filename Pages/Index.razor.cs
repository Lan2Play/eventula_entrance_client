using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using EventulaEntranceClient.Services;

namespace EventulaEntranceClient.Pages
{
    public partial class Index
    {
        protected override void OnInitialized()
        {
            BackgroundTrigger.Trigger += Trigger;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("startVideo", "videoFeed");
            }
        }

        private async void Trigger(object sender, EventArgs eventArgs)
        {
            CaptureFrame();
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