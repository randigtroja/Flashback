using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Flashback.Model;
using Flashback.Services.Threads;
using FlashbackUwp.Services.SettingsServices;

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

        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        public string UserId
        {
            get { return _userId; }
            set { Set(ref _userId, value); }
        }

        public string ThreadId
        {
            get { return _threadId; }
            set { Set(ref _threadId, value); }
        }

        public string SubscriptionType
        {
            get { return _subscriptionType; }
            set { Set(ref _subscriptionType, value); }
        }


        public PostReplyViewModel()
        {
            _threadService = new ThreadsService(App.CookieContainer, null);
            _settings = SettingsService.Instance;
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
            var model = await _threadService.GetReply(replyRequestId, replyRequestIsQuote);

            Title = model.Title;
            Message = model.Message;
            UserId = model.UserId;
            ThreadId = model.ThreadId;
            SubscriptionType = model.SubscriptionType;
        }
    }
}
