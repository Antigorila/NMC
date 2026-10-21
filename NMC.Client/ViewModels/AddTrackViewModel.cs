using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;
using NMC.Client.Navigation;
using NMC.Client.Services;

namespace NMC.Client.ViewModels
{
    public partial class AddTrackViewModel : BaseViewModel
    {
        private readonly TrackService _trackService;
        private readonly SessionService _sessionService;
        private readonly MediaPickerService _mediaPickerService;
        private readonly AudioMetadataService _audioMetadataService;
        private readonly LocalFileService _localFileService;

        private bool _isUploaded;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasFile))]
        [NotifyCanExecuteChangedFor(nameof(UploadCommand))]
        private string filePath = string.Empty;

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string artist = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CoverSource))]
        private string? coverPath;

        public bool HasFile => !string.IsNullOrWhiteSpace(FilePath);

        public string CoverSource => string.IsNullOrWhiteSpace(CoverPath) ? TrackModel.PlaceholderCover : CoverPath;

        public AddTrackViewModel(TrackService trackService, SessionService sessionService, MediaPickerService mediaPickerService, AudioMetadataService audioMetadataService, LocalFileService localFileService)
        {
            _trackService = trackService;
            _sessionService = sessionService;
            _mediaPickerService = mediaPickerService;
            _audioMetadataService = audioMetadataService;
            _localFileService = localFileService;
        }

        #region Commands
        [RelayCommand]
        private async Task PickAudioFile()
        {
            ErrorMessage = string.Empty;

            FileResult? pickedFile = await _mediaPickerService.PickAudioFileAsync();
            if (pickedFile is null)
            {
                return;
            }

            string? copiedPath = await _localFileService.CopyToAppFolderAsync(pickedFile, LocalFileService.TracksFolder);
            if (copiedPath is null)
            {
                ErrorMessage = "The selected file could not be copied into the app.";
                return;
            }

            DiscardPickedFiles();
            FilePath = copiedPath;

            AudioMetadata metadata = _audioMetadataService.Read(copiedPath);

            if (!string.IsNullOrWhiteSpace(metadata.Title))
            {
                Title = metadata.Title;
            }

            if (!string.IsNullOrWhiteSpace(metadata.Artist))
            {
                Artist = metadata.Artist;
            }

            if (metadata.CoverData != null)
            {
                CoverPath = await _localFileService.SaveToAppFolderAsync(metadata.CoverData, LocalFileService.CoversFolder, metadata.CoverExtension);
            }
        }

        [RelayCommand]
        private async Task PickCover()
        {
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

            _localFileService.DeleteAppFile(CoverPath);
            CoverPath = copiedPath;
        }

        private bool CanUpload()
        {
            return HasFile;
        }

        [RelayCommand(CanExecute = nameof(CanUpload))]
        private async Task Upload()
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                ApiResult<TrackModel> result = await _trackService.UploadTrackAsync(_sessionService.UserId, Title, Artist, FilePath, CoverPath);

                if (!result.IsSuccess || result.Data is null)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                _isUploaded = true;

                var parameters = new ShellNavigationQueryParameters
                {
                    { QueryKeys.Track, result.Data }
                };

                await NavigateAsync($"../{Routes.TrackDetail}", parameters);
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        public void DiscardUnsavedFiles()
        {
            if (_isUploaded)
            {
                return;
            }

            DiscardPickedFiles();
        }

        #region Support Methods
        private void DiscardPickedFiles()
        {
            _localFileService.DeleteAppFile(FilePath);
            _localFileService.DeleteAppFile(CoverPath);

            FilePath = string.Empty;
            CoverPath = null;
        }
        #endregion
    }
}
