using Drastic.Services;
using DrasticAI.Services;
using Microsoft.Extensions.Logging;
using PhiExplorer.ViewModels;

namespace PhiExplorer;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseVirtualListView()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddSingleton<IAppDispatcher, MauiAppDispatcher>();
        builder.Services.AddSingleton<IErrorHandlerService, DebugErrorHandler>();
        builder.Services.AddSingleton<PhiModelService>();
        builder.Services.AddSingleton<PhiModelDownloadViewModel>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}