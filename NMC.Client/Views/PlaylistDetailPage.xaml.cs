using NMC.Client.ViewModels;

namespace NMC.Client.Views
{
    public partial class PlaylistDetailPage : ContentPage
    {
        private readonly PlaylistDetailViewModel _viewModel;

        public PlaylistDetailPage(PlaylistDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadPlaylistCommand.ExecuteAsync(null);
        }
    }
}
