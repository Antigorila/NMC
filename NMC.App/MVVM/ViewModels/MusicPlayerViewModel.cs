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
            var profileView = new NMC.App.MVVM.Views.ProfileView();

            profileView.OnBackRequested += (s, e) => ShowHomeContent();

            CurrentMainContent = profileView;
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
