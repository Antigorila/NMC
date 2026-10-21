using NMC.Client.ViewModels;

namespace NMC.Client.Views
{
    public partial class NowPlayingPage : ContentPage
    {
        public NowPlayingPage(PlayerViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
