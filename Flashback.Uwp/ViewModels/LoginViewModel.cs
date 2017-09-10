using System.Diagnostics;
using Flashback.Model;
using Flashback.Services.Auth;
using FlashbackUwp.Views;
using GalaSoft.MvvmLight.Messaging;

namespace FlashbackUwp.ViewModels
{
    public class LoginViewModel : FlashbackViewModelBase
    {
        private AuthService _authSerivce;

        private string _userName = "";
        public string UserName
        {
            get => _userName;
            set => Set(ref _userName, value);
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public LoginViewModel()
        {
            _authSerivce = new AuthService(App.CookieContainer);
        }

        public async void Login()
        {
            var isSuccess = await _authSerivce.TryLogin(UserName, Password);
            Debug.WriteLine("Användarens IsLoggedIn sätts till: " + isSuccess);
            Messenger.Default.Send(isSuccess, FlashbackConstants.MessengerLoggedInStatus);

            if (isSuccess)
            {
                await App.SaveCookies();
                Messenger.Default.Send("Ok! Du är inloggad!", FlashbackConstants.MessengerShowInformation);
                await NavigationService.NavigateAsync(typeof(ForumMainList));
            }
            else
            {
                Messenger.Default.Send("Felaktigt lösenord/användarnamn!", "ShowError");
            }
        }
    }
}
