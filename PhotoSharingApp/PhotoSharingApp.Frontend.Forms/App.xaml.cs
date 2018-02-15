using System.Collections.Generic;
using DLToolkit.Forms.Controls;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Forms.Pages;
using PhotoSharingApp.Forms.Services;
using PhotoSharingApp.Frontend.Portable;
using PhotoSharingApp.Frontend.Portable.Abstractions;
using PhotoSharingApp.Frontend.Portable.Models;
using PhotoSharingApp.Frontend.Portable.Services;
using PhotoSharingApp.Frontend.Portable.ViewModels;
using Plugin.VersionTracking;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using IDialogService = PhotoSharingApp.Frontend.Portable.Abstractions.IDialogService;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using PhotoSharingApp.Forms.Abstractions;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace PhotoSharingApp.Forms
{
    public partial class App : Xamarin.Forms.Application
    {

        public static AppShell AppShell;

        public App()
        {
            InitializeComponent();
        }

        public App(IAuthenticationHandler authenticationHandler = null)
        {
            InitializeComponent();
            FlowListView.Init();

            // OOP Demo Comment

            // Setup IoC Container for Dependeny Injection
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Reset();

            // Register Dependencies
            if (authenticationHandler != null)
                SimpleIoc.Default.Register<IAuthenticationHandler>(() => authenticationHandler);
            SimpleIoc.Default.Register<IAppEnvironment, AppEnvironment>();
            SimpleIoc.Default.Register<IPhotoCroppingService>(() => DependencyService.Get<IPhotoCroppingService>());
            SimpleIoc.Default.Register<IPhotoService, ServiceClient>();
            SimpleIoc.Default.Register<IDialogService, FormsDialogService>();
            SimpleIoc.Default.Register<IConnectivityService, FormsConnectivityService>();
            SimpleIoc.Default.Register<ISettingsService, FormsSettingsService>();

            SimpleIoc.Default.Register<CategoriesViewModel>();
            SimpleIoc.Default.Register<PhotoDetailsViewModel>();
            SimpleIoc.Default.Register<StreamPageViewModel>();
            SimpleIoc.Default.Register<CameraViewModel>();
            SimpleIoc.Default.Register<ProfileViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();

            // Setup App Container
            var navigationPage = new Xamarin.Forms.NavigationPage();
            navigationPage.BarBackgroundColor = (Color)Resources["AccentColor"];
            navigationPage.BarTextColor = Color.White;

            // Register Navigation Service
            var navigationService = new FormsNavigationService(navigationPage);
            navigationService.Configure(ViewNames.PhotoDetailsPage, typeof(PhotoDetailsPage));
            navigationService.Configure(ViewNames.StreamPage, typeof(StreamPage));
            navigationService.Configure(ViewNames.SettingsPage, typeof(SettingsPage));
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);

            // Setup App Shell
            AppShell = new AppShell();
            AppShell.Children.Add(new CategoriesPage());   // Home
            AppShell.Children.Add(new CameraPage());       // Upload
            AppShell.Children.Add(new LeaderboardsPage()); // Leaderboards
            AppShell.Children.Add(new ProfilePage());      // My profile
            AppShell.On<Android>().DisableSwipePaging();

            navigationPage.PushAsync(AppShell);
            MainPage = navigationPage;
        }

        protected override void OnStart()
        {
            CrossVersionTracking.Current.Track();

            var appCenterKey =
                "ios=7e7901fb-6317-46d8-8a33-7cb200424c11;" +
                "android=60d30fa4-683b-4d74-aff1-434e694999e2;";

            var environment = DependencyService.Get<IEnvironmentService>();
            if (environment?.IsRunningInRealWorld() == true)
            {
                AppCenter.Start(appCenterKey, typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push));
            }
            else
            {
                // When running in Test Cloud, Debug Mode or on an Emulator / Simulator, do not kick off
                // Analytics, Crash Reports and Distribution
                AppCenter.Start(appCenterKey, typeof(Push));
            }

            // Stop collecting Analytics, when opted-out by the user
            var settings = SimpleIoc.Default.GetInstance<ISettingsService>();
            if (settings?.IsAnalyticsAllowed == false)
            {
                Analytics.SetEnabledAsync(false);
                Crashes.SetEnabledAsync(false);
            }

            Push.PushNotificationReceived += Push_PushNotificationReceived;
            Analytics.TrackEvent("App Started", new Dictionary<string, string>
            {
                { "Day of week", System.DateTime.Now.ToString("dddd") }
            });
        }

        async void Push_PushNotificationReceived(object sender, PushNotificationReceivedEventArgs e)
        {
            if (e.Title != null || e.Message != null)
            {
                var dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
                await dialogService?.DisplayDialogAsync(e.Title, e.Message, "Ok");
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
