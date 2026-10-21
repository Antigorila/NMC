using NMC.Client.ViewModels;

namespace NMC.Client.Views
{
    public partial class PlaylistEditPage : ContentPage
    {
        public PlaylistEditPage(PlaylistEditViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
