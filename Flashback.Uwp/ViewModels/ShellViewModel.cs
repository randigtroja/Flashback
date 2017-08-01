using System;
using System.Threading.Tasks;
using Flashback.Model;
using GalaSoft.MvvmLight.Messaging;
using Template10.Mvvm;

namespace FlashbackUwp.ViewModels
{
    public class ShellViewModel: FlashbackViewModelBase
    {
        private bool _isLoggedIn;        

        DelegateCommand _logoutCommand;
        public DelegateCommand LogoutCommand => _logoutCommand ?? (_logoutCommand = new DelegateCommand(async () => await Logout()));
        

        public bool IsLoggedIn
        {
            get
            {
                return _isLoggedIn;
            }
            set
            {
                Set(ref _isLoggedIn, value);
            }
        }

        public ShellViewModel()
        {
            Messenger.Default.Register<bool>(this, FlashbackConstants.MessengerLoggedInStatus, (result) => IsLoggedIn = result);            
        }

        public async Task Logout()
        {
            var dialog = new Windows.UI.Popups.MessageDialog("Vill du logga ut?", "Logga ut");

            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Ja") { Id = 0 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Avbryt") { Id = 1 });
            
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            if (result.Label == "Ja")
            {
                try
                {
                    Views.Busy.SetBusy(true, "Loggar ut...");
                    await App.Logout();
                }
                catch (Exception e)
                {
                    Error = e.Message;
                }
                finally
                {
                    Views.Busy.SetBusy(false, null);
                }
            }                        
        }
    }
}
