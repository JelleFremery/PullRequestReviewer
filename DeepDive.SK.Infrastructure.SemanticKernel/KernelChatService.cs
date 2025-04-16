using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace DeepDive.SK.Infrastructure.SemanticKernel;

public class KernelChatService : IKernelChatService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatService;
    private readonly OpenAIPromptExecutionSettings _openAIPromptExecutionSettings;

    //Reuse same toolcallbehavior.
    public KernelChatService(Kernel kernel)
    {
        _kernel = kernel;
        _chatService = kernel.GetRequiredService<IChatCompletionService>();
        _openAIPromptExecutionSettings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };
    }

    public void ProcessAssistantReply(ChatHistory chat, string assistantOutput)
    {
        chat.AddAssistantMessage(assistantOutput);
    }

    public IAsyncEnumerable<StreamingChatMessageContent> ProcessUserPrompt(ChatHistory chat, string userInput)
    {
        //This is a great place to add any custom logic to the message input before sending it to the chat service,
        //such as logging, prompt templating, or input and output message validation.
        //Also, ideally map ChatMessageContent and ChatHistory to domain models for easier consumption by the application.
        chat.AddUserMessage(userInput);
        return _chatService.GetStreamingChatMessageContentsAsync(chat, _openAIPromptExecutionSettings, _kernel);
    }
}