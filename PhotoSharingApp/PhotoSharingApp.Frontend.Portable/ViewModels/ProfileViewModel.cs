using System;
using PhotoSharingApp.Frontend.Portable.Services;
using System.Windows.Input;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAzure.MobileServices;

namespace PhotoSharingApp.Frontend.Portable.ViewModels
{
    public class ProfileViewModel : AsyncViewModelBase
    {
        private IDialogService dialogService;
        private IPhotoService photoService;

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

        public ProfileViewModel(IDialogService dialogService, IPhotoService photoService)
        {
            this.dialogService = dialogService;
            this.photoService = photoService;
        }

        public async Task LoginAsync(MobileServiceAuthenticationProvider provider)
        {
            await photoService.SignInAsync(provider);

            var user = await photoService.GetCurrentUser();
            if (user != null)
            {
                await dialogService.DisplayDialogAsync("Success", "Logged in", "Yuchay");
            }
        }
    }
}
