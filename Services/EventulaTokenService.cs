using EventulaEntranceClient.Models;
using EventulaEntranceClient.Storage;

namespace EventulaEntranceClient.Services
{
    public class EventulaTokenService
    {
        private const int _TokenIdentifier = 1337;
        private readonly IDataStore _DataStore;
        private readonly ILogger<EventulaTokenService> _Logger;

        public EventulaTokenService(IDataStore dataStore, ILogger<EventulaTokenService> logger)
        {
            _DataStore = dataStore;
            _Logger = logger;
        }

        public bool SaveToken(string token)
        {
            try
            {
                _DataStore.AddOrUpdate(new EventulaToken()
                {
                    Id = _TokenIdentifier,
                    Token = token
                }) ;

                return true;
            }
            catch(Exception ex)
            {
                _Logger.LogError(ex, "Error saving eventula token.");
            }

            return false;     
        }

        public string RetrieveToken()
        {
            return _DataStore.LoadById<EventulaToken>(_TokenIdentifier)?.Token;
        }
    }
}
