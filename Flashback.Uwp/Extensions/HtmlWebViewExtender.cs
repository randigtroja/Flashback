using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FlashbackUwp.Extensions
{
    /// <summary>
    /// Så vi kan binda direkt till "html" från vymodellen
    /// </summary>
    public class HtmlWebViewExtender
    {
        public static string GetHTML(DependencyObject obj) => (string)obj.GetValue(HTMLProperty);
        public static void SetHTML(DependencyObject obj, string value) => obj.SetValue(HTMLProperty, value);

        public static readonly DependencyProperty HTMLProperty = DependencyProperty.RegisterAttached("HTML", typeof(string), typeof(HtmlWebViewExtender), new PropertyMetadata("", new PropertyChangedCallback(OnHTMLChanged)));

        private static void OnHTMLChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebView wv)
            {
                wv.NavigateToString((string)e.NewValue);
            }
        }
    }
}
