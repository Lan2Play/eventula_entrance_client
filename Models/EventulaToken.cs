using EventulaEntranceClient.Storage;
using LiteDB;

namespace EventulaEntranceClient.Models;

public class EventulaToken : IStoreObject
{
    [BsonId]
    public int Id { get; set; }

    public string Token { get; set; }
}
