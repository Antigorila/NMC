using NMC.Client.ViewModels;

namespace NMC.Client.Views
{
    public partial class RecommendationPage : ContentPage
    {
        private readonly RecommendationViewModel _viewModel;

        public RecommendationPage(RecommendationViewModel viewModel)
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
