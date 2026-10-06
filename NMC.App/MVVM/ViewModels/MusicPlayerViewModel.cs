using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.App.MVVM.ViewModels
{
    public partial class MusicPlayerViewModel : ObservableObject
    {
        [ObservableProperty]
        private string currentSongTitle = "Selected Title";
        [ObservableProperty]
        private string playPauseButtonText = "Play";
        [ObservableProperty]
        private View currentMainContent;

        public MusicPlayerViewModel()
        {
            ShowHomeContent();
        }

        [RelayCommand]
        public void ShowHomeContent()
        {
            var homeView = new NMC.App.MVVM.Views.HomeContentView();

            homeView.BindingContext = this;
            CurrentMainContent = homeView;
        }

        [RelayCommand]
        public void GoToProfile()
        {
            var view = new NMC.App.MVVM.Views.ContentViews.Profile.ProfileContentView();
            view.OnBackRequested += (s, e) => ShowHomeContent();
            CurrentMainContent = view;
        }

        [RelayCommand]
        public void GoToPlaylists()
        {
            var view = new NMC.App.MVVM.Views.ContentViews.Playlist.PlaylistContentView();
            view.OnBackRequested += (s, e) => ShowHomeContent();
            CurrentMainContent = view;
        }

        [RelayCommand]
        public void GoToTracks()
        {
            var trackView = new NMC.App.MVVM.Views.ContentViews.Track.TrackContentView();
            var trackViewModel = new TrackViewModel();

            trackViewModel.OnBackToHomeRequested = () => ShowHomeContent();
            trackView.BindingContext = trackViewModel;
            CurrentMainContent = trackView;
        }

        [RelayCommand]
        private void TogglePlay()
        {
            PlayPauseButtonText = PlayPauseButtonText == "Play" ? "Pause" : "Play";
        }

        [RelayCommand]
        private async Task OpenFullPlayer()
        {
            await Shell.Current.DisplayAlertAsync("Player", "Opening full screen...", "OK");
        }
    }
}
