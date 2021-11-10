using EventulaEntranceClient.Storage;

namespace EventulaEntranceClient.Models
{
    public class EventulaToken : IStoreObject
    {
        public int Id { get; set; }

        public string Token { get; set; }
    }
}
