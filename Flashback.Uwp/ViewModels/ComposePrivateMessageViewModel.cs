using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Flashback.Model;
using Flashback.Services.Messages;
using FlashbackUwp.Views;

namespace FlashbackUwp.ViewModels
{
    public class ComposePrivateMessageViewModel : FlashbackViewModelBase
    {
       

        private bool _mayPost;        

        public bool MayPost
        {
            get { return _mayPost; }
            set { Set(ref _mayPost, value); }
        }

        private ComposePrivateMessageModel _composeModel;

        public ComposePrivateMessageModel ComposeModel
        {
            get
            {
                return _composeModel;
            }
            set
            {
                Set(ref _composeModel, value);
            }
        }

        private MessagesService _messageService;

        public ComposePrivateMessageViewModel()
        {
            _messageService = new MessagesService(App.CookieContainer);
            _composeModel = new ComposePrivateMessageModel();
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var id = parameter as string;
            if (id != null)
            {
                ComposeModel.Id =id;
                ComposeModel.IsQuote= true;
            }

            await LoadViewModel();

            await Task.CompletedTask;
        }

        public async Task LoadViewModel()
        {
            
            try
            {
                Busy.SetBusy(true, "Laddar...");
                Error = null;

                var result = await _messageService.NewPrivateMessage(ComposeModel.Id);
                ComposeModel = result;
            }
            catch (Exception e)
            {
                Error = e.Message;
            }
            finally
            {
                Busy.SetBusy(false);
            }
        }
    }
}
