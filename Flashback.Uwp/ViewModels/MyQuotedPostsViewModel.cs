using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Flashback.Model;
using Flashback.Services.Threads;
using FlashbackUwp.Services.SettingsServices;
using FlashbackUwp.Views;

namespace FlashbackUwp.ViewModels
{
    public class MyQuotedPostsViewModel : FlashbackViewModelBase
    {
        private ObservableCollection<FbItem> _posts;
        private readonly ThreadsService _threadService;
        private readonly SettingsService _settings;

        public bool IsDataLoaded => Posts != null && Posts.Any();

        public ObservableCollection<FbItem> Posts
        {
            get
            {
                return _posts;
            }
            set
            {
                Set(ref _posts, value);
            }
        }

        public MyQuotedPostsViewModel()
        {
            _threadService = new ThreadsService(App.CookieContainer, null);
            _settings = SettingsService.Instance;

            Posts = new ObservableCollection<FbItem>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Posts = SampleData.SampleData.GetDefaultNewTopics();
            }
        }

        public async Task LoadViewModel()
        {
            try
            {
                Busy.SetBusy(true, "Laddar citerade inlägg...");
                Error = null;

                var resultQuotedPosts = await _threadService.GetMyQuotedPosts(App.GetUserId());
                Posts = new ObservableCollection<FbItem>(resultQuotedPosts);
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

        public async void Refresh()
        {
            await LoadViewModel();
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (!IsDataLoaded)
            {
                await LoadViewModel();
            }

            await Task.CompletedTask;
        }

        public void NavigateToThread(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FbItem;

            if (item != null)
            {
                // NavigationService.Navigate(typeof(ThreadPage), item.Id + (_settings.HoppaTillSistaSidan ? "s" : ""));
                NavigationService.Navigate(typeof(ThreadPage), item.Id); // vi borde nog inte hoppa till sista sidan från mina citerade inlägg- vi borde hamna på inlägget ist.
            }
        }
    }
}
