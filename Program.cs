using ElectronNET.API;
using ElectronNET.API.Entities;
using EventulaEntranceClient.Services;
using EventulaEntranceClient.Services.Interfaces;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);  // add this line here

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddElectron();

// Webcam pictures need to be transferred via SignalR and they need a bigger message size
builder.Services.AddSignalR(e => { e.MaximumReceiveMessageSize = 102400000; });


builder.Services.AddSingleton<CookieContainer>();
builder.Services.AddSingleton<BackgroundTrigger>();
builder.Services.AddSingleton<UiNotifyService>();
builder.Services.AddSingleton<IDataStore, LiteDbDataStore>();
builder.Services.AddSingleton<IBarcodeService, ZXingBarcodeService>();

builder.Services.AddScoped<SettingsService>();
builder.Services.AddScoped<EventulaApiService>();
builder.Services.AddScoped<ProtectionService>();

builder.Services.AddHttpClient(nameof(EventulaApiService), (serviceProvider, client) =>
{
    var settingsService = serviceProvider.GetService<SettingsService>();
    client.BaseAddress = new Uri(settingsService.RetrieveEventulaApiBaseAddress());
}).ConfigurePrimaryHttpMessageHandler(sp =>
{
    var cookieContainer = sp.GetRequiredService<CookieContainer>();
    return new HttpClientHandler { CookieContainer = cookieContainer };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapPost("/adduserbyticket", async (http) =>
{
    using var reader = new StreamReader(http.Request.Body);

    var body = await reader.ReadToEndAsync();

    // Get ticket
    var eventulaApiService = http.RequestServices.GetService<EventulaApiService>();
    var ticket = await eventulaApiService.RequestTicket(body).ConfigureAwait(false);

    // Store participant
    var dataStore = http.RequestServices.GetService<IDataStore>();
    dataStore.AddOrUpdate(ticket.Participant);

    // Notify UI
    var uiNotifyService = http.RequestServices.GetService<UiNotifyService>();
    uiNotifyService.NotifyUi(ticket.Participant);

    http.Response.StatusCode = 200;
});

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.StartAsync();

if (HybridSupport.IsElectronActive)
{
    var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { AutoHideMenuBar = true, Fullscreen = true }).ConfigureAwait(false);
    window.OnClosed += () => Electron.App.Exit();
}

await app.WaitForShutdownAsync().ConfigureAwait(false);