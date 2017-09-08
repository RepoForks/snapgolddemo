using System;
using PhotoSharingApp.Frontend.Portable.Services;
namespace PhotoSharingApp.Frontend.Portable.ViewModels
{
    public class CameraViewModel : AsyncViewModelBase
    {
        private IPhotoService photoService;

        public CameraViewModel(IPhotoService photoService)
        {
            this.photoService = photoService;
        }

        public void UploadPhoto()
        {
            //photoService.U
        }
    }
}
