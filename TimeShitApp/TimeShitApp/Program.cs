
using System.Collections;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BlazorDownloadFile;
using Blazored.SessionStorage;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Refit;
using TimeShit;
using TimeShit.Services;
using TimeShitApp.Application;
using TimeShitApp.Application.ServicesInterfaces;
using TimeShitApp.Data;
using TimeShitApp.Options;
using TimeShitApp.Share;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new AutofacApplicationModule());
        builder.RegisterModule(new AutofacInfrastructureModule());
    });

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMediator();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddAntDesign();
builder.Services.AddOptions<GeneralSetting>().Bind(builder.Configuration.GetSection(nameof(GeneralSetting)));
builder.Services.AddBlazorDownloadFile();


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