using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Flashback.Model;
using Flashback.Services.News;
using Windows.System;

namespace FlashbackUwp.ViewModels
{
    public class NyheterViewModel : FlashbackViewModelBase
    {
        private ObservableCollection<FbRssItem> _nyheter;
        private readonly NewsService _service = new NewsService();

        public ObservableCollection<FbRssItem> Nyheter
        {
            get => _nyheter;
            set => Set(ref _nyheter, value);
        }

        public NyheterViewModel()
        {
            _nyheter = new ObservableCollection<FbRssItem>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Nyheter = SampleData.SampleData.GetDefaultRssItems();
            }
        }

        private async Task LoadViewModel()
        {
            try
            {
                Error = null;
                Views.Busy.SetBusy(true,"Laddar nyheter...");

                var news = await _service.GetNyheter();
                Nyheter = new ObservableCollection<FbRssItem>(news);
            }
            catch (Exception e)
            {
                Error = e.ToString();
            }
            finally
            {
                Views.Busy.SetBusy(false);
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            await LoadViewModel();
        }

        public async void RssNavigate(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is FbRssItem item)
            {
                await Launcher.LaunchUriAsync(new Uri(System.Net.WebUtility.HtmlDecode(item.Link)));
            }
        }
    }
}
