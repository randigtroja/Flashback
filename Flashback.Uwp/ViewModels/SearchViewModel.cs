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
    public class SearchViewModel : FlashbackViewModelBase
    {
        private ObservableCollection<FbItem> _searchResult;
        private readonly ThreadsService _threadService;
        private readonly SettingsService _settings;

        public bool IsDataLoaded => SearchResult != null && SearchResult.Any();

        private string _searchTerm;
        private string _forumId;
        private bool _canSearch;

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                Set(ref _searchTerm, value);
                RaisePropertyChanged("CanSearch");
            }
        }

        public string ForumId
        {
            get => _forumId;
            set => Set(ref _forumId, value);
        }

        public bool CanSearch
        {
            get => !string.IsNullOrWhiteSpace(SearchTerm) && SearchTerm.Length >= 2;
            set => Set(ref _canSearch, value);
        }

        public ObservableCollection<FbItem> SearchResult
        {
            get => _searchResult;
            set => Set(ref _searchResult, value);
        }

        public SearchViewModel()
        {
            _threadService = new ThreadsService(App.CookieContainer, null);
            _settings = SettingsService.Instance;

            SearchResult = new ObservableCollection<FbItem>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                SearchResult = SampleData.SampleData.GetDefaultSearchResult();
            }
        }

        public async Task LoadViewModel()
        {
            if (!CanSearch)
                return;

            try
            {
                Busy.SetBusy(true, "Söker...");
                Error = null;

                var result = await _threadService.SearchThreads(SearchTerm, ForumId);
                SearchResult = new ObservableCollection<FbItem>(result);
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

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var forumId = parameter as string;
            if (forumId != null)
            {
                ForumId = forumId;
            }

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
                NavigationService.Navigate(typeof(ThreadPage), item.Id + (_settings.HoppaTillSistaSidan ? "s" : ""));
            }
        }
    }
}
