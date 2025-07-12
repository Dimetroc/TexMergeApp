using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using TexMerge.Core;
#if WINDOWS
using TexMergeApp.Services;
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
#endif

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
