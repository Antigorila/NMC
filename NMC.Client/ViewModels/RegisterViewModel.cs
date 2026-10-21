using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;
using NMC.Client.Navigation;
using NMC.Client.Services;

namespace NMC.Client.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly SessionService _sessionService;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string confirmPassword = string.Empty;

        public RegisterViewModel(AuthService authService, SessionService sessionService)
        {
            _authService = authService;
            _sessionService = sessionService;
        }

        #region Commands
        [RelayCommand]
        private async Task Register()
        {
            ErrorMessage = string.Empty;

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "The two passwords do not match.";
                return;
            }

            IsBusy = true;

            try
            {
                ApiResult<UserSessionModel> result = await _authService.RegisterAsync(Username.Trim(), Password);

                if (!result.IsSuccess || result.Data is null)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                await _sessionService.SaveAsync(result.Data);
                await NavigateAsync(Routes.MainRoot);
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}
