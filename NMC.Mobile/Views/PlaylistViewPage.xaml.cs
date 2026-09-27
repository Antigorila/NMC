using NMC.Mobile.ViewModel;

namespace NMC.Mobile.Views
{
    public partial class PlaylistViewPage : ContentPage
    {
        private readonly PlaylistDetailViewModel _viewModel;

        public PlaylistViewPage(PlaylistDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }
    }
}