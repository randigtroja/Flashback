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
        private string _subject;
        private string _id;
        private bool _isQuote;
        private string _to;
        private string _message;
        private string _postToken;

        public bool MayPost
        {
            get { return _mayPost; }
            set { Set(ref _mayPost, value); }
        }

        public string Id
        {
            get { return _id; }
            set { Set(ref _id,value); }
        }

        public bool IsQuote
        {
            get { return _isQuote; }
            set { Set(ref _isQuote, value); }
        }

        public string To
        {
            get { return _to; }
            set { Set(ref _to, value); }
        }

        public string Subject
        {
            get { return _subject; }
            set { Set(ref _subject, value); }
        }

        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        public string PostToken
        {
            get { return _postToken; }
            set { Set(ref _postToken, value); }
        }
       
        private MessagesService _messageService;

        public ComposePrivateMessageViewModel()
        {
            _messageService = new MessagesService(App.CookieContainer,null);            

            MayPost = true;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var id = parameter as string;
            if (id != null)
            {
                Id =id;
                IsQuote= true;
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

                var result = await _messageService.NewPrivateMessage(Id);

                Id = result.Id;
                PostToken = result.PostToken;
                Message = result.Message;
                To = result.To;
                Subject = result.Subject;

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

            if (string.IsNullOrWhiteSpace(Subject) || string.IsNullOrWhiteSpace(To) || string.IsNullOrWhiteSpace(Message))
            {
                await new Windows.UI.Popups.MessageDialog("Ej fullständiga uppgifter ifyllda för att kunna skicka.").ShowAsync();
                return;
            }

            try
            {
                MayPost = false;

                Busy.SetBusy(true,"Skickar meddelande...");
                Error = null;

                var result = await _messageService.PostMessage(To, Subject, Message, PostToken);

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
