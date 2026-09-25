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

        public MainViewModel(PlaylistService playlistService, SessionService sessionService)
        {
            _playlistService = playlistService;
            _sessionService = sessionService;
        }

        public async Task LoadPlaylistsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var userIdStr = await _sessionService.GetUserIdAsync();
                if (!int.TryParse(userIdStr, out int userId))
                {
                    ErrorMessage = "Invalid user session!";
                    return;
                }

                var playlists = await _playlistService.GetPlaylistsAsync(userId);

                Playlists.Clear();
                foreach (var playlist in playlists)
                {
                    Playlists.Add(playlist);
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

        [RelayCommand]
        private async Task SelectPlaylistAsync(PlaylistModel playlist)
        {
            if (playlist == null) return;

            await Shell.Current.GoToAsync($"PlaylistViewPage?playlistId={playlist.Id}");
        }
    }
}