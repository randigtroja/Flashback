using System.ComponentModel;
using System.Linq;
using System;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Flashback.Model;
using GalaSoft.MvvmLight.Messaging;
using Template10.Mvvm;

namespace FlashbackUwp.Views
{
    public sealed partial class Shell : Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;
        Services.SettingsServices.SettingsService _settings;

        public Shell()
        {
            Instance = this;
            InitializeComponent();
            _settings = Services.SettingsServices.SettingsService.Instance;
            
            _settings.RefreshTitleBarColor();

            Messenger.Default.Register<string>(this, FlashbackConstants.MessengerShowWarning, ShowWarning);
            Messenger.Default.Register<string>(this, FlashbackConstants.MessengerShowError, ShowError);
            Messenger.Default.Register<string>(this, FlashbackConstants.MessengerShowInformation, ShowInformation);
        }
        

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
            HamburgerMenu.RefreshStyles(_settings.AppTheme, true);
            HamburgerMenu.IsFullScreen = _settings.IsFullScreen;
            HamburgerMenu.HamburgerButtonVisibility = _settings.ShowHamburgerButton ? Visibility.Visible : Visibility.Collapsed;
        }

        public async void ShowInformation(string message)
        {           
            FlashbackNotifierText.Text = message;
            FlasbackhNotifierGrid.Background = new SolidColorBrush(Colors.Green);
            FlashbackNotifier.IsOpen = true;
            await Task.Delay(2000);
            FlashbackNotifier.IsOpen = false;
        }

        public async void ShowWarning(string message)
        {
            FlashbackNotifierText.Text = message;
            FlasbackhNotifierGrid.Background = new SolidColorBrush(Colors.Orange);
            FlashbackNotifier.IsOpen = true;
            await Task.Delay(3000);
            FlashbackNotifier.IsOpen = false;
        }

        public async void ShowError(string message)
        {
            FlashbackNotifierText.Text = message;
            FlasbackhNotifierGrid.Background = new SolidColorBrush(Colors.Crimson);
            FlashbackNotifier.IsOpen = true;
            await Task.Delay(4000);
            FlashbackNotifier.IsOpen = false;
        }
    }
}
