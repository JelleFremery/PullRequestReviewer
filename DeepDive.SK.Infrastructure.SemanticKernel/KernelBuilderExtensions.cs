using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

namespace DeepDive.SK.Infrastructure.SemanticKernel
{
    internal static class KernelBuilderExtensions
    {
        public static IKernelBuilder ConfigureBuilder(this IKernelBuilder kernelBuilder)
        {
            kernelBuilder.Services.AddLogging(l => l
                .SetMinimumLevel(LogLevel.Information)
                .AddConsole()
            );

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

            return kernelBuilder;
        }
    }
}
