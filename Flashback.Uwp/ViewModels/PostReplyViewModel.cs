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
            throw new System.NotImplementedException();
        }
    }
}
