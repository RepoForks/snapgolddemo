using System;
using PhotoSharingApp.Frontend.Portable.Models;
using System.Linq;
using System.Threading.Tasks;
using PhotoSharingApp.Frontend.Portable.Services;

namespace PhotoSharingApp.Frontend.Portable
{
    public class PhotoDetailsViewModel : AsyncViewModelBase
    {
        private IPhotoService photoService;

        private Photo photo;
        public Photo Photo
        {
            get { return photo; }
            set { photo = value; RaisePropertyChanged(); }
        }

        public PhotoDetailsViewModel(IPhotoService photoService)
        {
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

            IsRefreshing = false;
        }
    }
}
