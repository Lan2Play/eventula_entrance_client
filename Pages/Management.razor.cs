using Microsoft.JSInterop;
using EventulaEntranceClient.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Components;
using EventulaEntranceClient.Services.Interfaces;
using EventulaEntranceClient.Models;
using EventulaEntranceClient.Storage;

namespace EventulaEntranceClient.Pages
{
    public partial class Management
    {
        #region Injects

        [Inject]
        private ProtectionService _ProtectionService { get; set; }

        [Inject]
        private NavigationManager _NavigationManager { get; set; }

        [Inject]
        private IJSRuntime _JSRuntime { get; set; }

        [Inject]
        private IBarcodeService _BarcodeService { get; set; }

        [Inject]
        private ILogger<Index> _Logger { get; set; }

        [Inject]
        private BackgroundTrigger _BackgroundTrigger { get; set; }

        [Inject]
        private UiNotifyService _UiNotifyService { get; set; }

        [Inject]
        private EventulaApiService _EventulaApiService { get; set; }

        [Inject]
        private EventulaTokenService _EventulaTokenService { get; set; }


        [Inject]
        private IDataStore _DataStore { get; set; }

        #endregion

        public List<Participant> Participants { get; set; } = new List<Participant>();

        private string AccessCode { get; set; }


        protected override void OnInitialized()
        {
            _BackgroundTrigger.Trigger += Trigger;
            _UiNotifyService.NewParticipant += OnNewParticipant;

            Participants.AddRange(_DataStore.Load<Participant>());
        }

        private async void OnNewParticipant(object sender, Participant participant)
        {
            if (participant != null)
            {
                Participants.Add(participant);
                await InvokeAsync(StateHasChanged);
            }
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

                AccessCode = accessCode;

                await InvokeAsync(StateHasChanged);
            }
            else
            {
                _NavigationManager.NavigateTo("");
            }

            var token = await _EventulaTokenService.RetrieveTokenAsync().ConfigureAwait(false);

            if (string.IsNullOrEmpty(token))
            {
                _NavigationManager.NavigateTo($"settings?ac={accessCode}");
            }
            else
            {
                if (firstRender)
                {
                    await _JSRuntime.InvokeVoidAsync("startVideo", "videoFeed");
                }
            }
        }

        private async void Trigger(object sender, EventArgs eventArgs)
        {
            await CaptureFrame();
        }

        private async Task CaptureFrame()
        {
            var data = await _JSRuntime.InvokeAsync<string>("getFrame", "videoFeed", "currentFrame").ConfigureAwait(false);
            await ProcessImage(data);
        }

        public async Task ProcessImage(string imageString)
        {
            byte[] imageData = Convert.FromBase64String(imageString.Split(',')[1]);
            var qrCode = _BarcodeService.BarcodeTextFromImage(imageData);
            _Logger.LogInformation($"QR Code found {qrCode}");
            if (!string.IsNullOrEmpty(qrCode))
            {

                var ticketRequest = await _EventulaApiService.RequestTicket(qrCode).ConfigureAwait(false);
                if (ticketRequest?.Participant != null)
                {
                    _DataStore.AddOrUpdate(ticketRequest.Participant);

                    var oldParticipant = Participants.FirstOrDefault(x => x.Id == ticketRequest.Participant.Id);
                    if (oldParticipant == null)
                    {
                        Participants.Add(ticketRequest.Participant);
                    }
                    else
                    {
                        var oldId = Participants.IndexOf(oldParticipant);
                        Participants.Remove(oldParticipant);
                        Participants.Insert(oldId, ticketRequest.Participant);
                    }

                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        #region IDisposable

        void IDisposable.Dispose()
        {
            _BackgroundTrigger.Trigger -= Trigger;
        }
        #endregion

    }
}