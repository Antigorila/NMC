using NMC.Mobile.ViewModel;

namespace NMC.Mobile.Views
{
    public partial class PlaylistViewPage : ContentPage
    {
        public PlaylistViewPage(PlaylistDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}