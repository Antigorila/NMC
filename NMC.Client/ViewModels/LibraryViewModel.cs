using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;
using NMC.Client.Navigation;
using NMC.Client.Services;

namespace NMC.Client.ViewModels
{
    public partial class LibraryViewModel : BaseViewModel
    {
        private readonly TrackService _trackService;
        private readonly SessionService _sessionService;

        public ObservableCollection<TrackModel> Tracks { get; } = new();

        [ObservableProperty]
        private TrackModel? selectedTrack;

        public LibraryViewModel(TrackService trackService, SessionService sessionService)
        {
            _trackService = trackService;
            _sessionService = sessionService;
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
        private async Task LoadTracks()
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                ApiResult<List<TrackModel>> result = await _trackService.GetTracksAsync(_sessionService.UserId);

                if (!result.IsSuccess || result.Data is null)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                Tracks.Clear();
                foreach (TrackModel track in result.Data)
                {
                    Tracks.Add(track);
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

        [RelayCommand]
        private async Task OpenAddTrack()
        {
            await NavigateAsync(Routes.AddTrack);
        }
        #endregion
    }
}
