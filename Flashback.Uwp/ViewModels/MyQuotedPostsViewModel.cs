using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Flashback.Model;
using Flashback.Services.Threads;
using FlashbackUwp.Views;

namespace FlashbackUwp.ViewModels
{
    public class MyQuotedPostsViewModel : FlashbackViewModelBase
    {
        private ObservableCollection<FbItem> _posts;
        private readonly ThreadsService _threadService;

        public bool IsDataLoaded => Posts != null && Posts.Any();

        public ObservableCollection<FbItem> Posts
        {
            get => _posts;
            set => Set(ref _posts, value);
        }

        public MyQuotedPostsViewModel()
        {
            _threadService = new ThreadsService(App.CookieContainer, null);

            Posts = new ObservableCollection<FbItem>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Posts = SampleData.SampleData.GetDefaultMyQuotesPosts();
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
                NavigationService.Navigate(typeof(ThreadPage), item.Id);
            }
        }
    }
}
