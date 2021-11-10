using System.Text.Json.Serialization;

namespace EventulaEntranceClient.Models
{
    public class ParticipantSignInPlace : IStoreObject
    {
        public int Id { get; set; }

        public Participant Participant { get; set; }

        public DateTimeOffset TimerStartTime { get; set; }
    }
}
