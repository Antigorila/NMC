using NMC.Mobile.ViewModel;

namespace NMC.Mobile.Views
{
    public partial class MainPage : ContentPage
    {
        private MainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
        }

        public MainPage(MainViewModel viewModel) : this()
        {
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null && Handler?.MauiContext != null)
            {
                BindingContext = _viewModel = Handler.MauiContext.Services.GetService<MainViewModel>();
            }

            if (_viewModel != null)
            {
                await _viewModel.LoadPlaylistsAsync();
            }
        }

        private async void CreatePlaylistButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CreatePlaylistPage));
        }

        private async void StatsButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(StatsPage));
        }

        private async void ProfileButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ProfilePage));
        }

        private async void AddMusic_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AddMusicPage));
        }
    }
}