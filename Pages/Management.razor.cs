using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using EventulaEntranceClient.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Components;
using EventulaEntranceClient.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace EventulaEntranceClient.Pages
{
    public partial class Management
    {
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        IBarcodeService BarcodeService { get; set; }

        [Inject]
        ILogger<Index> Logger { get; set; }

        [Inject]
        BackgroundTrigger BackgroundTrigger { get; set; }

        public List<string> Persons { get; set; } = new List<string>();

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
            var data = await JSRuntime.InvokeAsync<string>("getFrame", "videoFeed", "currentFrame").ConfigureAwait(false);
            await ProcessImage(data);
        }

        public async Task ProcessImage(string imageString)
        {
            byte[] imageData = Convert.FromBase64String(imageString.Split(',')[1]);
            var qrCode = BarcodeService.BarcodeTextFromImage(imageData);
            Logger.LogInformation($"QR Code found {qrCode}");
            if (!string.IsNullOrEmpty(qrCode))
            {
                Persons.Add(qrCode);
                await InvokeAsync(StateHasChanged);
            }
        }

        void IDisposable.Dispose()
        {
            BackgroundTrigger.Trigger -= Trigger;
        }
    }
}