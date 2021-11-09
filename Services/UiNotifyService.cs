using EventulaEntranceClient.Models;

namespace EventulaEntranceClient.Services
{
    public class UiNotifyService
    {
        private readonly ILogger<UiNotifyService> _Logger;

        public UiNotifyService(ILogger<UiNotifyService> logger)
        {
            _Logger = logger;
        }

        public void NotifyUi(Participant participant)
        {
            NewParticipant?.Invoke(this, participant);
        }


        public event EventHandler<Participant> NewParticipant;
    }
}
