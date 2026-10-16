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

        public ObservableCollection<CategoryModel> Genres { get; } = new();

        public ObservableCollection<TrackModel> Results { get; } = new();

        [ObservableProperty]
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

        public FilterViewModel(MetadataService metadataService)
        {
            _metadataService = metadataService;
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

        #region Not implemented yet
        [RelayCommand]
        private void ApplyFilter()
        {
            ShowNotImplemented("Filtering by genre");
        }

        [RelayCommand]
        private void SaveAsPlaylist()
        {
            ShowNotImplemented("Saving the filter result as a playlist");
        }
        #endregion
    }
}
