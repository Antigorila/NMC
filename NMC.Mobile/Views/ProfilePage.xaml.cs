using NMC.Mobile.ViewModel;

namespace NMC.Mobile.Views
{
    public partial class ProfilePage : ContentPage
    {
        private readonly ProfileViewModel? _viewModel;
        public ProfilePage()
        {
            InitializeComponent();
        }

        public ProfilePage(ProfileViewModel viewModel) : this()
        {
            BindingContext = viewModel;
            _viewModel = viewModel;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel != null)
            {
                await _viewModel.InitializeAsync();
            }
        }
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}