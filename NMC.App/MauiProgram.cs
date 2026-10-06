using Microsoft.Extensions.Logging;
using NMC.App.MVVM.ViewModels;
using NMC.App.MVVM.Views;

namespace NMC.App;

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
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#region ViewModels
        builder.Services.AddSingleton<MusicPlayerViewModel>();
		builder.Services.AddTransient<UserViewModel>();
#endregion

        #region Views
        builder.Services.AddTransient<RegisterView>();
		builder.Services.AddTransient<LoginView>();
#endregion

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
