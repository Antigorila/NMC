using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Mobile.Services;

namespace NMC.Mobile.ViewModel
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly AuthenticationService _authService;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public RegisterViewModel(AuthenticationService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "We can't do it with missing data!";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Sure this is your password?";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            var success = await _authService.RegisterAsync(Username, Password);

            if (success)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ErrorMessage = "Registration failed, the username might be taken already...";
            }

            IsBusy = false;
        }

        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
