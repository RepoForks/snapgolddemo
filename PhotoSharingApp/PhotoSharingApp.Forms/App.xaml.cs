using DLToolkit.Forms.Controls;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Frontend.Portable;
using PhotoSharingApp.Frontend.Portable.Models;
using PhotoSharingApp.Frontend.Portable.Services;
using PhotoSharingApp.Frontend.Portable.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PhotoSharingApp.Frontend.Portable.Abstractions;
using PhotoSharingApp.Forms.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace PhotoSharingApp.Forms
{
    public partial class App : Application
    {
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
            SimpleIoc.Default.Register<IPhotoService, ServiceClient>();
            SimpleIoc.Default.Register<Frontend.Portable.Services.IDialogService, FormsDialogService>();

            SimpleIoc.Default.Register<CategoriesViewModel>();
            SimpleIoc.Default.Register<PhotoDetailsViewModel>();
            SimpleIoc.Default.Register<StreamPageViewModel>();
            SimpleIoc.Default.Register<CameraViewModel>();
            SimpleIoc.Default.Register<ProfileViewModel>();

            // Setup App Container
            var navigationPage = new NavigationPage();
            navigationPage.BarBackgroundColor = (Color)Resources["AccentColor"];
            navigationPage.BarTextColor = Color.Black;

            // Register Navigation Service
            var navigationService = new FormsNavigationService(navigationPage);
            navigationService.Configure(ViewNames.PhotoDetailsPage, typeof(PhotoDetailsPage));
            navigationService.Configure(ViewNames.StreamPage, typeof(StreamPage));
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);

            var appShell = new AppShell();
            appShell.Children.Add(new CategoriesPage());   // Home
            appShell.Children.Add(new CameraPage());       // Upload
            appShell.Children.Add(new LeaderboardsPage()); // Leaderboards
            appShell.Children.Add(new ProfilePage());      // My profile

            navigationPage.PushAsync(appShell);
            MainPage = navigationPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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
