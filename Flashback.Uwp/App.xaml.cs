using Windows.UI.Xaml;
using System.Threading.Tasks;
using FlashbackUwp.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Flashback.Services.Auth;
using FlashbackUwp.Services.SecurityServices;
using GalaSoft.MvvmLight.Messaging;
using Template10.Services.LoggingService;
using Template10.Services.SerializationService;

namespace FlashbackUwp
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : BootStrapper
    {
        private static CookieContainer _cookieContainer = null;        

        public static CookieContainer CookieContainer
        {
            get { return _cookieContainer ?? (_cookieContainer = new CookieContainer()); }
            set
            {
                _cookieContainer = value;
            }
        }

        public static bool IsUserLoggedIn()
        {
            var userId = CookieContainer
                .GetCookies(new Uri("https://flashback.org/"))
                .Cast<Cookie>()
                .FirstOrDefault(x => x.Name == "vbscanuserid");

            return userId != null;
        }

        private static string GetSessionhash()
        {
            var cookies = CookieContainer
                .GetCookies(new Uri("https://flashback.org/"))
                .Cast<Cookie>().ToList();

            var hash = cookies.FirstOrDefault(x => x.Name == "vbscansessionhash");

            return hash?.Value;
        }

        public static string GetUserId()
        {
            var cookies = CookieContainer
                .GetCookies(new Uri("https://flashback.org/"))
                .Cast<Cookie>().ToList();

            var hash = cookies.FirstOrDefault(x => x.Name == "vbscanuserid");

            return hash?.Value;
        }


        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);            

            #region app settings

            // some settings must be set in app.constructor
            var settings = SettingsService.Instance;
            RequestedTheme = settings.AppTheme;
            CacheMaxDuration = settings.CacheMaxDuration;
            ShowShellBackButton = settings.UseShellBackButton;

            #endregion

            //LoggingService.WriteLine = new DebugWriteDelegate(LogHandler);
        }

        public void LogHandler(string text = null, Severities severity = Severities.Info, Targets target = Targets.Debug, [CallerMemberName]string caller = null)
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now.TimeOfDay.ToString()} {severity} {caller} {text}");
        }

        public override UIElement CreateRootElement(IActivatedEventArgs e)
        {
            var service = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
            return new ModalDialog
            {
                DisableBackButtonWhenModal = true,
                Content = new Views.Shell(service),
                ModalContent = new Views.Busy(),
            };
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            EncryptionService encryptionService = new EncryptionService();
            
            var cookies = await encryptionService.GetCookieData();
            if (cookies != null && cookies.Any())
            {
                foreach (var cookie in cookies)
                {
                    CookieContainer.Add(new Uri("https://www.flashback.org"), cookie);
                }
            }            

            Messenger.Default.Send<bool>(IsUserLoggedIn(), "LoggedInStatus");
            
            AdditionalKinds cause = DetermineStartCause(args);
            if (cause == AdditionalKinds.SecondaryTile)
            {
                LaunchActivatedEventArgs eventArgs = args as LaunchActivatedEventArgs;
                if (eventArgs.Arguments.StartsWith("t"))
                {
                    NavigationService.Navigate(typeof(Views.ThreadPage),eventArgs.Arguments);
                }
                else if (eventArgs.Arguments.StartsWith("f")) // går ännu ej att pinna forum. Ska vi kunna det?
                {
                    NavigationService.Navigate(typeof(Views.ForumMainList), eventArgs.Arguments);
                }
                else
                {
                    // fuck up, här ska vi nog inte kunna hamna
                    NavigationService.Navigate(typeof(Views.ForumMainList));
                }               
            }
            else
            {
                NavigationService.Navigate(typeof(Views.ForumMainList));
            }            
        }

        public override async Task OnSuspendingAsync(object s, SuspendingEventArgs e, bool prelaunchActivated)
        {                          
            var encryptionService = new EncryptionService();           
            var saveOk = await encryptionService.WriteCookieData(CookieContainer);

            Debug.WriteLine("Status vid att försöka spara ner " + CookieContainer.Count + " st cookies:" + saveOk);
            
            await base.OnSuspendingAsync(s, e, prelaunchActivated);
            await Task.CompletedTask;
        }

        public static async Task Logout()
        {           
            await new AuthService(CookieContainer).Logout(GetSessionhash());
            CookieContainer = new CookieContainer();

            var encryptionService = new EncryptionService();
            await encryptionService.WriteCookieData(CookieContainer);

            Messenger.Default.Send<bool>(IsUserLoggedIn(), "LoggedInStatus");
        }
    }
}
