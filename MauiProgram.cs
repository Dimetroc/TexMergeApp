using Microsoft.Maui.LifecycleEvents;
using MudBlazor.Services;
using TexMerge.Core;
#if WINDOWS
using TexMergeApp.Services;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif


namespace TexMergeApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices();
        builder.Services.AddSingleton<ITextureMerger, TextureMerger>();
#if WINDOWS
        builder.Services.AddSingleton<IFolderPicker, Platforms.Windows.FolderPickerService>();

        var width = 1184;
        var height = 740;

        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(windows =>
            {
                windows.OnWindowCreated(window =>
                {
                    var mauiWinUIWindow = (MauiWinUIWindow)window;

                    var nativeWindow = mauiWinUIWindow.WindowHandle;
                    var windowId = Win32Interop.GetWindowIdFromWindow(nativeWindow);
                    var appWindow = AppWindow.GetFromWindowId(windowId);

                    if (appWindow is not null)
                    {
                        // Resize to desired size
                        appWindow.Resize(new SizeInt32(width, height));

                        if (appWindow.Presenter is OverlappedPresenter presenter)
                        {
                            presenter.IsResizable = false;
                            presenter.IsMaximizable = false;
                        }

                        // Optional: Center on screen
                        var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
                        var x = displayArea.WorkArea.X + (displayArea.WorkArea.Width - width) / 2;
                        var y = displayArea.WorkArea.Y + (displayArea.WorkArea.Height - height) / 2;

                        appWindow.Move(new PointInt32(x, y));
                    }
                });
            });
        });
#endif

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
