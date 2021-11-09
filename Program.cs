using ElectronNET.API;
using EventulaEntranceClient.Pages;
using EventulaEntranceClient.Services;
using EventulaEntranceClient.Services.Interfaces;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);  // add this line here

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSignalR(e => { e.MaximumReceiveMessageSize = 102400000; });
builder.Services.AddSingleton<CookieContainer>();

builder.Services.AddSingleton<BackgroundTrigger>();
builder.Services.AddScoped<UiNotifyService>();
builder.Services.AddSingleton<ProtectionService>();
builder.Services.AddScoped<EventulaTokenService>();
builder.Services.AddScoped<EventulaApiService>();


builder.Services.AddSingleton<IBarcodeService>(sp => new ZXingBarcodeService(sp.GetRequiredService<ILogger<ZXingBarcodeService>>()));

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

app.MapPost("/addticket", async (http) =>
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
