using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace GameGrid.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));
        services.AddValidatorsFromAssembly(assembly);
        
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(MediatR.IPipelineBehavior<,>), typeof(GameGrid.Application.Common.Behaviors.ValidationBehavior<,>));
        });

        return services;
    }
}
