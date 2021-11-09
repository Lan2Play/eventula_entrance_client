using ElectronNET.API;
using EventulaEntranceClient.Services;
using EventulaEntranceClient.Services.Interfaces;
using EventulaEntranceClient.Storage;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);  // add this line here

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Webcam pictures need to be transferred via SignalR and they need a bigger message size
builder.Services.AddSignalR(e => { e.MaximumReceiveMessageSize = 102400000; });

builder.Services.AddSingleton<CookieContainer>();
builder.Services.AddSingleton<BackgroundTrigger>();
builder.Services.AddSingleton<UiNotifyService>();
builder.Services.AddSingleton<IDataStore, LiteDbDataStore>();
builder.Services.AddSingleton<ProtectionService>();
builder.Services.AddSingleton<IBarcodeService, ZXingBarcodeService>();


builder.Services.AddScoped<EventulaTokenService>();
builder.Services.AddScoped<EventulaApiService>();

builder.Services.AddHttpClient(nameof(EventulaApiService), client =>
{
    client.BaseAddress = new System.Uri("https://lan2play.de");
}).ConfigurePrimaryHttpMessageHandler(sp =>
{
    var cookieContainer = sp.GetRequiredService<CookieContainer>();
    return new HttpClientHandler { CookieContainer = cookieContainer };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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

    // TODO store participant

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

Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());

app.Run();
