using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;
using NMC.Client.Navigation;
using NMC.Client.Services;

namespace NMC.Client.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly SessionService _sessionService;
        private readonly PlayerViewModel _playerViewModel;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string newUsername = string.Empty;

        [ObservableProperty]
        private string newPassword = string.Empty;

        public ProfileViewModel(AuthService authService, SessionService sessionService, PlayerViewModel playerViewModel)
        {
            _authService = authService;
            _sessionService = sessionService;
            _playerViewModel = playerViewModel;
            Username = _sessionService.Username;
        }

        #region Commands
        [RelayCommand]
        private async Task UpdateUsername()
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                ApiResult<UserSessionModel> result = await _authService.UpdateUsernameAsync(_sessionService.UserId, NewUsername);

                if (!result.IsSuccess || result.Data is null)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                await _sessionService.SaveAsync(result.Data);
                Username = result.Data.Username;
                NewUsername = string.Empty;

                ShowAlert("Profile", "Your username was changed.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ChangePassword()
        {
            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                ApiResult result = await _authService.ChangePasswordAsync(_sessionService.UserId, NewPassword);

                if (!result.IsSuccess)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                NewPassword = string.Empty;

                ShowAlert("Profile", "Your password was changed.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteAccount()
        {
            bool confirmed = await ConfirmAsync("Delete account", "Your playlists and your library will be deleted permanently. Continue?");
            if (!confirmed)
            {
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;

            try
            {
                ApiResult result = await _authService.DeleteAccountAsync(_sessionService.UserId);

                if (!result.IsSuccess)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                _playerViewModel.Stop();
                _sessionService.Clear();
                await NavigateAsync(Routes.LoginRoot);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task Logout()
        {
            _playerViewModel.Stop();
            _sessionService.Clear();
            await NavigateAsync(Routes.LoginRoot);
        }
        #endregion
    }
}
