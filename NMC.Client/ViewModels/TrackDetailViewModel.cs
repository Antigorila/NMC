using System.Collections.ObjectModel;
using System.Text;
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
        private readonly MediaPickerService _mediaPickerService;
        private readonly PlayerViewModel _playerViewModel;

        public ObservableCollection<PlaylistModel> Playlists { get; } = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddToPlaylistCommand))]
        private TrackModel? track;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddToPlaylistCommand))]
        private PlaylistModel? selectedPlaylist;

        public TrackDetailViewModel(PlaylistService playlistService, TrackService trackService, SessionService sessionService, LocalFileService localFileService, MediaPickerService mediaPickerService, PlayerViewModel playerViewModel)
        {
            _playlistService = playlistService;
            _trackService = trackService;
            _sessionService = sessionService;
            _localFileService = localFileService;
            _mediaPickerService = mediaPickerService;
            _playerViewModel = playerViewModel;
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

        [RelayCommand]
        private void Play()
        {
            if (Track is null)
            {
                return;
            }

            _playerViewModel.PlayTrack(Track);
        }

        [RelayCommand]
        private async Task ChangeCover()
        {
            if (Track is null)
            {
                return;
            }

            ErrorMessage = string.Empty;

            FileResult? pickedImage = await _mediaPickerService.PickImageAsync();
            if (pickedImage is null)
            {
                return;
            }

            string? copiedPath = await _localFileService.CopyToAppFolderAsync(pickedImage, LocalFileService.CoversFolder);
            if (copiedPath is null)
            {
                ErrorMessage = "The selected image could not be copied into the app.";
                return;
            }

            ApiResult result = await _trackService.UpdateCoverAsync(Track.UserTrackId, copiedPath);

            if (!result.IsSuccess)
            {
                _localFileService.DeleteAppFile(copiedPath);
                ErrorMessage = result.ErrorMessage;
                return;
            }

            _localFileService.DeleteAppFile(Track.CoverPath);
            Track.CoverPath = copiedPath;
        }
        #endregion

        #region Sharing
        [RelayCommand]
        private async Task ShareAnalysis()
        {
            if (Track is null)
            {
                return;
            }

            if (!HasInternetAccess())
            {
                return;
            }

            try
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Title = "Share track",
                    Text = CreateShareText(Track)
                });
            }
            catch (Exception ex)
            {
                ShowAlert("Sharing failed", ex.Message);
            }
        }

        [RelayCommand]
        private async Task ShareAudioFile()
        {
            if (Track is null)
            {
                return;
            }

            if (Track.IsFileMissing)
            {
                ShowAlert("File missing", "The audio file is not on this device.");
                return;
            }

            if (!HasInternetAccess())
            {
                return;
            }

            try
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = Track.DisplayName,
                    File = new ShareFile(Track.FilePath)
                });
            }
            catch (Exception ex)
            {
                ShowAlert("Sharing failed", ex.Message);
            }
        }

        private static string CreateShareText(TrackModel track)
        {
            var text = new StringBuilder();
            text.AppendLine(track.DisplayName);

            if (track.GenreScores.Count > 0)
            {
                text.AppendLine();
                text.AppendLine("Genres:");
                foreach (ScoreModel score in track.GenreScores)
                {
                    text.AppendLine($"- {score.Name}: {score.Percent}%");
                }
            }

            if (track.AcousticScores.Count > 0)
            {
                text.AppendLine();
                text.AppendLine("Acoustic attributes:");
                foreach (ScoreModel score in track.AcousticScores)
                {
                    text.AppendLine($"- {score.Name}: {score.Percent}%");
                }
            }

            text.AppendLine();
            text.AppendLine("Analysed by NMC");
            return text.ToString();
        }
        #endregion
    }
}
