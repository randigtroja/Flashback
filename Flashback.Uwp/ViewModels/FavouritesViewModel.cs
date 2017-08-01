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
using GalaSoft.MvvmLight.Messaging;
using Template10.Mvvm;

namespace FlashbackUwp.ViewModels
{
    public class FavouritesViewModel:FlashbackViewModelBase
    {
        private readonly ThreadsService _threadsService;
        private ObservableCollection<FbFavourite> _favourites;
        private readonly SettingsService _settings;

        DelegateCommand _removeFavouriteCommand;
        public DelegateCommand RemoveFavouriteCommand => _removeFavouriteCommand ?? (_removeFavouriteCommand = new DelegateCommand(async () => await RemoveFavourite()));

        public bool IsDataLoaded => Favourites != null && Favourites.Any();

        public FavouritesViewModel()
        {
            _threadsService = new ThreadsService(App.CookieContainer, null);
            _favourites = new ObservableCollection<FbFavourite>();
            _settings = SettingsService.Instance;

            Messenger.Default.Register<bool>(this, FlashbackConstants.MessengerFavoritesUpdated, (result) => Refresh());

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Favourites = SampleData.SampleData.GetDefaultFavourites();
            }
        }

        public ObservableCollection<FbFavourite> Favourites
        {
            get
            {
                return _favourites;
            }
            set
            {
                Set(ref _favourites, value);
            }
        }

        public async Task LoadViewModel()
        {
            try
            {                
                Busy.SetBusy(true, "Laddar dina favoriter...");
                Error = null;

                var threads = await _threadsService.GetFavourites();

                Favourites = new ObservableCollection<FbFavourite>(threads);
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

        private async Task RemoveFavourite()
        {
            
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
                NavigationService.Navigate(typeof(ThreadPage), item.Id + (_settings.HoppaTillSistaSidan ? "s" : ""));

            }
        }
    }
}
