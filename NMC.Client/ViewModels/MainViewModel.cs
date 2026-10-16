using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;
using NMC.Client.Navigation;
using NMC.Client.Services;

namespace NMC.Client.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly PlaylistService _playlistService;
        private readonly SessionService _sessionService;

        public ObservableCollection<PlaylistModel> Playlists { get; } = new();

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private PlaylistModel? selectedPlaylist;

        public MainViewModel(PlaylistService playlistService, SessionService sessionService)
        {
            _playlistService = playlistService;
            _sessionService = sessionService;
        }

        partial void OnSelectedPlaylistChanged(PlaylistModel? value)
        {
            if (value is null)
            {
                return;
            }

            OpenPlaylistCommand.Execute(value);
            SelectedPlaylist = null;
        }

        #region Loading
        [RelayCommand]
        private async Task LoadPlaylists()
        {
            Username = _sessionService.Username;
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

                Playlists.Clear();
                foreach (PlaylistModel playlist in result.Data)
                {
                    Playlists.Add(playlist);
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
        private async Task OpenPlaylist(PlaylistModel playlist)
        {
            var parameters = new ShellNavigationQueryParameters
            {
                { QueryKeys.Playlist, playlist }
            };

            await NavigateAsync(Routes.PlaylistDetail, parameters);
        }

        [RelayCommand]
        private async Task CreatePlaylist()
        {
            await NavigateAsync(Routes.PlaylistEdit);
        }

        [RelayCommand]
        private async Task OpenLibrary()
        {
            await NavigateAsync(Routes.Library);
        }

        [RelayCommand]
        private async Task OpenAddTrack()
        {
            await NavigateAsync(Routes.AddTrack);
        }

        [RelayCommand]
        private async Task OpenFilter()
        {
            await NavigateAsync(Routes.Filter);
        }

        [RelayCommand]
        private async Task OpenRecommendation()
        {
            await NavigateAsync(Routes.Recommendation);
        }

        [RelayCommand]
        private async Task OpenProfile()
        {
            await NavigateAsync(Routes.Profile);
        }
        #endregion
    }
}
