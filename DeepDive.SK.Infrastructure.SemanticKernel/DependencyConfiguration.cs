using System.Reflection;
using DeepDive.SK.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

namespace DeepDive.SK.Infrastructure.SemanticKernel;

public static class DependencyConfiguration
{
    public static IServiceCollection ConfigureInfrastructureDependencies(this IServiceCollection services)
    {
        var pluginTypes = Scanner();

        var pluginInterfaces = new List<Type>();

        pluginTypes.ForEach(type =>
        {
            var serviceType = type.GetInterfaces()
            .ToList()
            .Find(it => it.Name == $"I{type.Name}");

            if (serviceType == null) return;
            pluginInterfaces.Add(serviceType);
            services.AddSingleton(serviceType, type);
        });
        services.AddTransient<Kernel>(serviceProvider =>
        {
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddLogging(l => l
                .SetMinimumLevel(LogLevel.Information)
                .AddConsole()
            );

            pluginInterfaces.ForEach(type =>
            {
                kernelBuilder.Plugins.AddFromObject(serviceProvider.GetRequiredService(type));
            });

            // Let's add the built-in, experimental web search plugin for fun!            
#pragma warning disable SKEXP0050
            var textSearch = new BingTextSearch(apiKey: InfraConfig.BingKey);

            // Build a text search plugin with Bing search and add to the kernel
            var searchPlugin = textSearch.CreateWithSearch("SearchPlugin");
            kernelBuilder.Plugins.Add(searchPlugin);
#pragma warning restore SKEXP0050

            kernelBuilder.AddAzureOpenAIChatCompletion(
                deploymentName: InfraConfig.DeploymentName,
                endpoint: InfraConfig.ApiEndpoint,
                apiKey: InfraConfig.ApiKey);

            var kernel = kernelBuilder.Build();
            return kernel;
        });
        return services;
    }

    private static List<Type> Scanner()
    {
        var interfaceType = typeof(IKernelPlugin);
        // Get all loaded assemblies
        return Assembly.GetExecutingAssembly().GetTypes().Where(type =>
            type is { IsInterface: false, IsAbstract: false, IsClass: true } &&
            interfaceType.IsAssignableFrom(type)
        ).ToList();
    }
}