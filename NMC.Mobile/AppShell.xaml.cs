using NMC.Mobile.Services;
using NMC.Mobile.Views;

namespace NMC.Mobile;

public partial class AppShell : Shell
{
    private readonly SessionService _sessionService;

    public AppShell(SessionService sessionService)
    {
        InitializeComponent();
        _sessionService = sessionService;

        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));

        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(CreatePlaylistPage), typeof(CreatePlaylistPage));
        Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(AddMusicPage), typeof(AddMusicPage));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (await _sessionService.IsLoggedInAsync())
        {
            await GoToAsync(nameof(MainPage));
        }
    }
}