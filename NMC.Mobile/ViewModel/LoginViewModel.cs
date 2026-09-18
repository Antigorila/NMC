using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.Mobile.ViewModel
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthenticationService _authService;
        private readonly SessionService _sessionService;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public LoginViewModel(AuthenticationService authService, SessionService sessionService)
        {
            _authService = authService;
            _sessionService = sessionService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Not without a username and password!";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            var result = await _authService.LoginAsync(Username, Password);

            if (result != null)
            {
                await _sessionService.SaveSessionAsync(result.Token, result.UserId);
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                ErrorMessage = "This password or username seems wrong...";
            }

            IsBusy = false;
        }
    }
}
