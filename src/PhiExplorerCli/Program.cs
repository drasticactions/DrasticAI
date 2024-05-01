// See https://aka.ms/new-console-template for more information

using System.ComponentModel.Design;
using Drastic.Services;
using DrasticAI.Services;
using Microsoft.Extensions.DependencyInjection;
using PhiExplorerCli;

Console.WriteLine("PhiExplorerCli");

var serviceProviderBuilder = new ServiceCollection();
serviceProviderBuilder.AddSingleton<IAppDispatcher>(new ConsoleAppDispatcher());
serviceProviderBuilder.AddSingleton<IErrorHandlerService>(new DebugErrorHandler());
serviceProviderBuilder.AddSingleton<PhiModelService>();
serviceProviderBuilder.AddSingleton<PhiService>();
var serviceProvider = serviceProviderBuilder.BuildServiceProvider();

var modelService = serviceProvider.GetRequiredService<PhiModelService>();
if (modelService.AvailableModels.Count == 0)
{
    Console.WriteLine("No models found");
    return;
}

var model = modelService.AvailableModels[0];
modelService.SelectedModel = model;

var phiService = serviceProvider.GetRequiredService<PhiService>();
await phiService.LoadModelAsync();

var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) => { cancellationTokenSource.Cancel(); e.Cancel = true; };
// "You are a translator that will translate the following dialog into Japanese. You will translate the text as natural as you can. Match the tone of the given sentence. You will only translate the text and not generate any additional text. Do not add any additional information, context, or notes. Do not generate more text after the translation."
Console.WriteLine("Enter Prompt System message:");
var promptEntry = Console.ReadLine();
while (!cancellationTokenSource.IsCancellationRequested)
{
    Console.Write("User: ");
    var prompt = Console.ReadLine() ?? string.Empty;
    if (string.IsNullOrEmpty(prompt))
    {
        return;
    }


    var realText = $"{promptEntry}{Environment.NewLine}<|user|>{prompt}{Environment.NewLine}<|assistant|>";

    Console.Write("AI: ");
    await phiService.PromptAsync(realText, s => { Console.Write(s); }, cancellationTokenSource.Token);
    Console.WriteLine();
}

Console.WriteLine();
Console.WriteLine("Done");

