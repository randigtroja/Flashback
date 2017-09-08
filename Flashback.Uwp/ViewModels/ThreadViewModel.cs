using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Flashback.Model;
using Flashback.Services;
using Flashback.Services.Threads;
using FlashbackUwp.Services.FileServices;
using FlashbackUwp.Services.SettingsServices;
using FlashbackUwp.Views;
using GalaSoft.MvvmLight.Messaging;


namespace FlashbackUwp.ViewModels
{
    public class ThreadViewModel : FlashbackViewModelBase
    {
        private ForumThread _forumThread;
        private readonly ThreadsService _threadService;
        private readonly FileService _fileService;
        private readonly SettingsService _settings;
        private string _requestedId;
        private bool _hasScrolled = false;
        private bool _firstLoadDone = false;

        public ForumThread ForumThread
        {
            get => _forumThread;
            set => Set(ref _forumThread, value);
        }

        private bool _isPinned;
        public bool IsPinned
        {
            get => _isPinned;
            set => Set(ref _isPinned, value);
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => Set(ref _isLoggedIn, value);
        }

        public ThreadViewModel()
        {
            _forumThread = new ForumThread();

            var c = (Color)Application.Current.Resources["SystemAccentColor"];
            _settings = SettingsService.Instance;
            _fileService = new FileService();

            _threadService = new ThreadsService(App.CookieContainer, new HtmlRenderOptions()
            {
                IsDarkThemed = _settings.AppTheme == ApplicationTheme.Dark,
                ShowAvatars = _settings.ShowAvatars,
                AccentColor = string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B),
                FontSize = _settings.FontSize,
                RenderEmoticons = _settings.UseEmoticons,
                ShowSignatures = _settings.ShowSignatures
            });

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {                
                _forumThread = SampleData.SampleData.GetDefaultForumThread();
            }
        }
        
        public async Task LoadViewModel(string id)
        {
            try
            {
                Busy.SetBusy(true, "Laddar tråden...");                
                Error = null;
                _requestedId = id;

                if (!id.Contains("#") && !id.StartsWith("p") && !id.StartsWith("sp") && _settings.UseSmartNavigation && !_firstLoadDone)
                {
                    // Så länge vi inte navigerar till ett specifikt citerat inlägg och kör med inställningen smartnavigering
                    // så kollat vi om vi har besökt tråden tidigare. Har vi det hoppar vi till senast besökta sida i den.
                    var lastVisitedPageNumber = await _fileService.GetLastVisitedPageForThread(id.GetCleanId(false));

                    if (lastVisitedPageNumber.HasValue)
                    {
                        id = id.GetCleanId(false).GetCleanIdForPage(lastVisitedPageNumber.Value);
                    }

                }

                ForumThread = await _threadService.GetForumThread(id);

                Messenger.Default.Send(ForumThread.UnreadMessagesCount, FlashbackConstants.MessengerUnreadMessagesCount);

                _firstLoadDone = true;

                // spara ner senaste besökta sida om vi använder smartnavigering
                if (_settings.UseSmartNavigation)
                {                  
                    await _fileService.SaveLastVisitedPageNumber(id.GetCleanId(false),ForumThread.CurrentPage);
                }

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

        public void BrowserReady(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            // Om vi begär en tråd utifrån ett specifikt inlägg. Släng iväg message och låt vyn med webview scrolla ner till detta om möjligt. 
            // Vi kollar så det finns text i vymodellen samt om vi redan har scrollat, kan hamna i konstiga lägen annars då webview är muppig som satan
            if (_hasScrolled == false && this._requestedId != null && _requestedId.Contains("#") && _requestedId.StartsWith("p") && this.ForumThread.Html != null)
            {
                _hasScrolled = true;
                var index = _requestedId.IndexOf("#");
                Messenger.Default.Send<string>(_requestedId.Substring(0, index), FlashbackConstants.MessengerBrowserScrollToPost);
            }
        }

        public async void NavigateToParentForum()
        {
            if (!string.IsNullOrWhiteSpace(ForumThread.ParentId) && ForumThread.ParentId.StartsWith("p"))
            {
                await NavigationService.NavigateAsync(typeof(ThreadPage), ForumThread.ParentId);
            }
            else if (!string.IsNullOrWhiteSpace(ForumThread.ParentId))
            {
                await NavigationService.NavigateAsync(typeof(ForumMainList), ForumThread.ParentId);
            }
        }

        public async void Refresh()
        {
            await LoadViewModel(ForumThread.Id);
        }

        public async void AddToFavourites()
        {
            var result = await _threadService.AddThreadToFavourites(ForumThread.Id.GetCleanId(true));

            if (result)
            {

                Messenger.Default.Send(true, FlashbackConstants.MessengerFavoritesUpdated);
                Messenger.Default.Send("Ok, tråden är tillagd till favoriterna!", FlashbackConstants.MessengerShowInformation);                
            }
            else 
            {                
                Messenger.Default.Send("Fel vid tillägg av favorit!", FlashbackConstants.MessengerShowError);
            } 
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (parameter == null)
                throw new ArgumentException("Saknas id för vymodell(threads)");            

            ForumThread.Id = parameter.ToString();
            
            await LoadViewModel(parameter.ToString());

            IsPinned = SecondaryTile.Exists(ForumThread.Id);
            IsLoggedIn = App.IsUserLoggedIn();
        }
        
        public async Task Pin()
        {
            try
            {
                var tile = new SecondaryTile(
                    ForumThread.Id,
                    ForumThread.Title,
                    ForumThread.Id,
                    new Uri("ms-appx:///Assets/Square150x150Logo.png"),
                    TileSize.Default) {RoamingEnabled = false};

                tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.png");
                tile.VisualElements.ShowNameOnSquare150x150Logo = true;
                tile.VisualElements.ShowNameOnWide310x150Logo = true;
                tile.VisualElements.ShowNameOnSquare310x310Logo = true;

                bool success = await tile.RequestCreateAsync();

                if (success)
                    IsPinned = true;
            }
            catch (Exception e)
            {
                Error = e.Message;
            }            
        }

        public async Task UnPin()
        {
            if (SecondaryTile.Exists(ForumThread.Id))
            {
                SecondaryTile toBeDeleted = new SecondaryTile(ForumThread.Id);
                await toBeDeleted.RequestDeleteAsync();

                IsPinned = false;
            }                        
        }

        public async Task FirstPage()
        {
            await LoadViewModel(ForumThread.Id.GetCleanIdFirstPage());
        }

        public async Task PrevioustPage()
        {
            if(ForumThread.CurrentPage < 2)
                return;

            await LoadViewModel(ForumThread.Id.GetCleanIdPreviousPage(ForumThread.CurrentPage));
        }

        public async Task LastPage()
        {
            await LoadViewModel(ForumThread.Id.GetCleanIdLastPage(ForumThread.MaxPages));
        }

        public async Task NextPage()
        {
            if (ForumThread.CurrentPage >= ForumThread.MaxPages)
                return;

            await LoadViewModel(ForumThread.Id.GetCleanIdNextPage(ForumThread.CurrentPage));
        }

        public async Task PostReply()
        {
            await NavigationService.NavigateAsync(typeof(PostReplyPage),new PostReplyRequest(){Id = this.ForumThread.ReplyId, IsQuote = false});
        }

        public async Task ShowPicker()
        {
            InputScope scope = new InputScope();
            InputScopeName scopeName = new InputScopeName { NameValue = InputScopeNameValue.Number };

            scope.Names.Add(scopeName);

            TextBox inputTextBox = new TextBox
            {
                AcceptsReturn = false,
                Height = 32,
                InputScope = scope
            };

            ContentDialog dialog = new ContentDialog
            {
                Content = inputTextBox,
                Title = "Hoppa till sida",
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = "Ok",
                SecondaryButtonText = "Avbryt",
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (int.TryParse(inputTextBox.Text, out int pageNumer) && pageNumer > 0 && pageNumer <= ForumThread.MaxPages)
                {
                    await LoadViewModel(ForumThread.Id.GetCleanIdForPage(pageNumer));
                }
            }
        }

        public async void WebView_OnScriptNotify(object sender, NotifyEventArgs e)
        {
            if (e.Value.Contains("left"))
            {
                await this.NextPage();
                return;
            }
            else if (e.Value.Contains("right"))
            {
                await this.PrevioustPage();
                return;
            }
            else if (e.Value.Contains("post"))
            {
                await NavigationService.NavigateAsync(typeof(PostReplyPage), new PostReplyRequest() {Id = e.Value.Replace("post", "") , IsQuote = true});
            }
        }
    }
}
