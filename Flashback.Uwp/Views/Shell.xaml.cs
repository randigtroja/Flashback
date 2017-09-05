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
            Messenger.Default.Register<int?>(this, FlashbackConstants.MessengerUnreadMessagesCount, UpdateUnreadMessagesCount);
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

            // Vi gömmer undan knappen för menyn om vi kör på telefon och visar sedan upp den i appbaren längst ner
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                Shell.HamburgerMenu.HamburgerButtonVisibility = Visibility.Collapsed;
                Shell.HamburgerMenu.VisualStateWideDisplayMode = SplitViewDisplayMode.Overlay;
                Shell.HamburgerMenu.VisualStateNormalDisplayMode = SplitViewDisplayMode.Overlay;
                Shell.HamburgerMenu.VisualStateNarrowDisplayMode = SplitViewDisplayMode.Overlay;
            }
        }

        public async void ShowInformation(string message)
        {           
            FlashbackNotifierText.Text = message;
            FlasbackhNotifierGrid.Background = new SolidColorBrush(Colors.Green);
            FlashbackNotifier.IsOpen = true;
            await Task.Delay(2000);
            FlashbackNotifier.IsOpen = false;
        }

        private void UpdateUnreadMessagesCount(int? result)
        {
            if (!result.HasValue || result.Value == 0) // Det finns inget oläst, dölj skiten
            {
                PmCount.Visibility = Visibility.Collapsed;
                PmGrid.Visibility = Visibility.Collapsed;
            }
            else // Visa badge och antalet
            {
                PmCount.Visibility = Visibility.Visible;
                PmGrid.Visibility = Visibility.Visible;
                PmCount.Text = result.Value.ToString();
            }
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
