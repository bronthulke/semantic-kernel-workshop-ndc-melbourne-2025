using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernelWorkshop.Plugins;
using System.Text;

// Create the host
var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddUserSecrets<Program>();

        // Ensure the environment variable is set correctly
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        // Ensure configuration sources are loaded in the correct order
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

        // Verify the environment - uncomment this if your appsettings.Development.json does not appear to be loaded
        Console.WriteLine($"Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
    })
    
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Add configuration to DI
        services.AddSingleton(configuration);

        // Initialize the Semantic Kernel
        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.Services.AddSingleton(configuration);

        // With Azure Open AI
        var modelId = "gpt-4o";
        var apiKey = configuration["AzureOpenAIAPIKey"];
        var endpoint = configuration["AzureOpenAIEndpoint"];
        kernelBuilder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

        // Add the plugins - uncomment to use them
        kernelBuilder.Plugins
            .AddFromType<AuthorEmailPlugin>()
            .AddFromType<PersonalDetailsPlugin>()
            .AddFromType<PublicHolidaysPlugin>()
            .AddFromType<DateHelpers>()
            .AddFromType<MyTimePlugin>();
            
        // Extra plugins to play with:
            // .AddFromType<MyPluginTemplate>()
            // .AddFromType<TimePlugin>()
            // .AddFromObject(new MyLightPlugin(turnedOn: true))
            // .AddFromObject(new MyAlarmPlugin("11"));

        // Add the kernel to DI
        services.AddSingleton(kernelBuilder.Build());
    })
    .Build();

// Resolve services
var kernel = host.Services.GetRequiredService<Kernel>();

// Create a new chat
var ai = kernel.GetRequiredService<IChatCompletionService>();

ChatHistory chatHistory = new("""
You are a friendly assistant responsible for helping with tasks as a personal assistant, like sending emails. You have access to a set of tools and plugins to help you fulfill requests. You will complete required steps and request approval before taking any consequential actions.

If the user doesn't provide enough information to complete a task, you must ask follow-up questions until you have all necessary details. You are especially careful with emails:
1. Do not fabricate email addresses. 
2. If no email is provided, ask for clarification. For example: "What is Jody's email address?"
3. Do not proceed with incomplete or placeholder content in the email subject, body, or recipient field.

Always keep responses concise, under 200 characters where possible, as the user may not have much time.
""");

StringBuilder stringBuilder = new();

// User question & answer loop
while (true)
{
    stringBuilder.Clear();

    Console.WriteLine();
    Console.WriteLine("Your request: ");
    string request = Console.ReadLine()!;

    // Set up automatic function calling
    OpenAIPromptExecutionSettings settings = new()
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    };

    // Get the AI response streamed back to the console
    chatHistory.AddUserMessage(request);

    Console.WriteLine("Assistant: ");
    await foreach (var message in ai.GetStreamingChatMessageContentsAsync(
        chatHistory,
        kernel: kernel,
        executionSettings: settings))
    {
        Console.Write(message);
        stringBuilder.Append(message.Content);
    }

    chatHistory.AddAssistantMessage(stringBuilder.ToString());
    Console.WriteLine();
}