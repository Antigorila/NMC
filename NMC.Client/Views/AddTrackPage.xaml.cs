using NMC.Client.ViewModels;

namespace NMC.Client.Views
{
    public partial class AddTrackPage : ContentPage
    {
        private readonly AddTrackViewModel _viewModel;

        public AddTrackPage(AddTrackViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.DiscardUnsavedFiles();
        }
    }
}
