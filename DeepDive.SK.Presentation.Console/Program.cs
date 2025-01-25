// See https://aka.ms/new-console-template for more information
using DeepDive.SK.Infrastructure.ADO;
using DeepDive.SK.Infrastructure.SemanticKernel;
using DeepDive.SK.Presentation.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = CreateServices(LogLevel.Information);
Runner runner = services.GetRequiredService<Runner>();
await runner.Start();

ServiceProvider CreateServices(LogLevel logLevel)
{
    var serviceProvider = new ServiceCollection()
        .AddLogging(options =>
        {
            options.ClearProviders();
            options.SetMinimumLevel(logLevel);
            options.AddConsole();
        })
        .ConfigureInfrastructureDependencies()
        .ConfigureAdoDependencies()
        .AddSingleton<Runner>()
        .BuildServiceProvider();

    return serviceProvider;
}
