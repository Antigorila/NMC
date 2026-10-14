using NMC.App.MVVM.ViewModels;

namespace NMC.App.MVVM.Views;

public partial class RegisterView : ContentPage
{
    public RegisterView(UserViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}