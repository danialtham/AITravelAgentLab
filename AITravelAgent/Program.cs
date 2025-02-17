using System.Text;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Microsoft.Extensions.Configuration;
using AITravelAgent.Plugins.ConvertCurrency;

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();
var configuration = builder.Build();

string yourDeploymentName = "gpt-4o";
string yourEndpoint = configuration["AZURE_OPENAI_ENDPOINT_4O"];
string yourApiKey = configuration["AZURE_OPENAI_API_KEY_4O"];

var kernel = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(yourDeploymentName, yourEndpoint, yourApiKey).Build();

//var prompts = kernel.ImportPluginFromPromptDirectory("Prompts");
kernel.ImportPluginFromType<CurrencyConverter>();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var history = new ChatHistory();
AddUserMessage("Can you convert 52000 VND to USD?");
await GetReply();



# region helper methods
void AddUserMessage(string msg)
{
    Console.WriteLine("User: " + msg);
    history.AddUserMessage(msg);
}
async Task GetReply()
{
    ChatMessageContent reply = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel
    );

    Console.WriteLine("Assistant: " + reply.ToString());
    history.AddAssistantMessage(reply.ToString());
}
#endregion