using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Mobile.Models;
using NMC.Mobile.Services;

namespace NMC.Mobile.ViewModel
{
    [QueryProperty(nameof(PlaylistId), "playlistId")]
    public partial class PlaylistDetailViewModel : ObservableObject
    {
        private readonly PlaylistService _playlistService;

        [ObservableProperty]
        private int _playlistId;

        [ObservableProperty]
        private PlaylistDetailModel? _playlist;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isEditingName;

        [ObservableProperty]
        private string _editedPlaylistName = string.Empty;

        [ObservableProperty]
        private bool _isShuffleEnabled;

        public string ShuffleIconSource => IsShuffleEnabled ? "shuffle_on.png" : "shuffle_off.png";

        public PlaylistDetailViewModel(PlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        partial void OnPlaylistIdChanged(int value)
        {
            _ = LoadPlaylistDetailsAsync();
        }

        public async Task LoadPlaylistDetailsAsync()
        {
            if (IsBusy || PlaylistId == 0) return;
            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                Playlist = await _playlistService.GetPlaylistDetailAsync(PlaylistId);
                if (Playlist == null)
                {
                    ErrorMessage = "Could not load playlist details.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void StartEditingName()
        {
            if (Playlist == null) return;
            EditedPlaylistName = Playlist.PlaylistName;
            IsEditingName = true;
        }

        [RelayCommand]
        private async Task SavePlaylistNameAsync()
        {
            if (Playlist == null || string.IsNullOrWhiteSpace(EditedPlaylistName)) return;

            Playlist.PlaylistName = EditedPlaylistName.Trim();
            IsEditingName = false;

            // TODO
        }

        [RelayCommand]
        private void CancelEditingName()
        {
            IsEditingName = false;
        }

        [RelayCommand]
        private void ToggleShuffle()
        {
            IsShuffleEnabled = !IsShuffleEnabled;
            OnPropertyChanged(nameof(ShuffleIconSource));
        }

        [RelayCommand]
        private async Task ChangeCoverAsync()
        {
            // TODO
        }

        [RelayCommand]
        private void PlayPlaylist()
        {
            // TODO
        }

        [RelayCommand]
        private async Task AddTrackToPlaylistAsync()
        {
            // TODO
        }

        [RelayCommand]
        private async Task DeletePlaylistAsync()
        {
            if (Playlist == null) return;

            bool confirm = await Shell.Current.DisplayAlertAsync("Delete Playlist", "Are you sure you want to delete this playlist?", "Yes", "No");
            if (!confirm) return;

            bool success = await _playlistService.DeletePlaylistAsync(Playlist.Id);
            if (success)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ErrorMessage = "Failed to delete playlist.";
            }
        }

        [RelayCommand]
        private async Task RemoveTrackAsync(UserTrackModel track)
        {
            if (Playlist == null || track == null) return;

            bool success = await _playlistService.RemoveTrackFromPlaylistAsync(Playlist.Id, track.Id);
            if (success)
            {
                Playlist.Tracks.Remove(track);
            }
        }
    }
}