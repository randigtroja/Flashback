using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Flashback.Model;
using Flashback.Services;
using Flashback.Services.Forum;
using FlashbackUwp.Services.CacheServices;
using FlashbackUwp.Services.SettingsServices;
using FlashbackUwp.Views;

namespace FlashbackUwp.ViewModels
{
    public class ForumMainListViewModel : FlashbackViewModelBase
    {
        private ForumList _forumlist;        

        private readonly ForumService _forumService;
        private readonly SettingsService _settings;
        
        public ForumList ForumList
        {
            get
            {
                return _forumlist;
            }
            set
            {
                Set(ref _forumlist, value);                
            }
        }

        public ForumMainListViewModel()
        {                        
            ForumList = new ForumList(){Items = new ObservableCollection<FbItem>()};
            _forumService = new ForumService(App.CookieContainer);

            _settings = SettingsService.Instance;

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                ForumList = SampleData.SampleData.GetDefaultForumList();
            }
        }

        public async Task LoadViewModel(string id)
        {
            try
            {
                CacheService cacheService = new CacheService();

                if (string.IsNullOrWhiteSpace(id) || id == "mainlist")
                {
                    
                    await cacheService.ResetCache();

                    Busy.SetBusy(true, "Laddar forumlistan...");
                    Error = null;

                    var resultForumList = await _forumService.GetMainForumlist();
                    ForumList = resultForumList;
                }
                else
                {
                    Busy.SetBusy(true, "Laddar...");
                    Error = null;

                    var resultForumList = await _forumService.GetForums(id);

                    ForumList = resultForumList;
                }

                if (ForumList.Items.Count > 0)
                {
                    
                    await cacheService.AddToCacheList(ForumList);
                }
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

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var id = parameter == null ? "mainlist" : parameter.ToString();
            ForumList.Id = id;
            
            if (mode == NavigationMode.New || mode == NavigationMode.Refresh)
            {                             
                await LoadViewModel(parameter?.ToString());
            }
            else
            {
                var cache = new CacheService();
                
                var cachedForumlist = await cache.TryGetFromCache(ForumList.Id);

                if (cachedForumlist != null)
                {
                    ForumList = cachedForumlist;                    
                }
                else
                {
                    await LoadViewModel(id);                    
                }
            }            
        }

        public async void Refresh()
        {
            await LoadViewModel(ForumList.Id);
        }

        public async void NavigateToSearch()
        {
            await NavigationService.NavigateAsync(typeof(SearchPage), ForumList.Id);
        }

        public void NavigateToForumThread(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FbItem;

            if (item != null)
            {
                if (item.Type == FbItemType.Forum)
                {
                    NavigationService.Navigate(typeof(ForumMainList), item.Id);
                }
                else if (item.Type == FbItemType.Thread)
                {
                    NavigationService.Navigate(typeof(ThreadPage), item.Id + (_settings.HoppaTillSistaSidan ? "s" : ""));
                }               
            }
        }

        public async Task FirstPage()
        {
            await LoadViewModel(ForumList.Id.GetCleanIdFirstPage());
        }

        public async Task PrevioustPage()
        {
            if (ForumList.CurrentPage < 2)
                return;

            await LoadViewModel(ForumList.Id.GetCleanIdPreviousPage(ForumList.CurrentPage));
        }

        public async Task LastPage()
        {
            await LoadViewModel(ForumList.Id.GetCleanIdLastPage(ForumList.MaxPages));
        }

        public async Task NextPage()
        {
            if (ForumList.CurrentPage >= ForumList.MaxPages)
                return;

            await LoadViewModel(ForumList.Id.GetCleanIdNextPage(ForumList.CurrentPage));
        }
    }
}
