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

string hbprompt = """
    <message role="system">Instructions: Before providing the the user with a travel itinerary, ask how many days their trip is.</message>
    <message role="user">I'm going to Rome. Can you create an itinerary for me?</message>
    <message role="assistant">Sure, how many days is your trip?</message>
    <message role="user">{{input}}</message>
""";

var templateFactory = new HandlebarsPromptTemplateFactory();
var promptTemplateConfig = new PromptTemplateConfig()
{
    Template = hbprompt,
    TemplateFormat = "handlebars",
    Name = "CreateItinerary",
};

var function = kernel.CreateFunctionFromPrompt(promptTemplateConfig, templateFactory);
var plugin = kernel.CreatePluginFromFunctions("TravelItinerary", [function]);
kernel.Plugins.Add(plugin);

var history = new ChatHistory();
history.AddSystemMessage("Before providing destination recommendations, ask the user if they have a budget for their trip.");

Console.WriteLine("Press enter to exit");
Console.WriteLine("Assistant: How may I help you?");
Console.Write("User: ");

string input = Console.ReadLine()!;
while (input != "")
{
    history.AddUserMessage(input);
    await GetReply();
    Console.Write("User: ");
    input = Console.ReadLine()!;
}

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