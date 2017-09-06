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
    public class MyStartedThreadsViewModel : FlashbackViewModelBase
    {
        private ObservableCollection<FbItem> _threads;
        private readonly ThreadsService _threadService;
        private readonly SettingsService _settings;

        public bool IsDataLoaded => Threads != null && Threads.Any();

        public ObservableCollection<FbItem> Threads
        {
            get => _threads;
            set => Set(ref _threads, value);
        }

        public MyStartedThreadsViewModel()
        {
            _threadService = new ThreadsService(App.CookieContainer, null);
            _threads = new ObservableCollection<FbItem>();
            _settings = SettingsService.Instance;

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Threads = SampleData.SampleData.GetDefaultMyStartedPosts();
            }
        }

        public async Task LoadViewModel()
        {
            try
            {
                Busy.SetBusy(true, "Laddar dina startade trådar...");
                Error = null;

                var resultNewTopics = await _threadService.GetMyStartedThreads(App.GetUserId());
                Threads = new ObservableCollection<FbItem>(resultNewTopics);
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

        public async void Refresh()
        {
            await LoadViewModel();
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
