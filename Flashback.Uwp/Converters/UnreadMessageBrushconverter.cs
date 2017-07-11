using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace FlashbackUwp.Converters
{
    public class UnreadMessageBrushconverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? Application.Current.Resources["SystemControlForegroundAccentBrush"] as SolidColorBrush : Application.Current.Resources["SystemControlForegroundBaseMediumLowBrush"] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
