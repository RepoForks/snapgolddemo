using System;
using PhotoSharingApp.Frontend.Portable.Models;
using System.Linq;
using System.Threading.Tasks;
using PhotoSharingApp.Frontend.Portable.Services;
using PhotoSharingApp.Frontend.Portable.Exceptions;
using GalaSoft.MvvmLight.Command;
using PhotoSharingApp.Frontend.Portable.Abstractions;
using GalaSoft.MvvmLight.Views;
using IDialogService = PhotoSharingApp.Frontend.Portable.Abstractions.IDialogService;

namespace PhotoSharingApp.Frontend.Portable
{
    public class PhotoDetailsViewModel : AsyncViewModelBase
    {
        private INavigationService navigationService;
        private IDialogService dialogService;
        private IPhotoService photoService;

        private Photo photo;
        public Photo Photo
        {
            get { return photo; }
            set { photo = value; RaisePropertyChanged(); }
        }

        private bool isCurrentUsersPhoto;
        public bool IsCurrentUsersPhoto
        {
            get { return isCurrentUsersPhoto; }
            set { isCurrentUsersPhoto = value; RaisePropertyChanged(); }
        }

        private RelayCommand deletePhotoCommand;
        public RelayCommand DeletePhotoCommand
        {
            get
            {
                return deletePhotoCommand ?? (deletePhotoCommand = new RelayCommand(async () =>
                {
                    await DeletePhotoAsync();
                }));
            }
        }

        private RelayCommand setAsProfilePictureCommand;
        public RelayCommand SetAsProfilePictureCommand
        {
            get
            {
                return setAsProfilePictureCommand ?? (setAsProfilePictureCommand = new RelayCommand(async () =>
                {
                    await SetAsProfilePictureAsync();
                }));
            }
        }


        public PhotoDetailsViewModel(INavigationService navigationService, IDialogService dialogService, IPhotoService photoService)
        {
            this.navigationService = navigationService;
            this.dialogService = dialogService;
            this.photoService = photoService;

            // Design
            Photo = new Photo
            {
                StandardUrl = "https://canaryappstorage.blob.core.windows.net/dummy-container/food1.jpg",
                User = new User { ProfilePictureUrl = "https://canaryappstorage.blob.core.windows.net/dummy-container/a1_tn.jpg" },
                Caption = "Oh, look at this",
                CreatedAt = DateTime.Now,
                Annotations = new System.Collections.ObjectModel.ObservableCollection<Annotation>()
                {
                    new Annotation { Text = "Cool", GoldCount = 10 },
                    new Annotation { Text = "Wow, looks fantastic!", GoldCount = 10 }
                }
            };
        }

        public async Task RefreshAsync()
        {
            IsRefreshing = true;

            // Update photo with detailed information
            Photo = await photoService.GetPhotoDetails(Photo.Id);

            // Check if photo is a current user's one
            try
            {
                var currentUser = await photoService.GetCurrentUser();
                if (currentUser.UserId == Photo.User.UserId)
                    IsCurrentUsersPhoto = true;
                else
                    IsCurrentUsersPhoto = false;
            }
            catch (UnauthorizedException)
            {
                IsCurrentUsersPhoto = false;
            }

            IsRefreshing = false;
        }

        public async Task DeletePhotoAsync()
        {
            try
            {
                await photoService.DeletePhoto(Photo);
                navigationService.GoBack();
            }
            catch (Exception ex)
            {
                await dialogService.DisplayDialogAsync("Failed to delete", "Could not delete photo", "Ok");
            }
        }

        public async Task SetAsProfilePictureAsync()
        {
            try
            {
                await photoService.GetCurrentUser();
                await photoService.UpdateUserProfilePhoto(Photo);
                await dialogService.DisplayDialogAsync("Profile Picture updated", "", "Ok");
            }
            catch (Exception ex)
            {
                await dialogService.DisplayDialogAsync("Failed to delete", "Could not delete photo", "Ok");
            }
        }
    }
}
