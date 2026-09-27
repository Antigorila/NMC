using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Mobile.Services;
using NMC.Mobile.Views;

namespace NMC.Mobile.ViewModel
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly AuthenticationService _authService;
        private readonly SessionService _sessionService;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _newUsername = string.Empty;

        [ObservableProperty]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        public ProfileViewModel(AuthenticationService authService, SessionService sessionService)
        {
            _authService = authService;
            _sessionService = sessionService;
        }

        [RelayCommand]
        private async Task UpdateUsernameAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(NewUsername))
            {
                ErrorMessage = "Please enter a valid new username.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;
            StatusMessage = string.Empty;

            var userIdStr = await _sessionService.GetUserIdAsync();
            if (int.TryParse(userIdStr, out int userId))
            {
                var result = await _authService.UpdateUsernameAsync(userId, NewUsername);
                if (result != null)
                {
                    Username = result.Username;
                    await _sessionService.SaveSessionAsync(result.Token, result.UserId.ToString(), result.Username);
                    StatusMessage = "Username updated successfully!";
                    NewUsername = string.Empty;
                }
                else
                {
                    ErrorMessage = "Failed to update username. It might be taken.";
                }
            }

            IsBusy = false;
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                ErrorMessage = "Please enter a new password.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;
            StatusMessage = string.Empty;

            var userIdStr = await _sessionService.GetUserIdAsync();
            if (int.TryParse(userIdStr, out int userId))
            {
                bool success = await _authService.ChangePasswordAsync(userId, NewPassword);
                if (success)
                {
                    StatusMessage = "Password changed successfully!";
                    NewPassword = string.Empty;
                }
                else
                {
                    ErrorMessage = "Failed to change password.";
                }
            }

            IsBusy = false;
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                _sessionService.ClearSession();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    App.Current!.MainPage = new AppShell();
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Logout failed: {ex.Message}";
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteAccountAsync()
        {
            if (IsBusy) return;

            bool confirm = await Shell.Current.DisplayAlertAsync("Delete Account", "Are you sure you want to permanently delete your account?", "Yes, Delete", "Cancel");

            if (!confirm) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var userIdStr = await _sessionService.GetUserIdAsync();
                if (int.TryParse(userIdStr, out int userId))
                {
                    bool success = await _authService.DeleteAccountAsync(userId);
                    if (success)
                    {
                        _sessionService.ClearSession();

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            App.Current!.MainPage = new AppShell();
                        });
                        return;
                    }
                    else
                    {
                        ErrorMessage = "Failed to delete account on the server.";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }

            IsBusy = false;
        }

        public async Task InitializeAsync()
        {
            var storedUsername = await _sessionService.GetUsernameAsync();
            if (!string.IsNullOrEmpty(storedUsername))
            {
                Username = storedUsername;
            }
            else
            {
                var userIdStr = await _sessionService.GetUserIdAsync();
                Username = !string.IsNullOrEmpty(userIdStr) ? $"User #{userIdStr}" : "User";
            }
        }
    }
}