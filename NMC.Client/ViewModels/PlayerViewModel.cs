using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;
using NMC.Client.Navigation;

namespace NMC.Client.ViewModels
{
    public partial class PlayerViewModel : BaseViewModel
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasTrack))]
        [NotifyPropertyChangedFor(nameof(NowPlayingText))]
        [NotifyPropertyChangedFor(nameof(CoverSource))]
        private TrackModel? currentTrack;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlayPauseIcon))]
        private bool isPlaying;

        public bool HasTrack => CurrentTrack != null;

        public string NowPlayingText => CurrentTrack?.DisplayName ?? "Nothing is playing";

        public string PlayPauseIcon => IsPlaying ? "pause.png" : "play.png";

        public string CoverSource => CurrentTrack?.CoverSource ?? TrackModel.PlaceholderCover;

        #region Navigation
        [RelayCommand]
        private async Task OpenNowPlaying()
        {
            await NavigateAsync(Routes.NowPlaying);
        }
        #endregion

        #region Not implemented yet
        [RelayCommand]
        private void TogglePlay()
        {
            ShowNotImplemented("Playback");
        }

        [RelayCommand]
        private void Next()
        {
            ShowNotImplemented("Skipping to the next track");
        }

        [RelayCommand]
        private void Previous()
        {
            ShowNotImplemented("Going back to the previous track");
        }
        #endregion
    }
}
