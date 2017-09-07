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
    public class NewPostsViewModel:FlashbackViewModelBase
    {
        private ObservableCollection<FbItem> _topics;
        private readonly ThreadsService _threadService;
        private readonly SettingsService _settings;

        public bool IsDataLoaded => Topics != null && Topics.Any();

        public ObservableCollection<FbItem> Topics
        {
            get => _topics;
            set => Set(ref _topics, value);
        }

        public NewPostsViewModel()
        {
            _threadService = new ThreadsService(App.CookieContainer, null);
            _settings = SettingsService.Instance;

            Topics = new ObservableCollection<FbItem>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Topics = SampleData.SampleData.GetDefaultNewPosts();
            }
        }

        public async Task LoadViewModel()
        {
            try
            {
                Busy.SetBusy(true, "Laddar nya inlägg...");
                Error = null;

                var resultNewTopics = await _threadService.GetNewPosts();
                Topics = new ObservableCollection<FbItem>(resultNewTopics);
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
            if (e.ClickedItem is FbItem item)
            {
                NavigationService.Navigate(typeof(ThreadPage), item.Id + (_settings.HoppaTillSistaSidan ? "s" : ""));
            }
        }
    }
}
