using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;

namespace NMC.Client.ViewModels
{
    public partial class AddTrackViewModel : BaseViewModel
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasFile))]
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

        #region Not implemented yet
        [RelayCommand]
        private void PickAudioFile()
        {
            ShowNotImplemented("Picking an audio file");
        }

        [RelayCommand]
        private void PickCover()
        {
            ShowNotImplemented("Picking a cover image");
        }

        [RelayCommand]
        private void Upload()
        {
            ShowNotImplemented("Uploading and analysing a track");
        }
        #endregion
    }
}
