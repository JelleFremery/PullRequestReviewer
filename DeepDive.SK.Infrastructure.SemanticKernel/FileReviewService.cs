using System.Text;
using DeepDive.SK.Domain.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace DeepDive.SK.Infrastructure.SemanticKernel;

public class FileReviewService(Kernel kernel) : IFileReviewService
{
    public async Task<string> ReviewFileAsync(string fileContent)
    {
        var chatService = kernel.GetRequiredService<IChatCompletionService>();
        ChatHistory chatMessages = new(systemMessage: """
            You are a friendly assistant who is a senior developer. 
            You will review code from the user.            
            You will answer with one or two sentences of critical, uplifting code review.
            Preferably focus less on formatting and style and more on potential bugs or bad syntax.
            """);
        chatMessages.AddUserMessage(fileContent);
        var result = chatService.GetStreamingChatMessageContentsAsync(chatMessages);
        StringBuilder review = new();
        await foreach (var content in result)
        {
            review.Append(content);
        }
        return review.ToString();
    }
}