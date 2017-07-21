using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Flashback.Model;
using Flashback.Services.Messages;
using FlashbackUwp.Views;

namespace FlashbackUwp.ViewModels
{
    public class ViewPrivateMessageViewModel : FlashbackViewModelBase
    {
        private readonly MessagesService _messagesService;
        
        private PrivateMessage _message;
      

        public PrivateMessage Message
        {
            get
            {
                return _message;
            }
            set
            {
                Set(ref _message, value);
            }
        }


        public ViewPrivateMessageViewModel()
        {
            _messagesService = new MessagesService(App.CookieContainer);    
        }

        public async void Delete()
        {
            var ok = await _messagesService.DeleteMessage(Message.Id, Message.FolderId,Message.Token);

            if (ok)
            {
                await new Windows.UI.Popups.MessageDialog("Meddelandet är raderat").ShowAsync();
                await NavigationService.NavigateAsync(typeof(PrivateMessagesPage));
            }
        }

        public async void Reply()
        {

        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var id = parameter as string;

            await LoadViewModel(id);

            await Task.CompletedTask;
        }

        private async Task LoadViewModel(string id)
        {
            Message = await _messagesService.GetMessage(id);            
        }
    }
}
