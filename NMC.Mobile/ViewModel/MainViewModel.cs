using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Mobile.Helper;
using NMC.Mobile.Models;
using NMC.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NMC.Mobile.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly PlaylistService _playlistService;
        private readonly SessionService _sessionService;

        public ObservableCollection<PlaylistModel> Playlists { get; set; } = new();
        public AcousticGraphDrawable GraphDrawable { get; } = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private PlaylistModel? _selectedPlaylist;

        public MainViewModel(PlaylistService playlistService, SessionService sessionService)
        {
            _playlistService = playlistService;
            _sessionService = sessionService;
        }

        partial void OnSelectedPlaylistChanged(PlaylistModel? value)
        {
            if (value != null)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync($"PlaylistViewPage?playlistId={value.Id}");
                    SelectedPlaylist = null;
                });
            }
        }

        public async Task LoadPlaylistsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var userIdStr = await _sessionService.GetUserIdAsync();
                if (!int.TryParse(userIdStr, out int userId) || userId <= 0)
                {
                    return;
                }

                var playlists = await _playlistService.GetPlaylistsAsync(userId);

                Playlists.Clear();
                if (playlists != null)
                {
                    foreach (var playlist in playlists)
                    {
                        Playlists.Add(playlist);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load playlists: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}