using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;
using NMC.Client.Navigation;
using NMC.Client.Services;

namespace NMC.Client.ViewModels
{
    public partial class FilterViewModel : BaseViewModel
    {
        private readonly MetadataService _metadataService;
        private readonly TrackService _trackService;
        private readonly PlaylistService _playlistService;
        private readonly SessionService _sessionService;

        private string _lastFilterName = string.Empty;

        public ObservableCollection<CategoryModel> Genres { get; } = new();

        public ObservableCollection<TrackModel> Results { get; } = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyFilterCommand))]
        private CategoryModel? selectedGenre;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSoftFilter))]
        [NotifyPropertyChangedFor(nameof(ModeDescription))]
        private bool isHardFilter;

        [ObservableProperty]
        private double threshold = 0.10;

        [ObservableProperty]
        private TrackModel? selectedResult;

        public bool IsSoftFilter => !IsHardFilter;

        public string ModeDescription
        {
            get
            {
                if (IsHardFilter)
                {
                    return "Hard filter: only tracks whose top genre is the selected one.";
                }

                return "Soft filter: every track whose score for the genre is above the threshold.";
            }
        }

        public FilterViewModel(MetadataService metadataService, TrackService trackService, PlaylistService playlistService, SessionService sessionService)
        {
            _metadataService = metadataService;
            _trackService = trackService;
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
        private async Task LoadGenres()
        {
            if (Genres.Count > 0)
            {
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                ApiResult<List<CategoryModel>> result = await _metadataService.GetGenresAsync();

                if (!result.IsSuccess || result.Data is null)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                foreach (CategoryModel genre in result.Data)
                {
                    Genres.Add(genre);
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
        private bool CanApplyFilter()
        {
            return SelectedGenre != null;
        }

        [RelayCommand(CanExecute = nameof(CanApplyFilter))]
        private async Task ApplyFilter()
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                ApiResult<List<TrackModel>> result = await _trackService.FilterTracksAsync(_sessionService.UserId, SelectedGenre!.Id, IsHardFilter, Threshold);

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

                _lastFilterName = BuildFilterName();
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
                ApiResult<PlaylistModel> result = await _playlistService.CreatePlaylistWithTracksAsync(_sessionService.UserId, _lastFilterName, Results);

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

        #region Support Methods
        private string BuildFilterName()
        {
            string genreName = SelectedGenre?.Name ?? "Filter";

            if (IsHardFilter)
            {
                return $"{genreName} (hard filter)";
            }

            return $"{genreName} (soft filter, {Threshold:P0})";
        }
        #endregion
    }
}
