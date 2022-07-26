using LiteDB;

namespace EventulaEntranceClient.Models;

public class ParticipantSignInPlace : IStoreObject
{
    [BsonId]
    public int Id { get; set; }

    public Participant Participant { get; set; }

    public DateTimeOffset TimerStartTime { get; set; }

    public DateTimeOffset Paid { get; set; }
    public DateTimeOffset CoronaCheck { get; set; }
    public DateTimeOffset CoronaTestCheck { get; set; }
    public DateTimeOffset Terms { get; set; }
}
