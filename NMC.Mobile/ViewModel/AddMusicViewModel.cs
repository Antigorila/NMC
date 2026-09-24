using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Mobile.Models;
using NMC.Mobile.Services;

namespace NMC.Mobile.ViewModel
{
    public partial class AddMusicViewModel : ObservableObject
    {
        private readonly TrackService _trackService;
        private readonly SessionService _sessionService;
        private readonly MediaService _mediaService;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _artist = string.Empty;

        [ObservableProperty]
        private string _filePath = string.Empty;

        [ObservableProperty]
        private string? _coverPath;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public AddMusicViewModel(TrackService trackService, SessionService sessionService, MediaService mediaService)
        {
            _trackService = trackService;
            _sessionService = sessionService;
            _mediaService = mediaService;
        }

        [RelayCommand]
        private async Task SelectAudioAsync()
        {
            ErrorMessage = string.Empty;
            var metadata = await _mediaService.PickAudioFileAsync();

            if (metadata != null)
            {
                FilePath = metadata.AudioFilePath;

                if (!string.IsNullOrWhiteSpace(metadata.Title)) Title = metadata.Title;
                if (!string.IsNullOrWhiteSpace(metadata.Artist)) Artist = metadata.Artist;
                if (!string.IsNullOrWhiteSpace(metadata.ExtractedCoverPath)) CoverPath = metadata.ExtractedCoverPath;
            }
        }

        [RelayCommand]
        private async Task SelectCoverAsync()
        {
            var path = await _mediaService.PickCoverImageAsync();
            if (!string.IsNullOrEmpty(path))
            {
                CoverPath = path;
            }
        }

        [RelayCommand]
        private async Task UploadTrackAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(FilePath))
            {
                ErrorMessage = "Choose an MP3 file!";
                return;
            }

            var trimmedTitle = Title.Trim();
            var trimmedArtist = Artist.Trim();

            if (string.IsNullOrWhiteSpace(trimmedTitle) || string.IsNullOrWhiteSpace(trimmedArtist))
            {
                ErrorMessage = "Title and Artist fields cannot be empty!";
                return;
            }

            IsBusy = true;

            try
            {
                var userIdStr = await _sessionService.GetUserIdAsync();
                if (!int.TryParse(userIdStr, out int userId))
                {
                    ErrorMessage = "Invalid user session!";
                    return;
                }

                var request = new CreateUserTrackRequest(userId, trimmedTitle, trimmedArtist, FilePath,CoverPath);

                var (success, serverError) = await _trackService.UploadTrackAsync(request);

                if (success)
                {
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    ErrorMessage = serverError;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}