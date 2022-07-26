using LiteDB;

namespace EventulaEntranceClient.Models;

public class SignInProtocol : IStoreObject
{
    [BsonId]
    public int Id { get; set; }

    public Participant Participant { get; set; }

    public DateTimeOffset TimerStartTime { get; set; }

    public DateTimeOffset SignInCompleted { get; set; }
}
