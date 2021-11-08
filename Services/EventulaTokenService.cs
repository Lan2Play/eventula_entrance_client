using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace EventulaEntranceClient.Services
{
    public class EventulaTokenService
    {
        private const string _TokenIdentifier = "eventulatoken";
        private readonly IJSRuntime _JSRuntime;
        private readonly ILogger<EventulaTokenService> _Logger;

        public EventulaTokenService(IJSRuntime jSRuntime, ILogger<EventulaTokenService> logger)
        {
            _JSRuntime = jSRuntime;
            _Logger = logger;
        }

        public async Task<bool> SaveTokenAsync(string token)
        {
            try
            {
              
                await _JSRuntime.InvokeVoidAsync("localStorage.setItem", _TokenIdentifier, token);

                return true;
            }
            catch(Exception ex)
            {
                _Logger.LogError(ex, "Error saving eventula token.");
            }

            return false;     
        }

        public async Task<string> RetrieveTokenAsync()
        {
            return await _JSRuntime.InvokeAsync<string>("localStorage.getItem", _TokenIdentifier);
        }
    }
}
