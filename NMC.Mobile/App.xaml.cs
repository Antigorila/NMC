using Microsoft.Extensions.DependencyInjection;
using NMC.Mobile.Services;

namespace NMC.Mobile
{
    public partial class App : Application
    {
        public App(AppShell appShell)
        {
            InitializeComponent();
            MainPage = appShell;
        }
    }
}