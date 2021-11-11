namespace EventulaEntranceClient.Models
{
    public class SignInProtocol : IStoreObject
    {
        public int Id { get; set; }

        public Participant Participant { get; set; }

        public DateTimeOffset TimerStartTime { get; set; }

        public DateTimeOffset SignInCompleted { get; set; }
    }
}
