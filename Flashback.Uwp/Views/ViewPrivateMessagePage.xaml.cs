using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Template10.Common;
using Windows.System;

namespace FlashbackUwp.Views
{
    public sealed partial class ViewPrivateMessagePage : Page
    {
        public ViewPrivateMessagePage() => InitializeComponent();

        private async void WebView_OnNewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            var nav = WindowWrapper.Current().NavigationServices.FirstOrDefault();

            if (args.Uri.AbsoluteUri.Contains("https://www.flashback.org/t") || args.Uri.AbsoluteUri.Contains("https://www.flashback.org/sp")) // intern FB-trådlänk eller fb-singlepost
            {
                string id = args.Uri.AbsoluteUri.Replace("https://www.flashback.org/", "");
                
                nav?.Navigate(typeof(ThreadPage), id);
            }
            else if (args.Uri.AbsoluteUri.Contains("https://www.flashback.org/f")) // intern FB-forumlänk
            {
                string id = args.Uri.AbsoluteUri.Replace("https://www.flashback.org/", "");
                
                nav?.Navigate(typeof(ForumMainList), id);
            }
            else if(args.Uri.AbsoluteUri.Contains("http://www.flashback.org/showthread.php?t=")) // gammla FB-standarden, öppna internt
            {
                string id = "t" + args.Uri.AbsoluteUri.Replace("http://www.flashback.org/showthread.php?t=", "");

                nav?.Navigate(typeof(ThreadPage), id);
            }
            else
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Vill du verkligen öppna en länk till extern sida? Tänk på att länkar som öppnas från privata meddelanden kan utnytjas för att spåra dig!" 
                    + Environment.NewLine + Environment.NewLine +
                    "Länken som kommer öppnas är: " + args.Uri.LocalPath, "Bekräfta");

                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Öppna") { Id = 0 });
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Avbryt") { Id = 1 });

                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;

                if ((await dialog.ShowAsync()).Label == "Öppna")
                {
                    await Launcher.LaunchUriAsync(new Uri(System.Net.WebUtility.HtmlDecode(args.Uri.LocalPath)));
                }
            }
        }
    }
}
