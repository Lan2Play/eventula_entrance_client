using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using EventulaEntranceClient.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Components;
using EventulaEntranceClient.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using EventulaEntranceClient.Models;
using Microsoft.Extensions.Primitives;

namespace EventulaEntranceClient.Pages
{
    public partial class Management
    {

        #region Injects

        [Inject]
        ProtectionService ProtectionService { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        IBarcodeService BarcodeService { get; set; }

        [Inject]
        ILogger<Index> Logger { get; set; }

        [Inject]
        BackgroundTrigger BackgroundTrigger { get; set; }

        [Inject]
        EventulaApiService EventulaApiService { get; set; }

        [Inject]
        EventulaTokenService EventulaTokenService { get; set; }

        #endregion

        public List<TicketRequest> TicketRequests { get; set; } = new List<TicketRequest>();

        private string AccessCode { get; set; }


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

                AccessCode = accessCode;

                await InvokeAsync(StateHasChanged);
            }
            else
            {
                NavigationManager.NavigateTo("");
            }

            var token = await EventulaTokenService.RetrieveTokenAsync().ConfigureAwait(false);

            if(string.IsNullOrEmpty(token))
            {
                NavigationManager.NavigateTo($"settings?ac={accessCode}");
            }
            else
            {
                if (firstRender)
                {
                    await JSRuntime.InvokeVoidAsync("startVideo", "videoFeed");
                }
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

                var ticketRequest = await EventulaApiService.RequestTicket(qrCode).ConfigureAwait(false);
                if (ticketRequest != null)
                {
                    TicketRequests.Add(ticketRequest);
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        #region IDisposable

        void IDisposable.Dispose()
        {
            BackgroundTrigger.Trigger -= Trigger;
        }
        #endregion

    }
}