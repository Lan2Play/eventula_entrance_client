using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Microsoft.Extensions.Logging;
using EventulaEntranceClient;
using EventulaEntranceClient.Shared;
using EventulaEntranceClient.Services;
using EventulaEntranceClient.Services.Interfaces;
using System.Threading;

namespace EventulaEntranceClient.Pages.Components
{
    public partial class ParticipantSignInPlaceComponent
    {
        #region Parameters

        [Parameter]
        public Models.ParticipantSignInPlace SignInPlace { get; set; }

        #endregion

        #region Injects

        [Inject]
        private IDataStore _DataStore { get; set; }

        [Inject]
        private EventulaApiService _EventulaApiService { get; set; }

        [Inject]
        private BackgroundTrigger _BackgroundTrigger { get; set; }

        #endregion

        #region Fields

        private const int _TimerTargetSeconds = 15 * 60;
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
            set => ActionWithSave(() => SignInPlace.Paid = value ? DateTimeOffset.Now : default);
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
                                   || !IsTermsChecked;

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