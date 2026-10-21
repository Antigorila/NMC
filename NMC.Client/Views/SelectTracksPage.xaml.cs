using NMC.Client.ViewModels;

namespace NMC.Client.Views
{
    public partial class SelectTracksPage : ContentPage
    {
        private readonly SelectTracksViewModel _viewModel;

        public SelectTracksPage(SelectTracksViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadTracksCommand.ExecuteAsync(null);
        }
    }
}
