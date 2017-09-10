using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Flashback.Model;
using Flashback.Services.Messages;
using FlashbackUwp.Views;
using GalaSoft.MvvmLight.Messaging;

namespace FlashbackUwp.ViewModels
{
    public class PrivateMessagesViewModel : FlashbackViewModelBase
    {
        private readonly MessagesService _messagesService;
        public bool IsDataLoaded => Messages != null && Messages.Any();

        private ObservableCollection<PrivateMessage> _messages;
        public ObservableCollection<PrivateMessage> Messages
        {
            get => _messages;
            set => Set(ref _messages, value);
        }

        public PrivateMessagesViewModel()
        {
            _messagesService = new MessagesService(App.CookieContainer, null);
            Messages = new ObservableCollection<PrivateMessage>();
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (!IsDataLoaded)
            {
                await LoadViewModel();
            }

            await Task.CompletedTask;
        }

        public async Task LoadViewModel()
        {
            try
            {
                Busy.SetBusy(true, "Laddar privata meddelanden...");
                Error = null;

                var resultMessages = await _messagesService.GetPrivateMessages();

                if (resultMessages.Any())
                {
                    var unreadCount = resultMessages.Count(x => x.IsUnread);
                    Messenger.Default.Send(unreadCount > 0 ? unreadCount : (int?)null, FlashbackConstants.MessengerUnreadMessagesCount);
                }

                Messages = new ObservableCollection<PrivateMessage>(resultMessages);
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

        public async void Refresh() => await LoadViewModel();

        public async Task NewMessage() => await NavigationService.NavigateAsync(typeof(ComposePrivateMessagePage), null);

        public void NavigateToMessage(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is PrivateMessage item)
            {
                // då fb inte uppdaterar att ett meddedelande är läst då man går in på det automatiskt
                // (sidan måste laddas om för att det ska slå igenom) så gör vi en manuell uppdatering
                // utifrån antalet meddelanden vi har - 1 om meddelandet vi navigerar till är "Unread"
                if (item.IsUnread)
                {
                    var unreadCount = Messages.Count(x => x.IsUnread);
                    unreadCount = unreadCount - 1;

                    Messenger.Default.Send(unreadCount > 0 ? unreadCount : (int?)null, FlashbackConstants.MessengerUnreadMessagesCount);
                } 

                NavigationService.Navigate(typeof(ViewPrivateMessagePage), item.Id);
            }
        }
    }
}
