using EventulaEntranceClient.Services;
using EventulaEntranceClient.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System.Timers;

namespace EventulaEntranceClient.Pages;

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
    private SettingsService _SettingsService { get; set; }


    [Inject]
    private IDataStore _DataStore { get; set; }

    #endregion

    private int _ParticipantSignInPlacesCount;

    private const string _NoTicketFound = "No Ticket found";

    private string _LastTicket = string.Empty;

    public List<ParticipantSignInPlace> ParticipantSignInPlaces { get; set; }

    public List<Participant> Participants { get; set; } = new List<Participant>();

    private string AccessCode { get; set; }

    private string AnimationClass { get; set; } = string.Empty;

    private bool IsRunningElectron { get; set; }

    public string LastTicketNr { get; set; } = _NoTicketFound;

    private System.Timers.Timer _InactiveTimer;

    protected override void OnInitialized()
    {
        _InactiveTimer = new System.Timers.Timer(TimeSpan.FromSeconds(120).TotalMilliseconds);
        _InactiveTimer.Elapsed += UserInactive;
        _InactiveTimer.AutoReset = false;

        _BackgroundTrigger.SubscribeTrigger(Trigger);
        _UiNotifyService.NewParticipant += OnNewParticipant;
        _ParticipantSignInPlacesCount = _SettingsService.RetrieveSignInPlaceCount();
        ParticipantSignInPlaces = new List<ParticipantSignInPlace>(_ParticipantSignInPlacesCount);
        Participants.AddRange(_DataStore.Load<Participant>());

        var savedParticipants = _DataStore.Load<ParticipantSignInPlace>();

        ParticipantSignInPlaces.AddRange(savedParticipants);

        for (int i = 1; i <= _ParticipantSignInPlacesCount; i++)
        {
            if (!ParticipantSignInPlaces.Any(a => a.Id == i))
            {
                var updatedParticipant = new ParticipantSignInPlace()
                {
                    Id = i,
                };

                _DataStore.AddOrUpdate(updatedParticipant);
                ParticipantSignInPlaces.Add(updatedParticipant);
            }
        }

        foreach (var removeParticipant in ParticipantSignInPlaces.Where(a => a.Id > _ParticipantSignInPlacesCount).ToList())
        {
            ParticipantSignInPlaces.Remove(removeParticipant);
            _DataStore.Delete(removeParticipant);
        }
    }

    private async void OnNewParticipant(object sender, Participant participant)
    {
        await AddParticipantAsync(participant).ConfigureAwait(false);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var uri = _NavigationManager.ToAbsoluteUri(_NavigationManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("ac", out var accessCode))
            {
                if (!_ProtectionService.CheckUserAccessCodeHash(accessCode))
                {
                    _NavigationManager.NavigateTo(string.Empty);
                }

                AccessCode = accessCode;
            }
            else
            {
                _NavigationManager.NavigateTo(string.Empty);
            }

            var token = _SettingsService.RetrieveToken();
            var eventulaApiBaseAddress = _SettingsService.RetrieveEventulaApiBaseAddress();

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(eventulaApiBaseAddress))
            {
                _NavigationManager.NavigateTo($"settings?ac={accessCode}");
            }
            else
            {
                await _JSRuntime.InvokeVoidAsync("startVideo", "videoFeed");
            }

            IsRunningElectron = await _JSRuntime.InvokeAsync<bool>("isElectron").ConfigureAwait(false);
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task Trigger()
    {
        try
        {
            await CaptureFrame();
        }
        catch (Exception e)
        {
            _Logger.LogError(e, "Error capturing frame.");
        }
    }

    private void GoBack()
    {
        _NavigationManager.NavigateTo(string.Empty);
    }

    private void CloseApp()
    {
        ElectronNET.API.Electron.App.Quit();
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
        _Logger.LogInformation(string.IsNullOrEmpty(qrCode) ? "QR Code not found" : $"QR Code found {qrCode}");

        if (string.IsNullOrEmpty(qrCode) || string.Equals(_LastTicket, qrCode, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _LastTicket = qrCode;
        LastTicketNr = string.IsNullOrEmpty(qrCode) ? _NoTicketFound : $"Ticket Nr.: {qrCode.Split('/')[^1]}";
        await InvokeAsync(StateHasChanged);

        var ticketRequest = await _EventulaApiService.RequestTicket(qrCode).ConfigureAwait(false);
        if (ticketRequest?.Participant != null)
        {
            await AddParticipantAsync(ticketRequest.Participant).ConfigureAwait(false);
        }
    }

    public async Task AddToSignInPlace(Participant participant)
    {
        if (participant == null)
        {
            return;
        }

        // Reset last ticket to be able to scan again
        _LastTicket = null;

        if (participant.Revoked != 0)
        {
            Participants.Remove(participant);
            _DataStore.Delete(participant);
            return;
        }

        var openPlace = FindEmptySignInPlace();

        if (openPlace == null)
        {
            return;
        }

        openPlace.Participant = participant;

        _DataStore.AddOrUpdate(openPlace);

        Participants.Remove(participant);
        _DataStore.Delete(participant);

        await InvokeAsync(StateHasChanged);
    }

    private ParticipantSignInPlace FindEmptySignInPlace()
    {
        return ParticipantSignInPlaces.OrderBy(a => a.Id).FirstOrDefault(x => x.Participant == null);
    }

    private async Task AddParticipantAsync(Participant participant)
    {
        if (participant == null)
        {
            return;
        }

        var oldParticipant = Participants
                                .Concat(ParticipantSignInPlaces.Where(x => x.Participant != null).Select(x => x.Participant))
                                .Concat(_DataStore.Load<SignInProtocol>().Where(x => x.Participant != null).Select(x => x.Participant))
                                .FirstOrDefault(x => x.Id == participant.Id);
        if (oldParticipant == null)
        {
            _DataStore.AddOrUpdate(participant);
            Participants.Add(participant);

            await _JSRuntime.InvokeAsync<string>("PlayAudio", participant.Revoked != 0 ? "revokedParticipantSound" : "newParticipantSound");

            AnimationClass = "glow-shadow";
            await InvokeAsync(StateHasChanged);
            await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
            AnimationClass = "";
        }
        else
        {
            var oldId = Participants.IndexOf(oldParticipant);
            if (oldId >= 0)
            {
                Participants.Remove(oldParticipant);
                Participants.Insert(oldId, participant);
                _DataStore.AddOrUpdate(participant);
            }
            else
            {
                var participantInSignInPlace = ParticipantSignInPlaces.Find(x => x.Participant == oldParticipant);
                if (participantInSignInPlace != null)
                {
                    participantInSignInPlace.Participant = participant;
                    _DataStore.AddOrUpdate(participantInSignInPlace);
                }
            }
        }

        await InvokeAsync(StateHasChanged);
    }

    public void ResetTimerInterval()
    {
        _InactiveTimer.Stop();
        _InactiveTimer.Start();
    }

    private void UserInactive(Object source, ElapsedEventArgs e)
    {
        _NavigationManager.NavigateTo(string.Empty);
    }


    #region IDisposable
    void IDisposable.Dispose()
    {
        _BackgroundTrigger.Unsubscribe(Trigger);
    }
    #endregion

}