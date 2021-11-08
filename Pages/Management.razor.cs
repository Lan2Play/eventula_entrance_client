using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using EventulaEntranceClient.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Components;
using EventulaEntranceClient.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Linq;
using System.Net.Http.Headers;
using EventulaEntranceClient.Models;
using System.Text.Json;

namespace EventulaEntranceClient.Pages
{
    public partial class Management
    {
        #region consts

        private const string _CsrfCookieUrl = "sanctum/csrf-cookie";
        private const string _UserApiParticipantUrl = "api/admin/event/participants/{0}";

        #endregion
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
        IHttpClientFactory HttpClientFactory { get; set; }

        [Inject]
        CookieContainer Cookies { get; set; }


        [Inject]
        EventulaTokenService TokenService { get; set; }
        #endregion

        public List<TicketRequest> TicketRequests { get; set; } = new List<TicketRequest>();

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
                using var httpClient = HttpClientFactory.CreateClient(nameof(Management));

                await SetXcsrfHeader(httpClient);
                SetDefaultHeaders(httpClient, await TokenService.RetrieveTokenAsync());

                var getResult = await httpClient.GetAsync(string.Format(_UserApiParticipantUrl, qrCode.Split('/').Last()));
                var content = await getResult.Content.ReadAsStringAsync();

               var ticketRequest = JsonSerializer.Deserialize<TicketRequest>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if(ticketRequest != null)
                {
                    TicketRequests.Add(ticketRequest);
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        private async Task SetXcsrfHeader(HttpClient httpClient)
        {
            var csrfCookieUrl = new Uri(httpClient.BaseAddress, _CsrfCookieUrl);


            var csrfCookies = Cookies.GetCookies(csrfCookieUrl);

            var csrfToken = csrfCookies.FirstOrDefault(x => x.Name.Equals("XSRF-TOKEN", StringComparison.Ordinal));

            if (csrfToken == null)
            {
                var getCsrfResult = await httpClient.GetAsync(_CsrfCookieUrl);
                csrfCookies = Cookies.GetCookies(csrfCookieUrl);
                csrfToken = csrfCookies.FirstOrDefault(x => x.Name.Equals("XSRF-TOKEN", StringComparison.Ordinal));
            }

            if (csrfToken != null)
            {
                httpClient.DefaultRequestHeaders.Add("X-CSRF-Token", csrfToken.Value);
            }
        }

        private static void SetDefaultHeaders(HttpClient httpClient, string token)
        {
            var requestToken = token.Split('|').Last();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #region IDisposable

        void IDisposable.Dispose()
        {
            BackgroundTrigger.Trigger -= Trigger;
        }
        #endregion

    }
}