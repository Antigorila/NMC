using Microsoft.Extensions.Logging;
using NMC.Mobile.Services;
using NMC.Mobile.ViewModel;
using NMC.Mobile.Views;

namespace NMC.Mobile
{
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

            string baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000";

            builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });

            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddSingleton<AuthenticationService>();

            //builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<MainViewModel>();

            // --- Service ---
            builder.Services.AddSingleton<MediaService>();
            builder.Services.AddTransient<TrackService>();
            builder.Services.AddTransient<PlaylistService>();

            // --- ViewModel ---
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<AddMusicViewModel>();
            builder.Services.AddTransient<CreatePlaylistViewModel>();
            builder.Services.AddTransient<PlaylistDetailViewModel>();

            // --- Page ---
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<StatsPage>();
            builder.Services.AddTransient<AddMusicPage>();
            builder.Services.AddTransient<CreatePlaylistPage>();
            builder.Services.AddTransient<PlaylistViewPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}