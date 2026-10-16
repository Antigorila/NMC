using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;
using NMC.Client.Navigation;
using NMC.Client.Services;

namespace NMC.Client.ViewModels
{
    [QueryProperty(nameof(Playlist), QueryKeys.Playlist)]
    public partial class PlaylistEditViewModel : BaseViewModel
    {
        private readonly PlaylistService _playlistService;
        private readonly SessionService _sessionService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEditMode))]
        [NotifyPropertyChangedFor(nameof(PageTitle))]
        private PlaylistModel? playlist;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CoverSource))]
        private string? coverPath;

        public bool IsEditMode => Playlist != null;

        public string PageTitle => IsEditMode ? "Edit playlist" : "New playlist";

        public string CoverSource => string.IsNullOrWhiteSpace(CoverPath) ? TrackModel.PlaceholderCover : CoverPath;

        public PlaylistEditViewModel(PlaylistService playlistService, SessionService sessionService)
        {
            _playlistService = playlistService;
            _sessionService = sessionService;
        }

        partial void OnPlaylistChanged(PlaylistModel? value)
        {
            if (value is null)
            {
                Name = string.Empty;
                CoverPath = null;
                return;
            }

            Name = value.Name;
            CoverPath = value.CoverPath;
        }

        #region Commands
        [RelayCommand]
        private async Task Save()
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                ApiResult<PlaylistModel> result;

                if (Playlist is null)
                {
                    result = await _playlistService.CreatePlaylistAsync(_sessionService.UserId, Name, CoverPath);
                }
                else
                {
                    result = await _playlistService.UpdatePlaylistAsync(Playlist.Id, Name, CoverPath);
                }

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
        private void PickCover()
        {
            ShowNotImplemented("Picking a playlist cover");
        }
        #endregion
    }
}
