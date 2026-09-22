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

    private void AddPlaylistButton_Clicked(object sender, EventArgs e)
    {

    }

    private void StatsButton_Clicked(object sender, EventArgs e)
    {

    }

    private void ProfileButton_Clicked(object sender, EventArgs e)
    {

    }
}