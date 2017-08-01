using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Template10.Common;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FlashbackUwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewPrivateMessagePage : Page
    {
        public ViewPrivateMessagePage()
        {
            this.InitializeComponent();
        }

        private async void WebView_OnNewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            args.Handled = true;

            if (args.Uri.AbsoluteUri.Contains("https://www.flashback.org/t")) // intern FB-trådlänk
            {
                string id = args.Uri.AbsoluteUri.Replace("https://www.flashback.org/", "");

                var nav = WindowWrapper.Current().NavigationServices.FirstOrDefault();

                nav?.Navigate(typeof(Views.ThreadPage), id);
            }
            else if (args.Uri.AbsoluteUri.Contains("https://www.flashback.org/f")) // intern FB-forumlänk
            {
                string id = args.Uri.AbsoluteUri.Replace("https://www.flashback.org/", "");

                var nav = WindowWrapper.Current().NavigationServices.FirstOrDefault();

                nav?.Navigate(typeof(Views.ForumMainList), id);
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

                var resultDialog = await dialog.ShowAsync();

                if (resultDialog.Label == "Öppna")
                {
                    var openUrl = Windows.System.Launcher.LaunchUriAsync(new Uri(System.Net.WebUtility.HtmlDecode(args.Uri.LocalPath)));
                }


                

            }

            
        }
    }
}
