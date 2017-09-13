using System;
using PhotoSharingApp.Frontend.Portable.Services;
using System.Windows.Input;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAzure.MobileServices;
using PhotoSharingApp.Frontend.Portable.Models;
using PhotoSharingApp.Frontend.Portable.Exceptions;
using PhotoSharingApp.Frontend.Portable.Abstractions;

namespace PhotoSharingApp.Frontend.Portable.ViewModels
{
    public class ProfileViewModel : AsyncViewModelBase
    {
        private IDialogService dialogService;
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
                    await LogoutAsync();
                }));
            }
        }

        public ProfileViewModel(IDialogService dialogService, IPhotoService photoService)
        {
            this.dialogService = dialogService;
            this.photoService = photoService;
        }

        public async Task InitAsync()
        {
            try
            {
                var currentUser = await photoService.GetCurrentUser();
                IsLoggedIn = true;
                SetUpUser(currentUser);
            }
            catch (UnauthorizedException)
            {
                IsLoggedIn = false;
            }
        }

        private void SetUpUser(User user)
        {
            ProfilePictureUrl = user.ProfilePictureUrl;
        }

        public async Task LoginAsync(MobileServiceAuthenticationProvider provider)
        {
            await photoService.SignInAsync(provider);

            var currentUser = await photoService.GetCurrentUser();
            if (currentUser != null)
            {
                IsLoggedIn = true;
                SetUpUser(currentUser);
            }
        }

        public async Task LogoutAsync()
        {
            await photoService.SignOutAsync();
            IsLoggedIn = false;
        }
    }
}
