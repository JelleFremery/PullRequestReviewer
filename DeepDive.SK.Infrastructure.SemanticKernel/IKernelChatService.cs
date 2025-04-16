using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace DeepDive.SK.Infrastructure.SemanticKernel;

public interface IKernelChatService
{
    void ProcessAssistantReply(ChatHistory chat, string assistantOutput);

    IAsyncEnumerable<StreamingChatMessageContent> ProcessUserPrompt(ChatHistory chat, string userInput);
}