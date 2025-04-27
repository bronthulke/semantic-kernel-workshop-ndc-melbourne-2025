# Creating your own personal assistant with Semantic Kernel

Welcome to my Semantic Kernel workshop at NDC Melbourne.  

I'm Bron Thulke, and I've created this workshop to introduce you to the concept of creating an AI agent to do your personal bidding, using Semantic Kernel with C#.

If you are viewing this during the conference workshop, I'll be running through all of the below, but you're welcome to skip ahead if you like (yeah, I'm that sort of person, too!), or run through it at a later time.

Let's get going!

## Grab the repository
You can either open this repository using a GitHub Codespace, or by cloning it onto your computer and using your preferred IDE (Visual Studio / Visual Studio Code / Rider / etc).

### Opening in Codespace
1. Go to this link to [create a Codespace](https://github.com/codespaces/new/bronthulke/semantic-kernel-workshop-ndc-melbourne-2025?skip_quickstart=true&machine=standardLinux32gb&repo=816523464&ref=main&geo=Australia)
2. Select your preferred region and size, though I recommend `4-Core`.
3. Click the green `Create codespace` button and wait for the magic to happen.

### Cloning Locally
1. At the top of this page, click **Clone**
2. Use your favourite method to clone the code onto your computer (e.g. using CLI or your IDE's tooling).

## Set up Azure Foundry

1. Go to the [Azure AI Foundry](https://ai.azure.com) and sign in using your Microsoft account that has an active Azure subscription
2. Click the **Create project** button
- Enter a project name such as `personal-assistant-workshop`
- Click **Create new hub** and enter a name such as `hub_personal_assistant_workshop`
3. Review the Azure resources that will be created, confirming the correct Subscription has been selected.  Click **Customize** if you need to change it
4. Click the **Create** button

    The creation process will take a couple of minutes.

    It also includes creating a Key Vault and Storage Account. 

    Once created, you should land on the Project page, where you can grab your **API Key** and **Azure AI model inference endpoint**. We will use these values later.
5. In the left hand side menu, click **Model catalog**

    Select a model that you'd like to use.  The choice is yours, but be aware that different models will behave differently and will cost different amounts.  For the demo version we will be using "gpt-4o".
6. Click into your selected model and click the blue **Deploy** button. In the dialog, use the default settings and click "Deploy".
7. Once it has deployed, click the blue "Open in playground" button, and have a quick play with it. You can change the "Model instructions and context" as desired.

We're now ready to start using this deployed model from our code - so let's get some code happening!

> Hint: try using the playground and checking on your model's metrics, by going to the "Models + endpoints" from the left hand side menu. It can be a great way to see how your requests equate to tokens.  Note that the metrics can take a few minutes to update after sending requests in the playground.

## Setting up environment

Back in your Codespace or VS Code (or whatever IDE you're using), let's get started setting up your environment.

1. Create a new file `appsettings.Development.json` and add the following with your own data for each value.  If you are not planning on testing with SendGrid to send actual emails, don't worry about the `FromEmail` and `FromName` values.

    ```
    {
        "AzureOpenAIEndpoint": "<value from AI Foundry>",
        "FromEmail": "<your SendGrid authenticated email>",
        "FromName": "<your name>",
        "OwnerFirstName": "<first name>",
        "OwnerLastName": "<last name>"
    }
    ```
2. Open a command prompt within your VS Code / Codespace (hint: in VS Code, hit `Ctrl`+`~`)
3. Add the following secrets by running each of these commands with your own values from previous steps:
    ```
    dotnet user-secrets set SendGridApiKey <value from SendGrid>
    dotnet user-secrets set AzureOpenAIAPIKey <value from AI Foundry>
    ```

## Reviewing the first version of code - a simple Chatbot

Open the file `Program.cs` and review the code within.  Following is a summary of what the code does:

- Sets up the application host and configuration, adding user secrets for sensitive config, and loading appsettings.json and the environment-specific config file (e.g., appsettings.Development.json) based on the ASPNETCORE_ENVIRONMENT variable (see `.vscode/launch.json`)
- Registers and configures the semantic kernel to use Azure Open AI.
- Optionally adds plugins for extra functionality (commented out initially).
- Retrieves the configured Kernel from the DI container.
- Gets the chat completion service from the kernel.
- Initializes a ChatHistory object with a system prompt that instructs the AI assistant on how to behave (e.g., how to handle emails, keep responses concise, etc.).
- Enters an infinite loop to interact with the user, by prompting for a user request, handling the request and streaming the response back to the console.

## First run - Simple Chatbot
1. Access the Visual Studio Code Command Palette (`Ctrl`+`Shift`+`P` / `Shift`+`Command`+`P`), and start typing "Start without". Click the option "Debug: Start without debugging".

    You can also access this from the menu, by selecting Run > Run without Debugging (or hitting `Ctrl`+`F5` if running VS Code on your desktop rather than in a browser)
2. Observe that the Debug Console will open at the bottom of the window, and eventually you will see a line:
    > Your request: 
3. Start talking with the chatbot.
4. In particular, ask it what the current time in, and see how it can't tell you - because AI doesn't have real time access to a clock. We can fix that with plugins!

## Adding some plugins
You can review the pre-created plugins under the `Plugins` folder.

In particular, if you look at the `AuthorEmailPlugin.cs` file, you will see a function `SendEmailAsync` which has attributes on it for `KernelFunction` and `Description`, and each parameter also has attributes.  These describe the function's purpose to the kernel, so that when automatic function calling is configured, it knows how to use the plugin.

Also take a look at the `MyTimePlugin.cs` file - it contains a function to get the current time.

Let's start using this and some other plugins now.

1. Go to the `Program.cs` file
2. Uncomment lines 46-51, which will be as follows:

    ```
    kernelBuilder.Plugins
        .AddFromType<AuthorEmailPlugin>()
        .AddFromType<PersonalDetailsPlugin>()
        .AddFromType<PublicHolidaysPlugin>()
        .AddFromType<DateHelpers>()
        .AddFromType<MyTimePlugin>();
    ```

3. Uncomment line 95, which will be as follows:

    ```
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    ```

    This tells Semantic Kernel to turns on automatic function calling.

4. Run the program again without debugging, and try asking it again what the time is.

    It should give you a satisfactory answer.

    You could also try asking it how it knew what the time was, and even what function it called - the answer will (hopefully!) reveal that it called the `MyTimePlugin.Time` function.

5.  Now try asking it to send an email, and notice that it will ask you for the details it needs in order to call the `AuthorEmailPlugin.SendEmailAsync` function:

    - recipient
    - subject
    - body

    Experiment with how you provide that information.  You can give it exact details, or you could ask to the create the subject and body for you, based on your inputs.

6. You can now explore the other plugins and play around with using them.

7. Try creating your own plugins, by duplicating the file `MyPluginTemplate.cs` and renaming it to a suitable name, and changing the class and function to whatever you like.

    Don't forget to add the plugin to the kernel in `Program.cs` at around line 47.

8. Semantic Kernel also comes with some [core plugins](https://learn.microsoft.com/en-us/dotnet/api/microsoft.semantickernel.plugins.core?view=semantic-kernel-dotnet&WT.mc_id=MVP_383786) that you can load in for performing various common tasks such as `TimePlugin`.  Try adding that by copying it up to you `kernelBuilder.Plugins` call, from line 55.  This plugin provides a similar feature to what we looked at above in our `MyTimePlugin` class, plus a bunch of extra features.

That's the end of this workshop, but as you can see there is so much you can do now with Semantic Kernel, including using your own code to perform tasks, calling external APIs (e.g. to get the current weather), and who knows, maybe one day it can help you build the next Ironman!!

Thanks, and if you have any questions, issues or feedback, feel free to contact me via [LinkedIn](https://www.linkedin.com/in/bronthulke) or [Bluesky](https://bsky.app/profile/bronthulke.bsky.social). 

## Bonus content

So we just did all of the above, but with a reliance on having an internet connection.

If you want to experiment with using a local SLM/LLM, such as Phi-4, you could do so by installing a tool such as Ollama or LM Studio.

At the moment, although automatic function calling is supposed to work with these tools, my personal experience is that it's not actually there yet.

The process of hooking up your chatbot to a model running in one of those tools is not too complex, though.  Once you have one of them installed, you can replace the following lines in the code (around line 39-43):

```
// With Azure Open AI
var modelId = "gpt-4o";
var apiKey = configuration["AzureOpenAIAPIKey"];
var endpoint = configuration["AzureOpenAIEndpoint"];
kernelBuilder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
```

with one of the following depending on which application you're using, and what your installed model is:

```
// With LM Studio
// string modelId = "phi-3-mini-4k-instruct";
// string apiKey = null;
// string endpoint = "http://localhost:1234/v1";
// kernelBuilder.AddOpenAIChatCompletion(modelId, new Uri(endpoint), apiKey);

// With Ollama
// string modelId = "phi4-mini";
// string endpoint = "http://localhost:11434";
// kernelBuilder.AddOllamaChatCompletion(modelId, new Uri(endpoint));
```

Then **supposedly** for Ollama you could enable automatic function calling by changing the settings from being a `OpenAIPromptExecutionSettings` object, to using the following instead (around line 93):

```
OllamaPromptExecutionSettings settings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
};
```

Your miles may vary, but at this stage the automatic function calling component just doesn't seem to work for me - but it will still act as a helpful chatbot, using your locally running model.

Once again, if you have any questions, issues or feedback - especially if you've managed to get automatic function calling working with either Ollama or LM Studio, feel free to contact me via [LinkedIn](https://www.linkedin.com/in/bronthulke) or [Bluesky](https://bsky.app/profile/bronthulke.bsky.social). 