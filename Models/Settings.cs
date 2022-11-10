using EventulaEntranceClient.Storage;
using LiteDB;

namespace EventulaEntranceClient.Models;

public class Settings : IStoreObject
{
    [BsonId]
    public int Id { get; set; }

    public string Token { get; set; }

    public string EventulaApiBaseAddress { get; set; }

    public bool EnableTwoGVerification { get; set; }

    public bool EnableTermsChecked { get; set; }

    public bool EnableCovidTest { get; set; }

    public int TestTimeInMinutes { get; set; }
    
    public string CustomBackgroundImage { get; set; }

}
