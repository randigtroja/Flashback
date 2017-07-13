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
using FlashbackUwp.Services.SettingsServices;
using FlashbackUwp.Views;
using GalaSoft.MvvmLight.Messaging;


namespace FlashbackUwp.ViewModels
{
    public class ThreadViewModel : FlashbackViewModelBase
    {
        private ForumThread _forumThread;
        private ThreadsService _threadService;
        private readonly SettingsService _settings;
        private string _requestedId;
        private bool _hasScrolled = false;

        public ForumThread ForumThread
        {
            get
            {
                return _forumThread;
            }
            set
            {
                Set(ref _forumThread, value);
            }
        }

        private bool _isPinned;
        public bool IsPinned
        {
            get
            {
                return _isPinned;
            }
            set
            {
                Set(ref _isPinned, value);
            }
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get
            {
                return _isLoggedIn;
            }
            set
            {
                Set(ref _isLoggedIn, value);
            }
        }

        public ThreadViewModel()
        {
            _forumThread = new ForumThread();

            var c = (Color)Application.Current.Resources["SystemAccentColor"];
            _settings = SettingsService.Instance;

            _threadService = new ThreadsService(App.CookieContainer, new ThreadRenderOptions()
            {
                IsDarkThemed = _settings.AppTheme == ApplicationTheme.Dark,
                ShowAvatars = _settings.ShowAvatars,
                AccentColor = string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B),
                FontSize = _settings.FontSize
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

                ForumThread = await _threadService.GetForumThread(id);                
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

        public void BrowserReady(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            // Om vi begär en tråd utifrån ett specifikt inlägg. Släng iväg message och låt vyn med webview scrolla ner till detta om möjligt. 
            // Vi kollar så det finns text i vymodellen samt om vi redan har scrollat, kan hamna i konstiga lägen annars då webview är muppig som satan
            if (_hasScrolled == false && this._requestedId != null && _requestedId.Contains("#") && _requestedId.StartsWith("p") && this.ForumThread.Html != null)
            {
                _hasScrolled = true;
                var index = _requestedId.IndexOf("#");
                Messenger.Default.Send<string>(_requestedId.Substring(0, index), "ScrollToPost");
            }
        }

        public async void NavigateToParentForum()
        {
            if (!string.IsNullOrWhiteSpace(ForumThread.ParentId))
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
                await new Windows.UI.Popups.MessageDialog("Ok, tråden är tillagd till favoriterna!").ShowAsync();
            }
            else 
            {
                await new Windows.UI.Popups.MessageDialog("Fel vid tillägg av favorit").ShowAsync();
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
                int pageNumer;

                if (int.TryParse(inputTextBox.Text, out pageNumer) && pageNumer > 0 && pageNumer <= ForumThread.MaxPages)
                {
                    await LoadViewModel(ForumThread.Id.GetCleanIdForPage(pageNumer));
                }
            }
        }
    }
}
