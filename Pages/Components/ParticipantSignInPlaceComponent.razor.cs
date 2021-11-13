using Microsoft.AspNetCore.Components;
using EventulaEntranceClient.Services;
using EventulaEntranceClient.Models;

namespace EventulaEntranceClient.Pages.Components
{
    public partial class ParticipantSignInPlaceComponent
    {
        #region Parameters

        [Parameter]
        public ParticipantSignInPlace SignInPlace { get; set; }

        #endregion

        #region Injects

        [Inject]
        private ILogger<ParticipantSignInPlaceComponent> _Logger { get; set; }


        [Inject]
        private IDataStore _DataStore { get; set; }

        [Inject]
        private EventulaApiService _EventulaApiService { get; set; }

        [Inject]
        private BackgroundTrigger _BackgroundTrigger { get; set; }

        #endregion

        #region Fields

        private const int _TimerTargetSeconds = 15 * 60 / 60;
        private static System.Timers.Timer _Timer;
        #endregion

        public ParticipantSignInPlaceComponent()
        {
            _Timer = new System.Timers.Timer(300);
            _Timer.Elapsed += CountDownTimer;
            _Timer.Enabled = true;
        }

        #region Properties



        public bool HasNoParticipant => SignInPlace.Participant == null;

        public double Progress { get; set; }

        public string TimeLeft { get; set; }

        public bool IsPaid
        {
            get => SignInPlace != null && SignInPlace.Paid != default || (SignInPlace.Participant?.Purchase != null && SignInPlace.Participant.Purchase.Status.Equals("Success", StringComparison.OrdinalIgnoreCase));
            set => ActionWithSave(async () =>
            {
                if (value && await SetIsPaid())
                {
                    SignInPlace.Paid = DateTimeOffset.Now;
                }
                else
                {
                    SignInPlace.Paid = default;
                }

                await InvokeAsync(StateHasChanged);
            });
        }

        public bool IsPaidDisabled => SignInPlace.Participant?.Purchase == null || SignInPlace.Participant.Purchase.Status.Equals("Success", StringComparison.OrdinalIgnoreCase);

        public bool IsCoronaChecked
        {
            get => SignInPlace != null && SignInPlace.CoronaCheck != default;
            set => ActionWithSave(() => SignInPlace.CoronaCheck = value ? DateTimeOffset.Now : default);
        }

        public bool IsCoronaTestChecked
        {
            get => SignInPlace != null && SignInPlace.CoronaTestCheck != default;
            set => ActionWithSave(() => SignInPlace.CoronaTestCheck = value ? DateTimeOffset.Now : default);
        }

        public bool IsCoronaTestDisabled => SignInPlace.Participant == null || Progress < 100;

        public bool IsTermsChecked
        {
            get => SignInPlace != null && SignInPlace.Terms != default;
            set => ActionWithSave(() => SignInPlace.Terms = value ? DateTimeOffset.Now : default);
        }

        public bool SignInDisabled => !IsPaid
                                   || !IsCoronaChecked
                                   || !IsCoronaTestChecked
                                   || !IsTermsChecked
                                   || SignInPlace.Participant.SignedIn;

        #endregion

        public void StartTimer()
        {
            SignInPlace.TimerStartTime = DateTimeOffset.Now;
            _DataStore.AddOrUpdate(SignInPlace);
        }

        public void Delete()
        {
            SignInPlace.Participant = null;
            SignInPlace.TimerStartTime = default;
            SignInPlace.CoronaCheck = default;
            SignInPlace.CoronaTestCheck = default;
            SignInPlace.Paid = default;
            SignInPlace.Terms = default;
            _DataStore.AddOrUpdate(SignInPlace);
        }

        private async Task<bool> SetIsPaid()
        {
            try
            {
                // Call Eventula API
                var result = await _EventulaApiService.SetIsPaidForParticipant(SignInPlace.Participant).ConfigureAwait(false);
                var resultTicket = await _EventulaApiService.RequestTicket(SignInPlace.Participant.Id.ToString());

                if (resultTicket.Successful)
                {
                    SignInPlace.Participant = resultTicket.Participant;
                }

                return result.Successful;
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, $"Error set is paid for participant {SignInPlace.Participant.Id}");
            }

            return false;
        }

        public async Task SignIn()
        {
            try
            {
                // Call Eventula API
                var result = await _EventulaApiService.SignInParticipant(SignInPlace.Participant).ConfigureAwait(false);


                if (result.Successful)
                {
                    // Save protocol to db
                    var protocol = new SignInProtocol()
                    {
                        Participant = SignInPlace.Participant,
                        TimerStartTime = SignInPlace.TimerStartTime,
                        SignInCompleted = DateTimeOffset.Now,
                    };

                    //_DataStore.AddOrUpdate(protocol);

                    // Reset component
                    Delete();
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, $"Error signing in participant {SignInPlace.Participant.Id}");
            }
        }

        public void CountDownTimer(Object source, System.Timers.ElapsedEventArgs e)
        {
            var oldProgress = Progress;
            if (SignInPlace.TimerStartTime != default)
            {

                var timeDone = (DateTimeOffset.Now - SignInPlace.TimerStartTime);

                Progress = Math.Min(_TimerTargetSeconds, timeDone.TotalSeconds) / _TimerTargetSeconds * 100d;
                TimeLeft = FormatTimeSpan(TimeSpan.FromSeconds(_TimerTargetSeconds - Math.Min(_TimerTargetSeconds, timeDone.TotalSeconds)));
            }
            else
            {
                Progress = 0;
                TimeLeft = FormatTimeSpan(TimeSpan.FromSeconds(_TimerTargetSeconds));
            }

            if (oldProgress != Progress)
            {
                InvokeAsync(StateHasChanged);
            }
        }

        private static string FormatTimeSpan(TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"mm\:ss");
        }

        private void ActionWithSave(Action action)
        {
            action();
            _DataStore.AddOrUpdate(SignInPlace);
        }
    }
}