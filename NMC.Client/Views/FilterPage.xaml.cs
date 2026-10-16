using NMC.Client.ViewModels;

namespace NMC.Client.Views
{
    public partial class FilterPage : ContentPage
    {
        private readonly FilterViewModel _viewModel;

        public FilterPage(FilterViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadGenresCommand.ExecuteAsync(null);
        }
    }
}
