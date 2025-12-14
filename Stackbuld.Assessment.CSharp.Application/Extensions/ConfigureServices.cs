using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Stackbuld.Assessment.CSharp.Application.Common.Behaviours;

namespace Stackbuld.Assessment.CSharp.Application.Extensions;

public static class ConfigureServices
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            //cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}