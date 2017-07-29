using Windows.UI.Xaml;

namespace FlashbackUwp.Triggers
{
    public class DeviceStateTrigger : StateTriggerBase
    {
        private string _deviceFamily;

        public string DeviceFamily
        {
            get { return _deviceFamily; }
            set
            {
                _deviceFamily = value;
                SetActive(_deviceFamily == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily);
            }
        }
    }
}
