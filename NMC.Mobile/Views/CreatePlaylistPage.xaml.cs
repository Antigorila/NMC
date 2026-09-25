using NMC.Mobile.ViewModel;

namespace NMC.Mobile.Views
{
    public partial class CreatePlaylistPage : ContentPage
    {
        private readonly CreatePlaylistViewModel _viewModel;

        public CreatePlaylistPage(CreatePlaylistViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }
    }
}