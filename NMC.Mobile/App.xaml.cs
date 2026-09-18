using Microsoft.Extensions.DependencyInjection;
using NMC.Mobile.Services;

namespace NMC.Mobile
{
    public partial class App : Application
    {
        public App(SessionService sessionService)
        {
            InitializeComponent();

            MainPage = new AppShell();

            CheckSessionAsync(sessionService);
        }

        private async void CheckSessionAsync(SessionService sessionService)
        {
            if (await sessionService.IsLoggedInAsync())
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
    }
}