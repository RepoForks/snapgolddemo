using System;
using PhotoSharingApp.Frontend.Portable.Services;
using System.Threading.Tasks;
using MvvmHelpers;
using PhotoSharingApp.Frontend.Portable.Models;
using System.Linq;
using System.IO;
using PhotoSharingApp.Frontend.Portable.Exceptions;
using PhotoSharingApp.Frontend.Portable.Abstractions;

namespace PhotoSharingApp.Frontend.Portable.ViewModels
{
    public class CameraViewModel : AsyncViewModelBase
    {
        private IDialogService dialogService;
        private IPhotoService photoService;

        private string caption;
        public string Caption
        {
            get { return caption; }
            set { caption = value; RaisePropertyChanged(); }
        }

        private ObservableRangeCollection<Category> categoryOptions;
        public ObservableRangeCollection<Category> CategoryOptions
        {
            get { return categoryOptions; }
            set { categoryOptions = value; RaisePropertyChanged(); }
        }

        private Category selectedCategory;
        public Category SelectedCategory
        {
            get { return selectedCategory; }
            set { selectedCategory = value; RaisePropertyChanged(); }
        }

        public CameraViewModel(IDialogService dialogService, IPhotoService photoService)
        {
            this.dialogService = dialogService;
            this.photoService = photoService;

            CategoryOptions = new ObservableRangeCollection<Category>();
            SelectedCategory = CategoryOptions.FirstOrDefault();
        }

        public async Task InitAsync()
        {
            var categories = await photoService.GetCategories();
            CategoryOptions.ReplaceRange(categories);
        }

        /// <summary>
        /// Uploads the photo.
        /// </summary>
        /// <returns>Success</returns>
        /// <param name="stream">Stream.</param>
        /// <param name="filePath">File path.</param>
        public async Task<bool> UploadPhoto(Stream stream, string filePath)
        {
            // Check if mandatory fields are set
            if (string.IsNullOrWhiteSpace(Caption) || SelectedCategory == null)
            {
                await dialogService.DisplayDialogAsync("Missing inputs", "Please define category and caption.", "Ok");
                return false;
            }

            try
            {
                // Check if user is logged in
                await photoService.GetCurrentUser();

                // Upload photo
                await photoService.UploadPhoto(stream, filePath, Caption, SelectedCategory.Id);

                // Notify user
                await dialogService.DisplayDialogAsync("Upload successful", "", "Ok");
                return true;
            }
            catch (UnauthorizedException)
            {
                await dialogService.DisplayDialogAsync("Not logged in!", "You need to be logged in to upload a photo. Please log in first.", "Ok");
                return false;
            }
        }
    }
}
