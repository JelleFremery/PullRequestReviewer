using Microsoft.Extensions.DependencyInjection;

namespace DeepDive.SK.Application;

public static class DependencyConfiguration
{
    public static void ConfigureApplicationDependencies(this IServiceCollection services)
    {
        services.AddScoped<IReviewerUseCase, ReviewerWorkCase>();
    }
}