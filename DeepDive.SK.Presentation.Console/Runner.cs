using System.Text;
using DeepDive.SK.Infrastructure.SemanticKernel;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using ConsoleApp = System.Console;

namespace DeepDive.SK.Presentation.Console;

public class Runner(ILogger<Runner> logger, IKernelChatService kernelChatService)
{
    public async Task Start()
    {
        logger.LogInformation("Runner start");

        //Create a persona by using the system message
        ChatHistory chatMessages = new(systemMessage: """
        You are a programmer who knows how to handle Azure DevOps and likes to perform code reviews.
        Your style is honest and direct, but you always have a mentoring approach.
        """);

        string? userInput;
        do
        {
            ConsoleApp.Write("User > ");
            userInput = ConsoleApp.ReadLine();
            if (userInput != null)
            {
                var result = kernelChatService.ProcessUserPrompt(chatMessages, userInput);

                ConsoleApp.Write("Assistant > ");
                // Stream the results
                StringBuilder fullMessage = new();
                await foreach (var content in result)
                {
                    ConsoleApp.Write(content.Content);
                    fullMessage.Append(content.Content);
                }

                kernelChatService.ProcessAssistantReply(chatMessages, fullMessage.ToString());
            }
            ConsoleApp.WriteLine("\n--------------------------------------------------------------");
        }
        while (userInput != null);

        logger.LogInformation("Runner end");
    }
}