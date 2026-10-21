using NMC.Client.ViewModels;

namespace NMC.Client.Views
{
    public partial class LibraryPage : ContentPage
    {
        private readonly LibraryViewModel _viewModel;

        public LibraryPage(LibraryViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadTracksCommand.ExecuteAsync(null);
        }
    }
}
