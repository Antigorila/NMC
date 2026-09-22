using NMC.Mobile.ViewModel;

namespace NMC.Mobile.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        public ProfilePage(ProfileViewModel viewModel) : this()
        {
            BindingContext = viewModel;
        }
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}