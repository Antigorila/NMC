using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Mobile.Models;
using NMC.Mobile.Services;
using NMC.Mobile.Views;
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
        [ObservableProperty]
        private bool _isCheckingSession = true;

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
                await _sessionService.SaveSessionAsync(result.Token, result.UserId.ToString(), result.Username);
                await Shell.Current.GoToAsync(nameof(MainPage));
            }
            else
            {
                ErrorMessage = "This password or username seems wrong...";
            }

            IsBusy = false;
        }

        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.RegisterPage));
        }

        public async Task CheckExistingSessionAsync()
        {
            if (await _sessionService.IsLoggedInAsync())
            {
                var currentRoute = Shell.Current.CurrentState.Location.OriginalString;
                if (!currentRoute.Contains(nameof(MainPage)))
                {
                    await Shell.Current.GoToAsync(nameof(MainPage));
                }
            }
            else
            {
                IsCheckingSession = false;
            }
        }
    }
}
