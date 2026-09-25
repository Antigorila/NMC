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