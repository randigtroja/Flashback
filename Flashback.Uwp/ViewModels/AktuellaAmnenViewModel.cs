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
using Template10.Services.NavigationService;

namespace FlashbackUwp.ViewModels
{
    public class AktuellaAmnenViewModel : FlashbackViewModelBase
    {
        private ObservableCollection<FbItem> _topics;
        private ObservableCollection<FbItem> _topicsAll;

        private readonly ThreadsService _threadService;
        private SettingsService _settings;

        private string _filterText;

        public ObservableCollection<FbItem> Topics
        {
            get
            {
                return _topics;
            }
            set
            {
                Set(ref _topics, value);
            }
        }

        public ObservableCollection<FbItem> TopicsAll
        {
            get
            {
                return _topicsAll;
            }
            set
            {
                Set(ref _topicsAll, value);
            }
        }

        public string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {                
                Set(ref _filterText, value);
                FilterList();
            }
        }

        public AktuellaAmnenViewModel()
        {            
            _threadService = new ThreadsService(App.CookieContainer, null);
            _settings = SettingsService.Instance;

            Topics = new ObservableCollection<FbItem>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Topics = SampleData.SampleData.GetDefaultAktuellaÄmnen();
            }
        }

        public async Task LoadViewModel()
        {
            try
            {
                Busy.SetBusy(true, "Laddar aktuella ämnen...");
                Error = null;

                var resultAktuellt = await _threadService.GetHotTopics();
                Topics = new ObservableCollection<FbItem>(resultAktuellt);
                TopicsAll = new ObservableCollection<FbItem>(resultAktuellt);
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
            if (!IsDataLoaded)
            {           
                await LoadViewModel();
            }

            await Task.CompletedTask;
        }

        public bool IsDataLoaded => TopicsAll != null && TopicsAll.Any();        

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public async void Refresh()
        {
            await LoadViewModel();
        }

        public void FilterList()
        {
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                Topics = new ObservableCollection<FbItem>(TopicsAll);
                return;
            }
           
            if (TopicsAll != null && TopicsAll.Any())
            {                
                Topics = new ObservableCollection<FbItem>(TopicsAll.Where(x => x.Name.ToLower().Contains(FilterText.ToLower()) || x.Description.ToLower().Contains(FilterText.ToLower())));
            }
        }

        public void NavigateToForum(object sender, ItemClickEventArgs e)
        {            
            var item = e.ClickedItem as FbItem;

            if (item != null)
            {
                NavigationService.Navigate(typeof(ThreadPage), item.Id + (_settings.HoppaTillSistaSidan ? "s" : ""));
            }
        }       
    }
}
