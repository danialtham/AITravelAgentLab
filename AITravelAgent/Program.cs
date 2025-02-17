using System.Text;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();
var configuration = builder.Build();

string yourDeploymentName = "gpt-4o";
string yourEndpoint = configuration["AZURE_OPENAI_ENDPOINT_4O"];
string yourApiKey = configuration["AZURE_OPENAI_API_KEY_4O"];

var kernel = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(yourDeploymentName, yourEndpoint, yourApiKey).Build();

// Note: ChatHistory isn't working correctly as of SemanticKernel v 1.4.0
StringBuilder chatHistory = new();

var prompts = kernel.ImportPluginFromPromptDirectory("Prompts");


