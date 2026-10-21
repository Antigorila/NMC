using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;
using NMC.Client.Navigation;
using NMC.Client.Services;

namespace NMC.Client.ViewModels
{
    [QueryProperty(nameof(Playlist), QueryKeys.Playlist)]
    public partial class PlaylistDetailViewModel : BaseViewModel
    {
        private readonly PlaylistService _playlistService;
        private readonly PlayerViewModel _playerViewModel;

        [ObservableProperty]
        private PlaylistModel? playlist;

        [ObservableProperty]
        private TrackModel? selectedTrack;

        public PlaylistDetailViewModel(PlaylistService playlistService, PlayerViewModel playerViewModel)
        {
            _playlistService = playlistService;
            _playerViewModel = playerViewModel;
        }

        partial void OnSelectedTrackChanged(TrackModel? value)
        {
            if (value is null)
            {
                return;
            }

            OpenTrackCommand.Execute(value);
            SelectedTrack = null;
        }

        #region Loading
        [RelayCommand]
        private async Task LoadPlaylist()
        {
            if (Playlist is null)
            {
                ErrorMessage = "No playlist was selected.";
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                ApiResult<PlaylistModel> result = await _playlistService.GetPlaylistAsync(Playlist.Id);

                if (!result.IsSuccess || result.Data is null)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                Playlist = result.Data;
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        #region Navigation
        [RelayCommand]
        private async Task OpenTrack(TrackModel track)
        {
            var parameters = new ShellNavigationQueryParameters
            {
                { QueryKeys.Track, track }
            };

            await NavigateAsync(Routes.TrackDetail, parameters);
        }

        [RelayCommand]
        private async Task EditPlaylist()
        {
            if (Playlist is null)
            {
                return;
            }

            var parameters = new ShellNavigationQueryParameters
            {
                { QueryKeys.Playlist, Playlist }
            };

            await NavigateAsync(Routes.PlaylistEdit, parameters);
        }
        #endregion

        #region Commands
        [RelayCommand]
        private void PlayAll()
        {
            if (Playlist is null || Playlist.Tracks.Count == 0)
            {
                ErrorMessage = "This playlist has no tracks to play.";
                return;
            }

            ErrorMessage = string.Empty;
            _playerViewModel.PlayTracks(Playlist.Tracks);
        }

        [RelayCommand]
        private async Task RemoveTrack(TrackModel track)
        {
            if (Playlist is null)
            {
                return;
            }

            ErrorMessage = string.Empty;

            ApiResult result = await _playlistService.RemoveTrackAsync(Playlist.Id, track.UserTrackId);

            if (!result.IsSuccess)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }

            Playlist.Tracks.Remove(track);
            Playlist.TrackCount = Playlist.Tracks.Count;
        }

        [RelayCommand]
        private async Task DeletePlaylist()
        {
            if (Playlist is null)
            {
                return;
            }

            bool confirmed = await ConfirmAsync("Delete playlist", $"Do you really want to delete \"{Playlist.Name}\"?");
            if (!confirmed)
            {
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                ApiResult result = await _playlistService.DeletePlaylistAsync(Playlist.Id);

                if (!result.IsSuccess)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                await NavigateAsync("..");
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        #region Not implemented yet
        [RelayCommand]
        private void AddTracks()
        {
            ShowNotImplemented("Adding several tracks to a playlist");
        }
        #endregion
    }
}
