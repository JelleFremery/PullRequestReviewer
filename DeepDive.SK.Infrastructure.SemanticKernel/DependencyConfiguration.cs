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
        services.AddTransient<IFileReviewService, FileReviewService>();
        services.AddScoped<IKernelChatService, KernelChatService>();

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
        services.AddTransient<IKernelFactory, KernelFactory>();
        services.AddTransient<Kernel>(serviceProvider =>
        {
            var kernelBuilder = Kernel.CreateBuilder().ConfigureBuilder();

            pluginInterfaces.ForEach(type =>
            {
                kernelBuilder.Plugins.AddFromObject(serviceProvider.GetRequiredService(type));
            });

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