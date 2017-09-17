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
using Windows.System;
using Windows.UI.Xaml.Media.Imaging;
using FlashbackUwp.UserControls;

namespace FlashbackUwp.Views
{
    public sealed partial class ThreadPage : Page
    {
        private Services.SettingsServices.SettingsService _settings;

        public ThreadPage()
        {
            InitializeComponent();
            Messenger.Default.Register<string>(this, FlashbackConstants.MessengerBrowserScrollToPost, ScrollToPost);

            DataTransferManager.GetForCurrentView().DataRequested += ThreadPage_DataRequested;
            _settings = Services.SettingsServices.SettingsService.Instance;
        }

        private void ThreadPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (DataContext is ThreadViewModel model)
            {
                var id = model.ForumThread.Id.GetCleanId(false);
                args.Request.Data.SetWebLink(new Uri("https://www.flashback.org/" + id));
                args.Request.Data.Properties.Title = model.ForumThread.Title;
                args.Request.Data.Properties.Description = "Jag hittade en intressant tråd på Flashback att läsa";
            }
        }

        private async void ScrollToPost(string postId)
        {            
            await WebView.InvokeScriptAsync("eval", new string[] { @"
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
                string id = "t" + args.Uri.AbsoluteUri.Replace("http://www.flashback.org/showthread.php?t=", "");

                nav?.Navigate(typeof(ThreadPage), id);
            }
            else if(_settings.UseImageViewer && (args.Uri.AbsoluteUri.ToLower().EndsWith(".jpg") // Öppna bilder i bildvisaren
                                                || args.Uri.AbsoluteUri.ToLower().EndsWith(".png")
                                                || args.Uri.AbsoluteUri.ToLower().EndsWith(".jpeg")
                                                || args.Uri.AbsoluteUri.ToLower().EndsWith(".gif")))
            {
                try
                {
                    var uri = new Uri(args.Uri.LocalPath, UriKind.Absolute);
                    var image = new BitmapImage(uri);

                    var model = new ImageViewerViewModel { Image = image };

                    ImageViewer.DataContext = model;
                    ImageViewer.Visibility = Visibility.Visible;
                    ImageViewer.Tapped += (o, e) =>
                    {
                        ImagePopup.IsOpen = false;
                    };

                    ImagePopup.MaxHeight = Window.Current.Bounds.Height - 100;
                    ImagePopup.MaxWidth = Window.Current.Bounds.Width - 100;
                    ImageViewer.MaxHeight = Window.Current.Bounds.Height - 100;
                    ImageViewer.MaxWidth = Window.Current.Bounds.Width - 100;

                    ImagePopup.IsOpen = true;
                }
                catch (UriFormatException)
                { 
                    Messenger.Default.Send("Länken till bilden är felaktig och går inte att ladda", FlashbackConstants.MessengerShowError);
                    ImagePopup.IsOpen = false;
                }
                catch (Exception)
                { 
                    Messenger.Default.Send("Ett okänt fel inträffade när bilden skulle laddas.", FlashbackConstants.MessengerShowError);
                    ImagePopup.IsOpen = false;
                }
            }
            else
            {
                var openUrl = Launcher.LaunchUriAsync(new Uri(System.Net.WebUtility.HtmlDecode(args.Uri.LocalPath)));                
            } 
        }

        private async void WebViewTop(object sender, RoutedEventArgs e)
        {
            await WebView.InvokeScriptAsync("eval", new string[] { @"window.scrollTo(0,0);" });
        }

        private async void WebViewBottom(object sender, RoutedEventArgs e)
        {
            await WebView.InvokeScriptAsync("eval", new string[] { @"window.scrollTo(0, document.body.scrollHeight);" });
        }

        private async void OpenInWebBrowser(object sender, RoutedEventArgs e)
        {
            if (DataContext is ThreadViewModel model)
            {
                await Launcher.LaunchUriAsync(new Uri("https://www.flashback.org/" + model.ForumThread.Id));
            }
        }

        private void ShowShareTask(object sender, RoutedEventArgs e) => DataTransferManager.ShowShareUI();
    }
}
