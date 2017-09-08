using System;
using PhotoSharingApp.Frontend.Portable.Services;
using System.Threading.Tasks;
using MvvmHelpers;
using PhotoSharingApp.Frontend.Portable.Models;
using System.Linq;
using System.IO;

namespace PhotoSharingApp.Frontend.Portable.ViewModels
{
    public class CameraViewModel : AsyncViewModelBase
    {
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

        public CameraViewModel(IPhotoService photoService)
        {
            this.photoService = photoService;

            CategoryOptions = new ObservableRangeCollection<Category>();
            SelectedCategory = CategoryOptions.FirstOrDefault();
        }

        public async Task InitAsync()
        {
            var categories = await photoService.GetCategories();
            CategoryOptions.ReplaceRange(categories);
        }

        public async Task UploadPhoto(Stream stream, string filePath)
        {
            if (!string.IsNullOrWhiteSpace(Caption) && SelectedCategory != null)
            {
                await photoService.UploadPhoto(stream, filePath, Caption, SelectedCategory.Id);
            }
        }
    }
}
