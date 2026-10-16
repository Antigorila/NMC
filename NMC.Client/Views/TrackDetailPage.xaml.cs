using NMC.Client.ViewModels;

namespace NMC.Client.Views
{
    public partial class TrackDetailPage : ContentPage
    {
        private readonly TrackDetailViewModel _viewModel;

        public TrackDetailPage(TrackDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadPlaylistsCommand.ExecuteAsync(null);
        }
    }
}
