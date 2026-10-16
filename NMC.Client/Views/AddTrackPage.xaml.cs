using NMC.Client.ViewModels;

namespace NMC.Client.Views
{
    public partial class AddTrackPage : ContentPage
    {
        public AddTrackPage(AddTrackViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
