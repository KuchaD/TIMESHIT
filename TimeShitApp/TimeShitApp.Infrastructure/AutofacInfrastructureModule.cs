using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using TimeShit;
using TimeShit.Services;

namespace TimeShitApp.Application;

public class AutofacInfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var services = new ServiceCollection();

        services.AddRefitClient<IRefitTargetProcess>()
            .ConfigureHttpClient(c => { c.BaseAddress = new Uri(Constants.TPUrl); });
        
        builder.Populate(services);
        builder.RegisterType<TPService>().AsImplementedInterfaces();
    }
}