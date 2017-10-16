using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using PhotoSharingApp.Forms.Controls;
using PhotoSharingApp.Frontend.Portable.ViewModels;
using Plugin.Media;
using Xamarin.Forms;
using Plugin.Media.Abstractions;

namespace PhotoSharingApp.Forms.Pages
{
    public partial class UploadPage : ContentPage
    {
        private CameraViewModel viewModel;
        private PhotoResultEventArgs photoResult;
        private MediaFile pickedPhoto;
        private bool isPickingPhoto;

        public UploadPage(PhotoResultEventArgs photoResult)
        {
            InitializeComponent();
            this.photoResult = photoResult;
            BindingContext = viewModel = SimpleIoc.Default.GetInstance<CameraViewModel>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await CrossMedia.Current.Initialize();

            if (isPickingPhoto)
                return;

            if (photoResult == null)
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await Navigation.PopAsync();
                    return;
                }

                // Pick a file from the local library
                isPickingPhoto = true;
                pickedPhoto = await CrossMedia.Current.PickPhotoAsync();
                isPickingPhoto = false;
                if (pickedPhoto == null)
                {
                    await Navigation.PopAsync();
                    return;
                }

                // Show selected photo in preview
                PhotoPreview.Source = ImageSource.FromFile(pickedPhoto.Path);
            }
            else
            {
                // Show taken photo in preview
                PhotoPreview.Source = ImageSource.FromStream(() => new MemoryStream(photoResult.Image));
            }

            await viewModel.InitAsync();
            CategoryPicker.SelectedIndex = 0;
        }

        async void UploadButton_Clicked(object sender, System.EventArgs e)
        {
            LoadingOverlay.IsVisible = true;
            LoadingAnimation.Play();

            // Upload photo
            var stream = pickedPhoto != null ? pickedPhoto.GetStream() : new MemoryStream(photoResult.Image);
            var success = await viewModel.UploadPhoto(stream, "");
            if (success)
            {
                PhotoPreview.Source = null;
                viewModel.Caption = string.Empty;

                await Navigation.PopAsync();

                // Navigate back to categories page
                App.AppShell.SelectedItem = null; // Hack: Xamarin.Forms Bug does not allow the same navigation twice otherwise
                App.AppShell.SelectedItem = App.AppShell.Children.First();
            }

            LoadingOverlay.IsVisible = false;
            LoadingAnimation.IsPlaying = false;
            LoadingAnimation.Pause();
        }

        async void CancelButton_Clicked(object sender, System.EventArgs e)
        {
            PhotoPreview.Source = null;
            viewModel.Caption = string.Empty;
            await Navigation.PopAsync();
        }
    }
}
