using NMC.App.MVVM.ViewModels;

namespace NMC.App.MVVM.Views;

public partial class LoginView : ContentPage
{
	public LoginView(UserViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}