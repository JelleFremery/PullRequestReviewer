using DeepDive.SK.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeepDive.SK.Infrastructure.ADO;

public static class DependencyConfiguration
{
    public static IServiceCollection ConfigureAdoDependencies(this IServiceCollection services)
    {
        services.AddScoped<IPullRequestRepository, AdoPullRequestService>();
        services.AddScoped<IPullRequestCommenter, AdoPullRequestService>();
        return services;
    }
}