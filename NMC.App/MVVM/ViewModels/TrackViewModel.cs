using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.App.MVVM.ViewModels
{
    partial class TrackViewModel : ObservableObject
    {
        public Action OnBackToHomeRequested { get; set; }
        [ObservableProperty]
        private ContentView currentTrackContent;

        public TrackViewModel()
        {
            ShowTrackList();
        }

        [RelayCommand]
        public void ShowTrackList()
        {
            var listView = new NMC.App.MVVM.Views.ContentViews.Track.TrackListContentView();
            listView.BindingContext = this;
            CurrentTrackContent = listView;
        }

        [RelayCommand]
        public void ShowAddTrack()
        {
            var view = new NMC.App.MVVM.Views.ContentViews.Track.TrackAddContentView();
            view.BindingContext = this;
            CurrentTrackContent = view;
        }

        [RelayCommand]
        private void GoBackToHome()
        {
            OnBackToHomeRequested?.Invoke();
        }
    }
}
