using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NMC.Client.Messages;

namespace NMC.Client.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string errorMessage = string.Empty;

        public bool IsNotBusy => !IsBusy;

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        #region Navigation
        [RelayCommand]
        protected async Task GoBack()
        {
            await NavigateAsync("..");
        }

        protected async Task NavigateAsync(string route)
        {
            try
            {
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                ShowAlert("Navigation error", ex.Message);
            }
        }

        protected async Task NavigateAsync(string route, ShellNavigationQueryParameters parameters)
        {
            try
            {
                await Shell.Current.GoToAsync(route, parameters);
            }
            catch (Exception ex)
            {
                ShowAlert("Navigation error", ex.Message);
            }
        }
        #endregion

        #region Messages
        protected void ShowAlert(string title, string message)
        {
            WeakReferenceMessenger.Default.Send(new AlertMessage(title, message));
        }

        protected bool HasInternetAccess()
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                return true;
            }

            ShowAlert("No internet", "There is no internet connection. Check your network and try again.");
            return false;
        }

        protected async Task<bool> ConfirmAsync(string title, string message)
        {
            ConfirmMessage request = WeakReferenceMessenger.Default.Send(new ConfirmMessage(title, message));

            if (!request.HasReceivedResponse)
            {
                return false;
            }

            return await request.Response;
        }
        #endregion
    }
}
