using DLToolkit.Forms.Controls;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
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

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace PhotoSharingApp.Forms
{
    public partial class App : Application
    {
        public static AppShell AppShell;

        public App()
        {
            InitializeComponent();
        }

        public App(IAuthenticationHandler authenticationHandler)
        {
            InitializeComponent();
            FlowListView.Init();

            // Setup IoC Container for Dependeny Injection
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Reset();

            // Register Dependencies
            SimpleIoc.Default.Register<IAppEnvironment, AppEnvironment>();
            SimpleIoc.Default.Register<IAuthenticationHandler>(() => authenticationHandler);
            SimpleIoc.Default.Register<IPhotoCroppingService>(() => DependencyService.Get<IPhotoCroppingService>());
            SimpleIoc.Default.Register<IPhotoService, ServiceClient>();
            SimpleIoc.Default.Register<IDialogService, FormsDialogService>();

            SimpleIoc.Default.Register<CategoriesViewModel>();
            SimpleIoc.Default.Register<PhotoDetailsViewModel>();
            SimpleIoc.Default.Register<StreamPageViewModel>();
            SimpleIoc.Default.Register<CameraViewModel>();
            SimpleIoc.Default.Register<ProfileViewModel>();

            // Setup App Container
            var navigationPage = new NavigationPage();
            navigationPage.BarBackgroundColor = (Color)Resources["AccentColor"];
            navigationPage.BarTextColor = Color.White;

            // Register Navigation Service
            var navigationService = new FormsNavigationService(navigationPage);
            navigationService.Configure(ViewNames.PhotoDetailsPage, typeof(PhotoDetailsPage));
            navigationService.Configure(ViewNames.StreamPage, typeof(StreamPage));
            navigationService.Configure(ViewNames.SettingsPage, typeof(SettingsPage));
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);

            AppShell = new AppShell();
            AppShell.Children.Add(new CategoriesPage());   // Home
            AppShell.Children.Add(new CameraPage());       // Upload
            AppShell.Children.Add(new LeaderboardsPage()); // Leaderboards
            AppShell.Children.Add(new ProfilePage());      // My profile

            navigationPage.PushAsync(AppShell);
            MainPage = navigationPage;
        }

        protected override void OnStart()
        {
            CrossVersionTracking.Current.Track();

            // Visual Studio Mobile Center
            MobileCenter.Start(
                "ios=7e7901fb-6317-46d8-8a33-7cb200424c11;" +
                "android=60d30fa4-683b-4d74-aff1-434e694999e2;",
                typeof(Analytics),
                typeof(Crashes));
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
