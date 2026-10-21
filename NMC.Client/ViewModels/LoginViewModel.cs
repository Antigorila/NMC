using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;
using NMC.Client.Navigation;
using NMC.Client.Services;

namespace NMC.Client.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly SessionService _sessionService;

        private bool _sessionChecked;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        public LoginViewModel(AuthService authService, SessionService sessionService)
        {
            _authService = authService;
            _sessionService = sessionService;
        }

        public async Task InitializeAsync()
        {
            Password = string.Empty;

            if (_sessionChecked)
            {
                return;
            }

            _sessionChecked = true;

            bool restored = await _sessionService.TryRestoreAsync();
            if (restored)
            {
                await NavigateAsync(Routes.MainRoot);
            }
        }

        #region Commands
        [RelayCommand]
        private async Task Login()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                ApiResult<UserSessionModel> result = await _authService.LoginAsync(Username.Trim(), Password);

                if (!result.IsSuccess || result.Data is null)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                await _sessionService.SaveAsync(result.Data);
                Password = string.Empty;

                await NavigateAsync(Routes.MainRoot);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToRegister()
        {
            ErrorMessage = string.Empty;
            await NavigateAsync(Routes.Register);
        }
        #endregion
    }
}
