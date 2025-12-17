using Stackbuld.Assessment.CSharp.Presentation.Abstractions;
using Stackbuld.Assessment.CSharp.Presentation.Middleware;

namespace Stackbuld.Assessment.CSharp.Presentation.Extensions;

public static class ConfigureServices
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddHealthChecks();

        return services;
    }
}

public static class EndpointRegistrationExtensions
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        var endpointTypes = typeof(IEndpoint).Assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && typeof(IEndpoint).IsAssignableFrom(t));

        foreach (var type in endpointTypes)
        {
            var instance = (IEndpoint)Activator.CreateInstance(type)!;
            instance.MapEndpoints(app);
        }
    }
}