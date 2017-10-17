using Windows.UI.Xaml;
using System.Threading.Tasks;
using FlashbackUwp.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using System;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Data;
using Flashback.Model;
using Flashback.Services.Auth;
using FlashbackUwp.Services.SecurityServices;
using GalaSoft.MvvmLight.Messaging;
using Template10.Services.LoggingService;

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
            get => _cookieContainer ?? (_cookieContainer = new CookieContainer());
            set => _cookieContainer = value;
        }

        public static bool IsUserLoggedIn()
        {
            return CookieContainer
                .GetCookies(new Uri("https://flashback.org/"))
                .Cast<Cookie>()
                .FirstOrDefault(x => x.Name == "vbscanuserid") != null;
        }        

        public static string GetUserId()
        {
            var cookies = CookieContainer
                .GetCookies(new Uri("https://flashback.org/"))
                .Cast<Cookie>().ToList();

            return cookies.FirstOrDefault(x => x.Name == "vbscanuserid")?.Value;
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
            return new ModalDialog
            {
                DisableBackButtonWhenModal = true,
                Content = new Views.Shell(NavigationServiceFactory(BackButton.Attach, ExistingContent.Include)),
                ModalContent = new Views.Busy(),
            };
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            await LoadSavedCookies();

            if (DetermineStartCause(args) == AdditionalKinds.SecondaryTile)
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

        public override async void OnResuming(object s, object e, AppExecutionState previousExecutionState)
        {
            await LoadSavedCookies();

            base.OnResuming(s, e, previousExecutionState);
        }

        private async Task LoadSavedCookies()
        {
            var cookies = await new EncryptionService().GetCookieData();

            if (cookies != null && cookies.Any())
            {
                foreach (var cookie in cookies)
                {
                    CookieContainer.Add(new Uri("https://www.flashback.org"), cookie);
                }
            }

            Messenger.Default.Send(IsUserLoggedIn(), FlashbackConstants.MessengerLoggedInStatus);

            await Task.CompletedTask;
        }

        public static async Task SaveCookies()
        {
            await new EncryptionService().WriteCookieData(CookieContainer);

            await Task.CompletedTask;
        }

        public static async Task Logout()
        {
            await new AuthService(CookieContainer).Logout();

            CookieContainer = new CookieContainer();

            await new EncryptionService().WriteCookieData(CookieContainer);

            Messenger.Default.Send(IsUserLoggedIn(), FlashbackConstants.MessengerLoggedInStatus);

            Messenger.Default.Send("Ok! Du är utloggad!", FlashbackConstants.MessengerShowInformation);
        }
    }
}
