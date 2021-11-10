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

        public double Progress { get; set; }

        public string TimeLeft { get; set; }

        #endregion

        public void StartTimer()
        {
            SignInPlace.TimerStartTime = DateTimeOffset.Now;
            _DataStore.AddOrUpdate(SignInPlace);
        }

        public void CountDownTimer(Object source, System.Timers.ElapsedEventArgs e)
        {
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

            InvokeAsync(StateHasChanged);
        }

        private static string FormatTimeSpan(TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"mm\:ss");
        }
    }
}