using NMC.Mobile.ViewModel;

namespace NMC.Mobile.Views
{
    public partial class AddMusicPage : ContentPage
    {
        private readonly AddMusicViewModel _viewModel;

        public AddMusicPage(AddMusicViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        private async void SelectTrackButton_Clicked(object sender, EventArgs e)
        {
            await _viewModel.SelectAudioCommand.ExecuteAsync(null);

            if (!string.IsNullOrEmpty(_viewModel.FilePath))
            {
                SelectTrackButton.IsVisible = false;
                TrackCover.IsEnabled = true;
                Title.IsVisible = true;
                Artist.IsVisible = true;
                AddTrackButton.IsVisible = true;
                CoverImageLabel.IsVisible = true;

                if (!string.IsNullOrEmpty(_viewModel.Title)) Title.Text = _viewModel.Title;
                if (!string.IsNullOrEmpty(_viewModel.Artist)) Artist.Text = _viewModel.Artist;

                if (!string.IsNullOrEmpty(_viewModel.CoverPath))
                {
                    TrackCover.Source = _viewModel.CoverPath;
                }
            }
        }

        private async void AddTrackButton_Clicked(object sender, EventArgs e)
        {
            if(Title.Text == null)
            {
                await DisplayAlertAsync("Error", "Please enter a title for the track.", "OK");
                return;
            }
            
            if(Artist.Text == null)
            {
                await DisplayAlertAsync("Error", "Please enter an artist for the track.", "OK");
                return;
            }

            string enteredTitle = Title.Text?.Trim();
            string enteredArtist = Artist.Text?.Trim();

            _viewModel.Title = enteredTitle;
            _viewModel.Artist = enteredArtist;

            await _viewModel.UploadTrackCommand.ExecuteAsync(null);

            if (!string.IsNullOrEmpty(_viewModel.ErrorMessage))
            {
                await DisplayAlertAsync("Error", _viewModel.ErrorMessage, "OK");
            }
            else
            {
                await DisplayAlertAsync("Success", "The song was uploaded successfully!", "OK");
            }
        }

        private async void TrackCover_Clicked(object sender, EventArgs e)
        {
            await _viewModel.SelectCoverCommand.ExecuteAsync(null);

            if (!string.IsNullOrEmpty(_viewModel.CoverPath))
            {
                TrackCover.Source = _viewModel.CoverPath;
            }
        }
    }
}