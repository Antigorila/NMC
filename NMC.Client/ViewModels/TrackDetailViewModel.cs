using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;
using NMC.Client.Navigation;
using NMC.Client.Services;

namespace NMC.Client.ViewModels
{
    [QueryProperty(nameof(Track), QueryKeys.Track)]
    public partial class TrackDetailViewModel : BaseViewModel
    {
        private readonly PlaylistService _playlistService;
        private readonly TrackService _trackService;
        private readonly SessionService _sessionService;
        private readonly LocalFileService _localFileService;

        public ObservableCollection<PlaylistModel> Playlists { get; } = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddToPlaylistCommand))]
        private TrackModel? track;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddToPlaylistCommand))]
        private PlaylistModel? selectedPlaylist;

        public TrackDetailViewModel(PlaylistService playlistService, TrackService trackService, SessionService sessionService, LocalFileService localFileService)
        {
            _playlistService = playlistService;
            _trackService = trackService;
            _sessionService = sessionService;
            _localFileService = localFileService;
        }

        #region Loading
        [RelayCommand]
        private async Task LoadPlaylists()
        {
            ApiResult<List<PlaylistModel>> result = await _playlistService.GetPlaylistsAsync(_sessionService.UserId);

            if (!result.IsSuccess || result.Data is null)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }

            Playlists.Clear();
            foreach (PlaylistModel playlist in result.Data)
            {
                Playlists.Add(playlist);
            }
        }
        #endregion

        #region Commands
        private bool CanAddToPlaylist()
        {
            return Track != null && SelectedPlaylist != null;
        }

        [RelayCommand(CanExecute = nameof(CanAddToPlaylist))]
        private async Task AddToPlaylist()
        {
            ErrorMessage = string.Empty;

            ApiResult result = await _playlistService.AddTrackAsync(SelectedPlaylist!.Id, Track!.UserTrackId);

            if (!result.IsSuccess)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }

            ShowAlert("Added", $"\"{Track.Title}\" was added to \"{SelectedPlaylist.Name}\".");
            SelectedPlaylist = null;
        }

        [RelayCommand]
        private async Task DeleteTrack()
        {
            if (Track is null)
            {
                return;
            }

            bool confirmed = await ConfirmAsync("Delete track", $"Do you really want to remove \"{Track.Title}\" from your library?");
            if (!confirmed)
            {
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                ApiResult result = await _trackService.DeleteTrackAsync(Track.UserTrackId);

                if (!result.IsSuccess)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                _localFileService.DeleteAppFile(Track.FilePath);
                _localFileService.DeleteAppFile(Track.CoverPath);

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
        private void Play()
        {
            ShowNotImplemented("Playback");
        }

        [RelayCommand]
        private void ChangeCover()
        {
            ShowNotImplemented("Changing the cover");
        }

        [RelayCommand]
        private void Share()
        {
            ShowNotImplemented("Sharing the analysis");
        }
        #endregion
    }
}
