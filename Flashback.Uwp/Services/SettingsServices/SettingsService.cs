using System;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;

namespace FlashbackUwp.Services.SettingsServices
{
    public class SettingsService
    {
        public static SettingsService Instance { get; } = new SettingsService();
        Template10.Services.SettingsService.ISettingsHelper _helper;
        private SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public bool UseShellBackButton
        {
            get { return _helper.Read<bool>(nameof(UseShellBackButton), true); }
            set
            {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.GetDispatcherWrapper().Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                });
            }
        }

        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = ApplicationTheme.Dark;
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
                Views.Shell.HamburgerMenu.RefreshStyles(value, true);

                RefreshTitleBarColor();
            }
        }

        internal void RefreshTitleBarColor()
        {
            // för desktop
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    if (this.AppTheme == ApplicationTheme.Dark)
                    {
                        titleBar.ButtonBackgroundColor = Colors.Black;
                        titleBar.ButtonForegroundColor = Colors.White;
                        titleBar.BackgroundColor = Colors.Black;
                        titleBar.ForegroundColor = Colors.White;
                        titleBar.InactiveBackgroundColor = Colors.Black;
                        titleBar.InactiveForegroundColor = Colors.White;
                    }
                    else
                    {
                        titleBar.ButtonBackgroundColor = Colors.White;
                        titleBar.ButtonForegroundColor = Colors.Black;
                        titleBar.BackgroundColor = Colors.White;
                        titleBar.ForegroundColor = Colors.Black;
                        titleBar.InactiveBackgroundColor = Colors.White;
                        titleBar.InactiveForegroundColor = Colors.Black;
                    }


                }
            }

            // nedan är för mobiltelefoner
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {

                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    if (this.AppTheme == ApplicationTheme.Dark)
                    {
                        statusBar.BackgroundOpacity = 1;
                        statusBar.BackgroundColor = Colors.Black;
                        statusBar.ForegroundColor = Colors.White;
                    }
                    else
                    {
                        statusBar.BackgroundOpacity = 1;
                        statusBar.BackgroundColor = Colors.White;
                        statusBar.ForegroundColor = Colors.Black;
                    }
                }
            }
        }

        public TimeSpan CacheMaxDuration
        {
            get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set
            {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        public bool ShowHamburgerButton
        {
            get { return _helper.Read<bool>(nameof(ShowHamburgerButton), true); }
            set
            {
                _helper.Write(nameof(ShowHamburgerButton), value);
                Views.Shell.HamburgerMenu.HamburgerButtonVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool IsFullScreen
        {
            get { return _helper.Read<bool>(nameof(IsFullScreen), false); }
            set
            {
                _helper.Write(nameof(IsFullScreen), value);
                Views.Shell.HamburgerMenu.IsFullScreen = value;
            }
        }

        public bool ShowAvatars
        {
            get { return _helper.Read<bool>(nameof(ShowAvatars), true); }
            set { _helper.Write(nameof(ShowAvatars), value); }
        }

        public bool HoppaTillSistaSidan
        {
            get { return _helper.Read<bool>(nameof(HoppaTillSistaSidan), false); }
            set { _helper.Write(nameof(HoppaTillSistaSidan), value); }
        }

        public bool VisaBilderITraden
        {
            get { return _helper.Read<bool>(nameof(VisaBilderITraden), true); }
            set { _helper.Write(nameof(VisaBilderITraden), value); }
        }

        public bool KomIhagSidNr
        {
            get { return _helper.Read<bool>(nameof(KomIhagSidNr), true); }
            set { _helper.Write(nameof(KomIhagSidNr), value); }
        }

        public string FontSize
        {
            get { return _helper.Read<string>(nameof(FontSize), "90%"); }
            set { _helper.Write(nameof(FontSize), value); }
        }

        public bool UseEmoticons
        {
            get { return _helper.Read<bool>(nameof(UseEmoticons), true); }
            set { _helper.Write(nameof(UseEmoticons), value); }
        }

    }
}
