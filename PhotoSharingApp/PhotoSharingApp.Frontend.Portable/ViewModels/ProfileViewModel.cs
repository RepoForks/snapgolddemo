using System;
using PhotoSharingApp.Frontend.Portable.Services;
using System.Windows.Input;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAzure.MobileServices;
using PhotoSharingApp.Frontend.Portable.Models;
using PhotoSharingApp.Frontend.Portable.Exceptions;
using PhotoSharingApp.Frontend.Portable.Abstractions;
using MvvmHelpers;
using GalaSoft.MvvmLight.Views;
using IDialogService = PhotoSharingApp.Frontend.Portable.Abstractions.IDialogService;

namespace PhotoSharingApp.Frontend.Portable.ViewModels
{
    public class ProfileViewModel : AsyncViewModelBase
    {
        private IDialogService dialogService;
        private INavigationService navigationService;
        private IPhotoService photoService;

        private bool isLoggedIn;
        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set { isLoggedIn = value; RaisePropertyChanged(); }
        }

        private string profilePictureUrl;
        public string ProfilePictureUrl
        {
            get { return profilePictureUrl; }
            set { profilePictureUrl = value; RaisePropertyChanged(); }
        }

        private int numberOfPhotos;
        public int NumberOfPhotos
        {
            get { return numberOfPhotos; }
            set { numberOfPhotos = value; RaisePropertyChanged(); }
        }

        private int userGold;
        public int UserGold
        {
            get { return userGold; }
            set { userGold = value; RaisePropertyChanged(); }
        }

        private ObservableRangeCollection<Photo> userPhotos;
        public ObservableRangeCollection<Photo> UserPhotos
        {
            get { return userPhotos; }
            set { userPhotos = value; RaisePropertyChanged(); }
        }

        private RelayCommand facebookLoginCommand;
        public RelayCommand FacebookLoginCommand
        {
            get
            {
                return facebookLoginCommand ?? (facebookLoginCommand = new RelayCommand(async () =>
                {
                    await LoginAsync(MobileServiceAuthenticationProvider.Facebook);
                }));
            }
        }

        private RelayCommand logoutCommand;
        public RelayCommand LogoutCommand
        {
            get
            {
                return logoutCommand ?? (logoutCommand = new RelayCommand(async () =>
                {
                    if (await dialogService.DisplayDialogAsync("Log out", "Do you really want to sign out and delete all local user data?", "Log out", "Cancel"))
                        await LogoutAsync();
                }));
            }
        }

        private RelayCommand<Photo> navigateToPhotoCommand;
        public RelayCommand<Photo> NavigateToPhotoCommand
        {
            get
            {
                return navigateToPhotoCommand ?? (navigateToPhotoCommand = new RelayCommand<Photo>((Photo photo) =>
                {
                    navigationService.NavigateTo(ViewNames.PhotoDetailsPage, photo);
                }));
            }
        }

        private RelayCommand navigateToSettingsCommand;
        public RelayCommand NavigateToSettingsCommand
        {
            get
            {
                return navigateToSettingsCommand ?? (navigateToSettingsCommand = new RelayCommand(() =>
                {
                    navigationService.NavigateTo(ViewNames.SettingsPage);
                }));
            }
        }

        public ProfileViewModel(IDialogService dialogService, INavigationService navigationService, IPhotoService photoService)
        {
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.photoService = photoService;

            UserPhotos = new ObservableRangeCollection<Photo>();
        }

        public async Task InitAsync()
        {
            try
            {
                var currentUser = await photoService.GetCurrentUser();
                IsLoggedIn = true;
                await SetUpUser(currentUser);
            }
            catch (UnauthorizedException)
            {
                IsLoggedIn = false;
            }
        }

        private async Task SetUpUser(User user)
        {
            ProfilePictureUrl = user.ProfilePictureUrl;
            UserGold = user.GoldBalance;

            var photos = await photoService.GetPhotosForCurrentUser();
            NumberOfPhotos = photos.Items.Count;
            UserPhotos.ReplaceRange(photos.Items);
        }

        public async Task LoginAsync(MobileServiceAuthenticationProvider provider)
        {
            await photoService.SignInAsync(provider);

            var currentUser = await photoService.GetCurrentUser();
            if (currentUser != null)
            {
                IsLoggedIn = true;
                await SetUpUser(currentUser);
            }
        }

        public async Task LogoutAsync()
        {
            await photoService.SignOutAsync();
            IsLoggedIn = false;
        }
    }
}
