using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;
using NMC.Client.Navigation;
using NMC.Client.Services;

namespace NMC.Client.ViewModels
{
    public partial class RecommendationViewModel : BaseViewModel
    {
        private readonly PlaylistService _playlistService;
        private readonly RecommendationService _recommendationService;
        private readonly SessionService _sessionService;

        private string _lastReferenceName = string.Empty;

        public ObservableCollection<PlaylistModel> ReferencePlaylists { get; } = new();

        public ObservableCollection<TrackModel> Results { get; } = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GenerateCommand))]
        private PlaylistModel? selectedReference;

        [ObservableProperty]
        private TrackModel? selectedResult;

        public RecommendationViewModel(PlaylistService playlistService, RecommendationService recommendationService, SessionService sessionService)
        {
            _playlistService = playlistService;
            _recommendationService = recommendationService;
            _sessionService = sessionService;
        }

        partial void OnSelectedResultChanged(TrackModel? value)
        {
            if (value is null)
            {
                return;
            }

            OpenTrackCommand.Execute(value);
            SelectedResult = null;
        }

        #region Loading
        [RelayCommand]
        private async Task LoadPlaylists()
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                ApiResult<List<PlaylistModel>> result = await _playlistService.GetPlaylistsAsync(_sessionService.UserId);

                if (!result.IsSuccess || result.Data is null)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                ReferencePlaylists.Clear();
                foreach (PlaylistModel playlist in result.Data)
                {
                    ReferencePlaylists.Add(playlist);
                }
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
        #endregion

        #region Commands
        private bool CanGenerate()
        {
            return SelectedReference != null;
        }

        [RelayCommand(CanExecute = nameof(CanGenerate))]
        private async Task Generate()
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                ApiResult<List<TrackModel>> result = await _recommendationService.GenerateAsync(_sessionService.UserId, SelectedReference!.Id);

                if (!result.IsSuccess || result.Data is null)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                Results.Clear();
                foreach (TrackModel track in result.Data)
                {
                    Results.Add(track);
                }

                _lastReferenceName = SelectedReference.Name;
                SaveAsPlaylistCommand.NotifyCanExecuteChanged();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanSaveAsPlaylist()
        {
            return Results.Count > 0;
        }

        [RelayCommand(CanExecute = nameof(CanSaveAsPlaylist))]
        private async Task SaveAsPlaylist()
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                string playlistName = $"Based on {_lastReferenceName}";
                ApiResult<PlaylistModel> result = await _playlistService.CreatePlaylistWithTracksAsync(_sessionService.UserId, playlistName, Results);

                if (!result.IsSuccess || result.Data is null)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                ShowAlert("Playlist saved", $"\"{result.Data.Name}\" was created with {result.Data.TrackCount} tracks.");
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}
