using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FlashbackUwp.Views
{
    public sealed partial class ForumMainList : Page
    {
        public ForumMainList()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            //if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            //{
            //    Shell.HamburgerMenu.HamburgerButtonVisibility = Visibility.Collapsed;
            //    Shell.HamburgerMenu.VisualStateWideDisplayMode = SplitViewDisplayMode.Overlay;
            //    Shell.HamburgerMenu.VisualStateNormalDisplayMode = SplitViewDisplayMode.Overlay;
            //    Shell.HamburgerMenu.VisualStateNarrowDisplayMode = SplitViewDisplayMode.Overlay;
            //}
            
        }
    }
}
