using NMC.Mobile.ViewModel;

namespace NMC.Mobile.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    public MainPage(MainViewModel viewModel) : this()
    {
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext == null && Handler?.MauiContext != null)
        {
            BindingContext = Handler.MauiContext.Services.GetService<MainViewModel>();
        }
    }
}