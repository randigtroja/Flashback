using Flashback.Model;
using FlashbackUwp.Views;
using GalaSoft.MvvmLight.Messaging;
using Template10.Mvvm;

namespace FlashbackUwp.ViewModels
{
    public class FlashbackViewModelBase : ViewModelBase
    {
        private string _error;

        public string Error
        {
            get => _error;
            set
            {
                Set(ref _error, value);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    Messenger.Default.Send<string>("Fel uppstod! " + value, FlashbackConstants.MessengerShowError);
                }                
            }
        }

        public void OpenMenu()
        {
            Shell.HamburgerMenu.IsOpen = !Shell.HamburgerMenu.IsOpen;
        }
    }
}
