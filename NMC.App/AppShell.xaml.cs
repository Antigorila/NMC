using NMC.App.MVVM.Views;

namespace NMC.App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(RegisterView), typeof(NMC.App.MVVM.Views.RegisterView));
		Routing.RegisterRoute(nameof(LoginView), typeof(NMC.App.MVVM.Views.LoginView));
		Routing.RegisterRoute(nameof(MainPageView), typeof(NMC.App.MVVM.Views.MainPageView));
	}
}
