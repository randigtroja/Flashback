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

        private DelegateCommand<FbFavourite> _deleteItem = default(DelegateCommand<FbFavourite>);
        public DelegateCommand<FbFavourite> DeleteItem => _deleteItem ?? (_deleteItem = new DelegateCommand<FbFavourite>(ExecuteDeleteItemCommand, CanExecuteDeleteItemCommand));
        
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
            get => _favourites;
            set => Set(ref _favourites, value);
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

        private bool CanExecuteDeleteItemCommand(FbFavourite item)
        {
            return true;
        }

        private async void ExecuteDeleteItemCommand(FbFavourite item)
        {
            try
            {
                Error = null;

                var result = await _threadsService.RemoveFavourite(item);

                if (result)
                {
                    this.Favourites.Remove(item);
                    Messenger.Default.Send<string>(item.Name + " är borttagen från favoriterna!", FlashbackConstants.MessengerShowInformation);
                }
            }
            catch (Exception e)
            {
                Error = e.ToString();
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

        public void NavigateToThread(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is FbItem item)
            {
                NavigationService.Navigate(typeof(ThreadPage), item.Id + (_settings.HoppaTillSistaSidan ? "s" : ""));
            }
        }
    }
}
