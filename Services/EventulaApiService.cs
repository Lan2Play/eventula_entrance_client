using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EventulaEntranceClient.Services;

public class EventulaApiService
{
    #region consts

    private const string _CsrfCookieUrl = "sanctum/csrf-cookie";
    private const string _UserApiParticipantUrl = "api/admin/event/participants/{0}";

    private const string _UserApiParticipantSignInUrl = "api/admin/event/participants/{0}/signIn";
    private const string _UserApiParticipantPaidUrl = "/api/admin/purchases/{0}/setSuccess";

    #endregion

    private readonly IHttpClientFactory _HttpClientFactory;
    private readonly ILogger<EventulaApiService> _Logger;
    private readonly CookieContainer _Cookies;
    private readonly SettingsService _SettingsService;

    public EventulaApiService(IHttpClientFactory httpClientFactory, CookieContainer cookies, SettingsService settingsService, ILogger<EventulaApiService> logger)
    {
        _HttpClientFactory = httpClientFactory;
        _Cookies = cookies;
        _SettingsService = settingsService;
        _Logger = logger;
    }

    public async Task<TicketRequest> RequestTicket(string qrCode)
    {
        using var httpClient = _HttpClientFactory.CreateClient(nameof(EventulaApiService));

        await SetXcsrfHeader(httpClient);
        SetDefaultHeaders(httpClient, _SettingsService.RetrieveToken());

        var getResult = await httpClient.GetAsync(string.Format(_UserApiParticipantUrl, qrCode.Split('/').Last()));

        getResult.EnsureSuccessStatusCode();

        var content = await getResult.Content.ReadAsStringAsync();

        var ticketRequest = JsonSerializer.Deserialize<TicketRequest>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return ticketRequest;

    }

    public async Task<TicketRequest> SetIsPaidForParticipant(Participant participant)
    {
        using var httpClient = _HttpClientFactory.CreateClient(nameof(EventulaApiService));

        await SetXcsrfHeader(httpClient);
        SetDefaultHeaders(httpClient, _SettingsService.RetrieveToken());

        var getResult = await httpClient.GetAsync(string.Format(_UserApiParticipantPaidUrl, participant.Purchase.Id));

        getResult.EnsureSuccessStatusCode();

        var content = await getResult.Content.ReadAsStringAsync();

        var ticketRequest = JsonSerializer.Deserialize<TicketRequest>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return ticketRequest;
    }

    public async Task<TicketRequest> SignInParticipant(Participant participant)
    {
        using var httpClient = _HttpClientFactory.CreateClient(nameof(EventulaApiService));

        await SetXcsrfHeader(httpClient);
        SetDefaultHeaders(httpClient, _SettingsService.RetrieveToken());

        var getResult = await httpClient.GetAsync(string.Format(_UserApiParticipantSignInUrl, participant.Id));

        getResult.EnsureSuccessStatusCode();

        var content = await getResult.Content.ReadAsStringAsync();

        var ticketRequest = JsonSerializer.Deserialize<TicketRequest>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return ticketRequest;
    }


    private async Task SetXcsrfHeader(HttpClient httpClient)
    {
        var csrfCookieUrl = new Uri(httpClient.BaseAddress, _CsrfCookieUrl);

        var csrfToken = GetCsrfToken(csrfCookieUrl);

        if (csrfToken == null)
        {
            await httpClient.GetAsync(_CsrfCookieUrl);
            csrfToken = GetCsrfToken(csrfCookieUrl);
        }

        if (csrfToken != null)
        {
            httpClient.DefaultRequestHeaders.Add("X-CSRF-Token", csrfToken.Value);
        }
    }

    private Cookie GetCsrfToken(Uri csrfCookieUrl)
    {
        var csrfCookies = _Cookies.GetCookies(csrfCookieUrl);
        var csrfToken = csrfCookies.FirstOrDefault(x => x.Name.Equals("XSRF-TOKEN", StringComparison.Ordinal));
        return csrfToken;
    }

    private static void SetDefaultHeaders(HttpClient httpClient, string token)
    {
        var requestToken = token.Split('|').Last();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestToken);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
