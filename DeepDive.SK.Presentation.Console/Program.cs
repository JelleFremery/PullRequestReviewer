// See https://aka.ms/new-console-template for more information
using DeepDive.SK.Infrastructure.ADO;
using DeepDive.SK.Infrastructure.SemanticKernel;
using DeepDive.SK.Presentation.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

try
{
    var services = CreateServices(LogLevel.Information);
    Runner runner = services.GetRequiredService<Runner>();
    await runner.Start();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
}
finally
{
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}

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
