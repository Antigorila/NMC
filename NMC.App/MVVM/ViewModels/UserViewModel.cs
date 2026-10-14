using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.App.MVVM.Models;
using NMC.App.MVVM.Views;

namespace NMC.App.MVVM.ViewModels
{
    public partial class UserViewModel : ObservableObject
    {
        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [RelayCommand]
        public async Task RegisterUser()
        {
            //todo
            System.Diagnostics.Debug.WriteLine($"REGISTER: {username}");
            await Shell.Current.GoToAsync(nameof(MainPageView));
        }

        [RelayCommand]
        public async Task LoginUser()
        {
            //todo
            System.Diagnostics.Debug.WriteLine($"Login: {username}");
            await Shell.Current.GoToAsync(nameof(MainPageView));
        }

        [RelayCommand]
        public async Task GoToRegistration()
        {
            await Shell.Current.GoToAsync(nameof(RegisterView));
        }

        [RelayCommand]
        public async Task GoToLogin()
        {
            await Shell.Current.GoToAsync($"///{nameof(LoginView)}");
        }
    }
}
