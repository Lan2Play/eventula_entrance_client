using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ElectronNET.API;
using EventulaEntranceClient.Services.Interfaces;
using EventulaEntranceClient.Services;
using Microsoft.Extensions.Logging;
using EventulaEntranceClient.Pages;
using System.Net;
using System.Net.Http;

namespace EventulaEntranceClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSignalR(e => { e.MaximumReceiveMessageSize = 102400000; });
            services.AddSingleton<CookieContainer>();

            services.AddSingleton<BackgroundTrigger>();
            services.AddSingleton<ProtectionService>();
            services.AddScoped<EventulaTokenService>();
            services.AddScoped<EventulaApiService>();

            services.AddSingleton<IBarcodeService>(sp => new ZXingBarcodeService(sp.GetRequiredService<ILogger<ZXingBarcodeService>>()));

            services.AddHttpClient(nameof(Management), client =>
            {
                client.BaseAddress = new System.Uri("https://lan2play.de");
            }).ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var cookieContainer = sp.GetRequiredService<CookieContainer>();
                return new HttpClientHandler { CookieContainer = cookieContainer };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
        }
    }
}
