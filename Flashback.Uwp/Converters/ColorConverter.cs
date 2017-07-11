using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace FlashbackUwp.Converters
{
    public class ColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var linkName = value.ToString();

            if (linkName.Trim() == "Dator och IT")
            {
                var c = new Color { R = 123, G = 152, B = 175, A = 255 };
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Fordon & trafik")
            {
                var c = new Color { R = 234, G = 167, B = 92, A = 255 };
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Hem, bostad & familj")
            {
                var c = new Color { R = 193, G = 0, B = 0, A = 255 };
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Ekonomi")
            {
                var c = new Color { R = 0, G = 125, B = 54, A = 255 };
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Kultur & Media")
            {
                var c = new Color { R = 161, G = 37, B = 83, A = 255 };
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Livsstil")
            {
                var c = new Color { R = 129, G = 79, B = 134, A = 255 };
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Mat, dryck & tobak")
            {
                var c = new Color { R = 199, G = 88, B = 88, A = 255 };
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Politik")
            {
                var c = new Color { R = 165, G = 89, B = 10, A = 255 };
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Resor")
            {
                var c = new Color { R = 0, G = 104, B = 158, A = 255 };
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Samhälle")
            {
                var c = new Color { R = 255, G = 127, B = 0, A = 255 };
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Sport & träning")
            {
                var c = new Color { R = 105, G = 12, B = 7, A = 255 };
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Vetenskap & humaniora")
            {
                var c = new Color { R = 241, G = 164, B = 0, A = 255 };
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Övrigt")
            {
                var c = new Color { R = 153, G = 153, B = 153, A = 255 };
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Om Flashback")
            {
                var c = new Color { R = 25, G = 153, B = 0, A = 255 };
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Sex")
            {
                var c = new Color {R = 255, G = 30, B = 123, A = 255};
                return new SolidColorBrush(c);
            }
            else if (linkName.Trim() == "Droger")
            {
                var c = new Color { R = 113, G = 156, B = 121, A = 255 };
                return new SolidColorBrush(c);
            }
            else
            {
                var c = new Color { R = 255, G = 46, B = 18, A = 255 };
                return new SolidColorBrush(c);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
