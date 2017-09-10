using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Flashback.Model;
using Flashback.Services.Messages;
using FlashbackUwp.Services.SettingsServices;
using FlashbackUwp.Views;
using GalaSoft.MvvmLight.Messaging;

namespace FlashbackUwp.ViewModels
{
    public class ViewPrivateMessageViewModel : FlashbackViewModelBase
    {
        private readonly MessagesService _messagesService;
        private readonly SettingsService _settings;
        private PrivateMessage _message;

        public PrivateMessage Message
        {
            get => _message;
            set => Set(ref _message, value);
        }
        
        public ViewPrivateMessageViewModel()
        {
            var c = (Color)Application.Current.Resources["SystemAccentColor"];
            _settings = SettingsService.Instance;

            _messagesService = new MessagesService(App.CookieContainer, new HtmlRenderOptions()
            {
                IsDarkThemed = _settings.AppTheme == ApplicationTheme.Dark,
                ShowAvatars = _settings.ShowAvatars,
                AccentColor = string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B),
                FontSize = _settings.FontSize,
                RenderEmoticons = _settings.UseEmoticons,
                ShowSignatures = _settings.ShowSignatures
            });
        }

        public async void Delete()
        {
            if (await _messagesService.DeleteMessage(Message.Id, Message.FolderId, Message.Token))
            {
                Messenger.Default.Send("Meddelandet är raderat", FlashbackConstants.MessengerShowInformation);
                await NavigationService.NavigateAsync(typeof(PrivateMessagesPage));
            }
        }

        public async void Reply() => await NavigationService.NavigateAsync(typeof(ComposePrivateMessagePage), Message.Id);

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            await LoadViewModel(parameter as string);

            await Task.CompletedTask;
        }

        private async Task LoadViewModel(string id)
        {
            Message = await _messagesService.GetMessage(id);
        }
    }
}
