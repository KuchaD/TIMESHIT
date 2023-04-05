
using Refit;
using TimeShit;
using TimeShit.Services;
using TimeShit.Services.Interfaces;
using TimeShitApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddScoped<ITPService, TPService>();
builder.Services.AddTransient<BrowserDelegate>();
builder.Services.AddRefitClient<IRefitTargetProcess>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(Constants.TPUrl);
    }).AddHttpMessageHandler<BrowserDelegate>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();