using System;
using System.Diagnostics;
using Flashback.Services.Auth;
using FlashbackUwp.Views;
using GalaSoft.MvvmLight.Messaging;

namespace FlashbackUwp.ViewModels
{
    public class LoginViewModel : FlashbackViewModelBase
    {
        private AuthService _authSerivce;

        private string _userName = "";
        public string UserName { get { return _userName; } set { Set(ref _userName, value); } }

        private string _password = "";        
        public string Password { get { return _password; } set { Set(ref _password, value); } }
        
        public LoginViewModel()
        {
            _authSerivce = new AuthService(App.CookieContainer);
        }

        public async void Login()
        {
            var isSuccess = await _authSerivce.TryLogin(UserName, Password);
            Debug.WriteLine("Användarens IsLoggedIn sätts till: " + isSuccess);
            Messenger.Default.Send(isSuccess, "LoggedInStatus");

            if (isSuccess)
            {
                await new Windows.UI.Popups.MessageDialog("Ok. Du är inloggad!").ShowAsync();
                await NavigationService.NavigateAsync(typeof(ForumMainList));
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("Felaktigt lösenord/användarnamn!").ShowAsync();
            }           
        }
    }
}
