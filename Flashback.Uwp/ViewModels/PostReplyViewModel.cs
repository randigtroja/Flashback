using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Flashback.Model;
using Flashback.Services.Threads;
using FlashbackUwp.Services.SettingsServices;
using FlashbackUwp.Views;
using GalaSoft.MvvmLight.Messaging;

namespace FlashbackUwp.ViewModels
{
    public class PostReplyViewModel : FlashbackViewModelBase
    {
        private readonly ThreadsService _threadService;
        private readonly SettingsService _settings;

        private string _title;
        private string _message;
        private string _userId;
        private string _threadId;
        private string _subscriptionType;
        private string _postId;
        private bool _mayPost;

        public bool MayPost
        {
            get => _mayPost;
            set => Set(ref _mayPost, value);
        }

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        public string UserId
        {
            get => _userId;
            set => Set(ref _userId, value);
        }

        public string ThreadId
        {
            get => _threadId;
            set => Set(ref _threadId, value);
        }

        public string SubscriptionType
        {
            get => _subscriptionType;
            set => Set(ref _subscriptionType, value);
        }

        public string PostId
        {
            get => _postId;
            set => Set(ref _postId, value);
        }


        public PostReplyViewModel()
        {
            _threadService = new ThreadsService(App.CookieContainer, null);
            _settings = SettingsService.Instance;
            MayPost = true;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var replyRequest = parameter as PostReplyRequest;
            if (replyRequest != null)
            {
                await LoadViewModel(replyRequest.Id, replyRequest.IsQuote);
            }

            await Task.CompletedTask;
        }

        private async Task LoadViewModel(string replyRequestId, bool replyRequestIsQuote)
        {
            try
            {
                Busy.SetBusy(true,"Laddar...");
                Error = null;
                    
                var model = await _threadService.GetReply(replyRequestId, replyRequestIsQuote);

                Title = model.Title;
                Message = model.Message;
                UserId = model.UserId;
                ThreadId = model.ThreadId;
                SubscriptionType = model.SubscriptionType;
                PostId = replyRequestId;
            }
            catch (Exception e)
            {
                Error = e.ToString();
            }
            finally
            {
                Busy.SetBusy(false);
            }            
        }

        public async Task PostMessage()
        {
            var dialog = new Windows.UI.Popups.MessageDialog("Vill du skicka meddelandet?", "Bekräfta");

            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Ja") { Id = 0 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Avbryt") { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var resultDialog = await dialog.ShowAsync();

            if (resultDialog.Label == "Ja")
            {
                MayPost = false;

                var result = await _threadService.PostReply(Message, ThreadId, PostId, UserId, SubscriptionType);

                if (result)
                {                    
                    Messenger.Default.Send<string>("Inlägget är skickat! Gå tillbaka för att ladda om", FlashbackConstants.MessengerShowInformation);
                }
                else
                {                    
                    Messenger.Default.Send<string>("Fel vid skickande av meddelande!", FlashbackConstants.MessengerShowError);
                    MayPost = true;
                }
            }
        }
    }
}
