using NMC.Client.Services;
using NMC.Client.ViewModels;

namespace NMC.Client.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly LoginViewModel _viewModel;
        private readonly DemoDataService _demoDataService;

        public LoginPage(LoginViewModel viewModel, DemoDataService demoDataService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _demoDataService = demoDataService;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.IsBusy = true;
            await _demoDataService.EnsureFilesAsync();
            _viewModel.IsBusy = false;

            await _viewModel.InitializeAsync();
        }
    }
}
