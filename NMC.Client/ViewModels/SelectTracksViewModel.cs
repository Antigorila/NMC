using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;
using NMC.Client.Navigation;
using NMC.Client.Services;

namespace NMC.Client.ViewModels
{
    [QueryProperty(nameof(Playlist), QueryKeys.Playlist)]
    public partial class SelectTracksViewModel : BaseViewModel
    {
        private readonly TrackService _trackService;
        private readonly PlaylistService _playlistService;
        private readonly SessionService _sessionService;

        public ObservableCollection<TrackModel> AvailableTracks { get; } = new();

        public ObservableCollection<object> SelectedTracks { get; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PageTitle))]
        private PlaylistModel? playlist;

        public string PageTitle => Playlist is null ? "Add tracks" : $"Add to {Playlist.Name}";

        public string SelectionText => $"{SelectedTracks.Count} selected";

        public SelectTracksViewModel(TrackService trackService, PlaylistService playlistService, SessionService sessionService)
        {
            _trackService = trackService;
            _playlistService = playlistService;
            _sessionService = sessionService;

            SelectedTracks.CollectionChanged += OnSelectedTracksChanged;
        }

        #region Loading
        [RelayCommand]
        private async Task LoadTracks()
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
                ApiResult<List<TrackModel>> result = await _trackService.GetTracksAsync(_sessionService.UserId);

                if (!result.IsSuccess || result.Data is null)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                var tracksInPlaylist = new HashSet<int>();
                foreach (TrackModel track in Playlist.Tracks)
                {
                    tracksInPlaylist.Add(track.UserTrackId);
                }

                SelectedTracks.Clear();
                AvailableTracks.Clear();
                foreach (TrackModel track in result.Data)
                {
                    if (!tracksInPlaylist.Contains(track.UserTrackId))
                    {
                        AvailableTracks.Add(track);
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        #region Commands
        private bool CanAddSelected()
        {
            return Playlist != null && SelectedTracks.Count > 0;
        }

        [RelayCommand(CanExecute = nameof(CanAddSelected))]
        private async Task AddSelected()
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                var tracksToAdd = new List<TrackModel>();
                foreach (object selected in SelectedTracks)
                {
                    if (selected is TrackModel track)
                    {
                        tracksToAdd.Add(track);
                    }
                }

                ApiResult<int> result = await _playlistService.AddTracksAsync(Playlist!.Id, tracksToAdd);

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

        #region Support Methods
        private void OnSelectedTracksChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectionText));
            AddSelectedCommand.NotifyCanExecuteChanged();
        }
        #endregion
    }
}
