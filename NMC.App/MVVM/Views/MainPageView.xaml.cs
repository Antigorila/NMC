using NMC.App.MVVM.ViewModels;

namespace NMC.App.MVVM.Views;

public partial class MainPageView : ContentPage
{
    public MainPageView(MusicPlayerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}