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
        private readonly SessionService _sessionService;

        public ObservableCollection<PlaylistModel> ReferencePlaylists { get; } = new();

        public ObservableCollection<TrackModel> Results { get; } = new();

        [ObservableProperty]
        private PlaylistModel? selectedReference;

        [ObservableProperty]
        private TrackModel? selectedResult;

        public RecommendationViewModel(PlaylistService playlistService, SessionService sessionService)
        {
            _playlistService = playlistService;
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

        #region Not implemented yet
        [RelayCommand]
        private void Generate()
        {
            ShowNotImplemented("Generating recommendations");
        }

        [RelayCommand]
        private void SaveAsPlaylist()
        {
            ShowNotImplemented("Saving recommendations as a playlist");
        }
        #endregion
    }
}
