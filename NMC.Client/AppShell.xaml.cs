using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Dispatching;
using NMC.Client.Messages;
using NMC.Client.Navigation;
using NMC.Client.Views;

namespace NMC.Client
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();

            WeakReferenceMessenger.Default.Register<AlertMessage>(this, OnAlertMessageReceived);
            WeakReferenceMessenger.Default.Register<ConfirmMessage>(this, OnConfirmMessageReceived);
        }

        #region Support Methods
        private void RegisterRoutes()
        {
            Routing.RegisterRoute(Routes.Register, typeof(RegisterPage));
            Routing.RegisterRoute(Routes.Library, typeof(LibraryPage));
            Routing.RegisterRoute(Routes.TrackDetail, typeof(TrackDetailPage));
            Routing.RegisterRoute(Routes.AddTrack, typeof(AddTrackPage));
            Routing.RegisterRoute(Routes.PlaylistDetail, typeof(PlaylistDetailPage));
            Routing.RegisterRoute(Routes.PlaylistEdit, typeof(PlaylistEditPage));
            Routing.RegisterRoute(Routes.Filter, typeof(FilterPage));
            Routing.RegisterRoute(Routes.Recommendation, typeof(RecommendationPage));
            Routing.RegisterRoute(Routes.Profile, typeof(ProfilePage));
            Routing.RegisterRoute(Routes.NowPlaying, typeof(NowPlayingPage));
            Routing.RegisterRoute(Routes.SelectTracks, typeof(SelectTracksPage));
        }

        private async void OnAlertMessageReceived(object recipient, AlertMessage message)
        {
            await Dispatcher.DispatchAsync(async () =>
            {
                await DisplayAlertAsync(message.Title, message.Message, "OK");
            });
        }

        private void OnConfirmMessageReceived(object recipient, ConfirmMessage message)
        {
            message.Reply(ShowConfirmAsync(message));
        }

        private async Task<bool> ShowConfirmAsync(ConfirmMessage message)
        {
            return await Dispatcher.DispatchAsync(() => DisplayAlertAsync(message.Title, message.Message, "Yes", "No"));
        }
        #endregion
    }
}
