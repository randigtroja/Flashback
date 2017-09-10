using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FlashbackUwp.Views
{
    public sealed partial class SettingsPage : Page
    {
        Template10.Services.SerializationService.ISerializationService _SerializationService;

        public SettingsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            _SerializationService = Template10.Services.SerializationService.SerializationService.Json;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MyPivot.SelectedIndex = int.Parse(_SerializationService.Deserialize(e.Parameter?.ToString()).ToString());
        }
    }
}
