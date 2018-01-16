using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.AppCenter.Analytics;
using PhotoSharingApp.Frontend.Portable.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace PhotoSharingApp.Forms
{
    public partial class CameraPage : ContentPage
    {
        private CameraViewModel viewModel;
        private MediaFile file;

        public CameraPage()
        {
            InitializeComponent();
            BindingContext = viewModel = SimpleIoc.Default.GetInstance<CameraViewModel>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CrossMedia.Current.Initialize();
            await viewModel.InitAsync();
            CategoryPicker.SelectedIndex = 0;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //file?.Dispose();
        }

        private async Task TakePhoto(object sender, EventArgs e)
        {
            var takePhoto = "Take photo";
            var pickPhoto = "Pick from library";
            var options = new List<string>();
            if (CrossMedia.Current.IsTakePhotoSupported)
                options.Add(takePhoto);
            if (CrossMedia.Current.IsPickPhotoSupported)
                options.Add(pickPhoto);

            if (options.Any())
            {
                var result = await UserDialogs.Instance.ActionSheetAsync("Select picture to upload", "Cancel", null, null, options.ToArray());
                if (result.Equals(takePhoto))
                {
                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()); // TODO: Re-add Saving to local library. Fails at the current MediaPluginVersion
                }
                else if (result.Equals(pickPhoto))
                {
                    file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions { RotateImage = true });
                }

                if (file != null)
                {
                    // Show selected photo in preview
                    PhotoPreview.Source = ImageSource.FromFile(file.Path);
                }
            }
        }

        async void UploadButton_Clicked(object sender, System.EventArgs e)
        {
            if (PhotoPreview.Source == null)
                return;

            LoadingOverlay.IsVisible = true;
            LoadingAnimation.Play();

            using (var stream = new MemoryStream())
            {
                var croppedImage = await PhotoPreview.GetImageAsJpegAsync();
                await croppedImage.CopyToAsync(stream);
                stream.Position = 0;

                // Upload photo
                var success = await viewModel.UploadPhoto(stream, file.Path);
                if (success)
                {
                    PhotoPreview.Source = null;
                    viewModel.Caption = string.Empty;

                    // Navigate back to categories page
                    App.AppShell.SelectedItem = null; // Hack: Xamarin.Forms Bug does not allow the same navigation twice otherwise
                    App.AppShell.SelectedItem = App.AppShell.Children.First();

                    Analytics.TrackEvent("Photo uploaded");
                }
            }

            LoadingOverlay.IsVisible = false;
            LoadingAnimation.Pause();
        }

        private void CancelButton_Clicked(object sender, System.EventArgs e)
        {
            Analytics.TrackEvent("Photo upload cancelled by user");
            PhotoPreview.Source = null;
            file = null;
            viewModel.Caption = string.Empty;
        }
    }
}
