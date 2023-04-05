using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;
using TimeShit;
using TimeShit.Services;
using TimeShit.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<TimeShit.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ITPService, TPService>();
builder.Services.AddRefitClient<IRefitTargetProcess>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(Constants.TPUrl);
    });

await builder.Build().RunAsync();
