using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Template10.Common;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FlashbackUwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ThreadPage : Page
    {        
        public ThreadPage()
        {
            this.InitializeComponent();
            Messenger.Default.Register<string>(this, "ScrollToPost", ScrollToPost);
        }

        private async void ScrollToPost(string postId)
        {            
            await this.WebView.InvokeScriptAsync("eval", new string[] { @"
                                                                            var element = document.getElementById('" + postId + "');"+
                                                                            "if(element)" +
                                                                            "{" +
                                                                            "    var bodyRect = document.body.getBoundingClientRect();" +
                                                                            "    var elementRect = element.getBoundingClientRect();" +
                                                                            "    var offset = elementRect.top - bodyRect.top;" +
                                                                            "    window.scrollTo(0,offset);"+
                                                                            "}"
                                                                       });
        }

        private void WebView_OnNewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {            
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
                var openUrl = Windows.System.Launcher.LaunchUriAsync(new Uri(System.Net.WebUtility.HtmlDecode(args.Uri.LocalPath)));
                
            }

            args.Handled = true;
        }

        private async void WebViewTop(object sender, RoutedEventArgs e)
        {
            await this.WebView.InvokeScriptAsync("eval", new string[] { @"window.scrollTo(0,0);" });            
        }

        private async void WebViewBottom(object sender, RoutedEventArgs e)
        {
            await this.WebView.InvokeScriptAsync("eval", new string[] { @"window.scrollTo(0, document.body.scrollHeight);" });   
        }        
    }
}
