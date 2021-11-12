using EventulaEntranceClient.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EventulaEntranceClient.Services
{
    public class EventulaApiService
    {
        #region consts

        private const string _CsrfCookieUrl = "sanctum/csrf-cookie";
        private const string _UserApiParticipantUrl = "api/admin/event/participants/{0}";

        private const string _UserApiParticipantSignInUrl = "api/admin/event/participants/{0}/signIn";

        #endregion

        private readonly IHttpClientFactory _HttpClientFactory;
        private readonly ILogger<EventulaApiService> _Logger;
        private readonly CookieContainer _Cookies;
        private readonly EventulaTokenService _EventulaTokenService;

        public EventulaApiService(IHttpClientFactory httpClientFactory, CookieContainer cookies, EventulaTokenService eventulaTokenService, ILogger<EventulaApiService> logger)
        {
            _HttpClientFactory = httpClientFactory;
            _Cookies = cookies;
            _EventulaTokenService = eventulaTokenService;
            _Logger = logger;
        }

        public async Task<TicketRequest> RequestTicket(string qrCode)
        {
            using var httpClient = _HttpClientFactory.CreateClient(nameof(EventulaApiService));

            await SetXcsrfHeader(httpClient);
            SetDefaultHeaders(httpClient, await _EventulaTokenService.RetrieveTokenAsync());

            var getResult = await httpClient.GetAsync(string.Format(_UserApiParticipantUrl, qrCode.Split('/').Last()));
                 
            getResult.EnsureSuccessStatusCode();

            var content = await getResult.Content.ReadAsStringAsync();

            var ticketRequest = JsonSerializer.Deserialize<TicketRequest>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return ticketRequest;

        }

        public async Task<TicketRequest> SignInParticipant(Participant participant)
        {
            using var httpClient = _HttpClientFactory.CreateClient(nameof(EventulaApiService));

            await SetXcsrfHeader(httpClient);
            SetDefaultHeaders(httpClient, await _EventulaTokenService.RetrieveTokenAsync());

            var getResult = await httpClient.GetAsync(string.Format(_UserApiParticipantSignInUrl, participant.Id));

            getResult.EnsureSuccessStatusCode();

            var content = await getResult.Content.ReadAsStringAsync();

            var ticketRequest = JsonSerializer.Deserialize<TicketRequest>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return ticketRequest;
        }


        private async Task SetXcsrfHeader(HttpClient httpClient)
        {
            var csrfCookieUrl = new Uri(httpClient.BaseAddress, _CsrfCookieUrl);


            var csrfCookies = _Cookies.GetCookies(csrfCookieUrl);

            var csrfToken = csrfCookies.FirstOrDefault(x => x.Name.Equals("XSRF-TOKEN", StringComparison.Ordinal));

            if (csrfToken == null)
            {
                var getCsrfResult = await httpClient.GetAsync(_CsrfCookieUrl);
                csrfCookies = _Cookies.GetCookies(csrfCookieUrl);
                csrfToken = csrfCookies.FirstOrDefault(x => x.Name.Equals("XSRF-TOKEN", StringComparison.Ordinal));
            }

            if (csrfToken != null)
            {
                httpClient.DefaultRequestHeaders.Add("X-CSRF-Token", csrfToken.Value);
            }
        }

        private static void SetDefaultHeaders(HttpClient httpClient, string token)
        {
            var requestToken = token.Split('|').Last();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
