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

            MayPost = true;
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

        public async Task PostMessage()
        {

            if (string.IsNullOrWhiteSpace(this.ComposeModel.Subject) || string.IsNullOrWhiteSpace(this.ComposeModel.To) || string.IsNullOrWhiteSpace(this.ComposeModel.Message))
            {
                await new Windows.UI.Popups.MessageDialog("Ej fullständiga uppgifter ifyllda för att kunna skicka.").ShowAsync();
                return;
            }

            try
            {
                MayPost = false;

                Busy.SetBusy(true,"Skickar meddelande...");
                Error = null;

                var result = await _messageService.PostMessage(ComposeModel.To, ComposeModel.Subject, ComposeModel.Message, ComposeModel.PostToken);

                if (result)
                {
                    await new Windows.UI.Popups.MessageDialog("Meddelande är skickat!").ShowAsync();
                }
                else
                {
                    await new Windows.UI.Popups.MessageDialog("Något gick fel vid skickande av meddelande.").ShowAsync();
                }
            }
            catch (Exception e)
            {
                Error = e.Message;

            }
            finally
            {
                Busy.SetBusy(false);
                MayPost = true;
            }
        }
    }
}
