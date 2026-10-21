using Microsoft.Extensions.DependencyInjection;
using NMC.Client.ViewModels;

namespace NMC.Client.Controls
{
    public partial class MiniPlayerView : ContentView
    {
        public MiniPlayerView()
        {
            InitializeComponent();
            BindingContext = IPlatformApplication.Current?.Services.GetService<PlayerViewModel>();
        }
    }
}
