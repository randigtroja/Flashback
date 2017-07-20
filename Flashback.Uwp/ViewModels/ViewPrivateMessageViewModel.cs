using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Flashback.Model;
using Flashback.Services.Messages;

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
