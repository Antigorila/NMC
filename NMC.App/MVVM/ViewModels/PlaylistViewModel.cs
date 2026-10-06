using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.App.MVVM.ViewModels
{
    partial class PlaylistViewModel : ObservableObject
    {
        public Action OnBackToHomeRequested { get; set; }
        [ObservableProperty]
        private ContentView currentPlaylistContent;

        public PlaylistViewModel()
        {
            ShowPlaylistList();
        }

        [RelayCommand]
        public void ShowPlaylistList()
        {
            var listView = new NMC.App.MVVM.Views.ContentViews.Playlist.PlaylistListContentView();
            listView.BindingContext = this;
            CurrentPlaylistContent = listView;
        }

        [RelayCommand]
        public void ShowPlaylistAdd()
        {
            var view = new NMC.App.MVVM.Views.ContentViews.Playlist.PlaylistAddContentView();
            view.BindingContext = this;
            CurrentPlaylistContent = view;
        }

        [RelayCommand]
        public void ShowPlaylistEdit()
        {
            var view = new NMC.App.MVVM.Views.ContentViews.Playlist.PlaylistEditContentView();
            view.BindingContext = this;
            CurrentPlaylistContent = view;
        }

        [RelayCommand]
        private void GoBackToHome()
        {
            OnBackToHomeRequested?.Invoke();
        }
    }
}
