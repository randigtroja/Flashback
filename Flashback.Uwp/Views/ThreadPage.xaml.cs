using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Flashback.Model;
using Flashback.Services;
using FlashbackUwp.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Template10.Common;


namespace FlashbackUwp.Views
{
    public sealed partial class ThreadPage : Page
    {        
        public ThreadPage()
        {
            this.InitializeComponent();
            Messenger.Default.Register<string>(this, FlashbackConstants.MessengerBrowserScrollToPost, ScrollToPost);

            DataTransferManager.GetForCurrentView().DataRequested += ThreadPage_DataRequested;
        }

        private void ThreadPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var model = DataContext as ThreadViewModel;
            if (model != null)
            {
                var id = model.ForumThread.Id.GetCleanId(false);
                args.Request.Data.SetWebLink(new Uri("https://www.flashback.org/" + id));
                args.Request.Data.Properties.Title = model.ForumThread.Title;
                args.Request.Data.Properties.Description = "Jag hittade en intressant tråd på Flashback att läsa";
            }
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
            args.Handled = true;
            var nav = WindowWrapper.Current().NavigationServices.FirstOrDefault();

            if (args.Uri.AbsoluteUri.Contains("https://www.flashback.org/t") || args.Uri.AbsoluteUri.Contains("https://www.flashback.org/sp")) // intern FB-trådlänk
            {
                string id = args.Uri.AbsoluteUri.Replace("https://www.flashback.org/", "");
                                
                nav?.Navigate(typeof(ThreadPage), id);                
            }
            else if (args.Uri.AbsoluteUri.Contains("https://www.flashback.org/f")) // intern FB-forumlänk
            {
                string id = args.Uri.AbsoluteUri.Replace("https://www.flashback.org/", "");
                
                nav?.Navigate(typeof(ForumMainList), id);
            }
            else if (args.Uri.AbsoluteUri.Contains("http://www.flashback.org/showthread.php?t=")) // gammla FB-standarden, öppna internt
            {
                var id = args.Uri.AbsoluteUri.Replace("http://www.flashback.org/showthread.php?t=", "");
                id = "t" + id;

                nav?.Navigate(typeof(ThreadPage), id);
            }
            else
            {
                var openUrl = Windows.System.Launcher.LaunchUriAsync(new Uri(System.Net.WebUtility.HtmlDecode(args.Uri.LocalPath)));                
            }            
        }

        private async void WebViewTop(object sender, RoutedEventArgs e)
        {
            await this.WebView.InvokeScriptAsync("eval", new string[] { @"window.scrollTo(0,0);" });            
        }

        private async void WebViewBottom(object sender, RoutedEventArgs e)
        {
            await this.WebView.InvokeScriptAsync("eval", new string[] { @"window.scrollTo(0, document.body.scrollHeight);" });   
        }

        private async void OpenInWebBrowser(object sender, RoutedEventArgs e)
        {
            if (DataContext is ThreadViewModel model)
            {
                var id = model.ForumThread.Id;
                var openUrl = await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.flashback.org/" + id));
            }
        }

        private void ShowShareTask(object sender, RoutedEventArgs e)
        {            
            DataTransferManager.ShowShareUI();
        }
    }
}
