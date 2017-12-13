using System.Collections.ObjectModel;
using PhotoSharingApp.Frontend.Portable.Models;

namespace PhotoSharingApp.Frontend.Portable.ViewModels
{
    public class SettingsViewModel : AsyncViewModelBase
    {
        private ObservableCollection<ThirdPartyLibrary> thirdPartyLibraries;
        public ObservableCollection<ThirdPartyLibrary> ThirdPartyLibraries
        {
            get { return thirdPartyLibraries; }
            set { thirdPartyLibraries = value; RaisePropertyChanged(); }
        }

        public SettingsViewModel()
        {
            ThirdPartyLibraries = new ObservableCollection<ThirdPartyLibrary>
            {
                new ThirdPartyLibrary("MVVM Light", "Laurent Bugnion", "http://www.mvvmlight.net", Platform.Xamarin),
                new ThirdPartyLibrary("MVVM Helpers", "James Montemagno", "https://github.com/jamesmontemagno/mvvm-helpers", Platform.Xamarin),
                new ThirdPartyLibrary("CarouselView control for Xamarin Forms", "Alexander Reyes", "https://github.com/alexrainman/CarouselView", Platform.Xamarin),
                new ThirdPartyLibrary("Media Plugin", "James Montemagno", "https://github.com/jamesmontemagno/GeolocatorPlugin", Platform.Xamarin),
                new ThirdPartyLibrary("Connectivity Plugin", "James Montemagno", "https://github.com/jamesmontemagno/ConnectivityPlugin", Platform.Xamarin),
                new ThirdPartyLibrary("Version Tracking Plugin", "Colby L. Williams", "https://github.com/colbylwilliams/VersionTrackingPlugin", Platform.Xamarin),
                new ThirdPartyLibrary("FFImageLoading", "Daniel Luberda", "https://github.com/luberda-molinet/FFImageLoading", Platform.Xamarin),
                new ThirdPartyLibrary("Lottie", "AirBnB, Martijn van Dijk", "https://github.com/martijn00/LottieXamarin", Platform.Xamarin),
                new ThirdPartyLibrary("ACR User Dialogs", "Allan Ritchie", "https://github.com/aritchie/userdialogs", Platform.Xamarin),
                new ThirdPartyLibrary("Icons", "Icons8", "https://icons8.com/", Platform.Xamarin)
            };
        }
    }
}