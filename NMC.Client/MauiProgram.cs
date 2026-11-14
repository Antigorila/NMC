using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using NMC.Client.Services;
using NMC.Client.ViewModels;
using NMC.Client.Views;

namespace NMC.Client
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

            RegisterServices(builder.Services);
            RegisterViewModels(builder.Services);
            RegisterPages(builder.Services);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        #region Registrations
        private static void RegisterServices(IServiceCollection services)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.BaseAddress),
                Timeout = TimeSpan.FromSeconds(ApiConfig.TimeoutSeconds)
            };

            services.AddSingleton(httpClient);
            services.AddSingleton<SessionService>();
            services.AddSingleton<IAudioManager>(AudioManager.Current);
            services.AddSingleton<AudioPlayerService>();
            services.AddSingleton<LocalFileService>();
            services.AddSingleton<DemoDataService>();
            services.AddSingleton<MediaPickerService>();
            services.AddSingleton<AudioMetadataService>();
            services.AddSingleton<AuthService>();
            services.AddSingleton<TrackService>();
            services.AddSingleton<PlaylistService>();
            services.AddSingleton<MetadataService>();
            services.AddSingleton<RecommendationService>();
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddSingleton<PlayerViewModel>();

            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<LibraryViewModel>();
            services.AddTransient<TrackDetailViewModel>();
            services.AddTransient<AddTrackViewModel>();
            services.AddTransient<PlaylistDetailViewModel>();
            services.AddTransient<PlaylistEditViewModel>();
            services.AddTransient<FilterViewModel>();
            services.AddTransient<RecommendationViewModel>();
            services.AddTransient<ProfileViewModel>();
            services.AddTransient<SelectTracksViewModel>();
        }

        private static void RegisterPages(IServiceCollection services)
        {
            services.AddSingleton<AppShell>();

            services.AddTransient<LoginPage>();
            services.AddTransient<RegisterPage>();
            services.AddTransient<MainPage>();
            services.AddTransient<LibraryPage>();
            services.AddTransient<TrackDetailPage>();
            services.AddTransient<AddTrackPage>();
            services.AddTransient<PlaylistDetailPage>();
            services.AddTransient<PlaylistEditPage>();
            services.AddTransient<FilterPage>();
            services.AddTransient<RecommendationPage>();
            services.AddTransient<ProfilePage>();
            services.AddTransient<NowPlayingPage>();
            services.AddTransient<SelectTracksPage>();
        }
        #endregion
    }
}
