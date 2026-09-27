using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NMC.Mobile.Helper;
using NMC.Mobile.Models;
using NMC.Mobile.Services;
using System.Collections.ObjectModel;

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


        private bool _hasLoadedOnce = false;

        public MainViewModel(PlaylistService playlistService, SessionService sessionService)
        {
            _playlistService = playlistService;
            _sessionService = sessionService;

            WeakReferenceMessenger.Default.Register<PlaylistCreatedMessage>(this, (recipient, message) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (message.Value != null)
                    {
                        Playlists.Add(message.Value);
                    }
                });
            });
        }

        public async Task LoadPlaylistsOnceAsync()
        {
            if (_hasLoadedOnce && Playlists.Count > 0) return;
            if (IsBusy) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var userIdStr = await _sessionService.GetUserIdAsync();
                if (string.IsNullOrWhiteSpace(userIdStr) || !int.TryParse(userIdStr, out int userId))
                {
                    ErrorMessage = "Invalid user session!";
                    return;
                }

                var playlists = await _playlistService.GetPlaylistsAsync(userId) ?? new List<PlaylistModel>();
                var validPlaylists = playlists.Where(p => p != null).ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Playlists.Clear();
                    foreach (var playlist in validPlaylists)
                    {
                        Playlists.Add(playlist);
                    }
                });

                _hasLoadedOnce = true;
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

        partial void OnSelectedPlaylistChanged(PlaylistModel? value)
        {
            if (value != null)
            {
                SelectPlaylistCommand.Execute(value);
                SelectedPlaylist = null;
            }
        }
    }
}