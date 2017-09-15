using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using PhotoSharingApp.Frontend.Portable.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using System.Globalization;

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

            if (file != null)
                file.Dispose();
        }

        async void LibraryButton_Clicked(object sender, System.EventArgs e)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
                return;

            // Pick a file from the local library
            file = await CrossMedia.Current.PickPhotoAsync();
            if (file == null)
                return;

            // Hide camera controls and show upload controls
            CameraControls.IsVisible = false;
            UploadControls.IsVisible = true;

            // Show selected photo in preview
            PhotoPreview.Source = ImageSource.FromFile(file.Path);
        }

        async void TakeButton_Clicked(object sender, System.EventArgs e)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                return;

            file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions { AllowCropping = false, SaveToAlbum = false });
            if (file == null)
                return;

            // Hide camera controls and show upload controls
            CameraControls.IsVisible = false;
            UploadControls.IsVisible = true;

            // Show selected photo in preview
            PhotoPreview.Source = ImageSource.FromFile(file.Path);
        }


        private void Handle_Clicked(object sender, System.EventArgs e)
        {
            ViewFinder.Init();
        }

        private void CancelButton_Clicked(object sender, System.EventArgs e)
        {
            CameraControls.IsVisible = true;
            UploadControls.IsVisible = false;
            PhotoPreview.Source = null;
        }

        async void UploadButton_Clicked(object sender, System.EventArgs e)
        {
            LoadingOverlay.IsVisible = true;
            LoadingAnimation.Play();

            var success = await viewModel.UploadPhoto(file.GetStream(), file.Path);
            if (success)
            {
                CameraControls.IsVisible = true;
                UploadControls.IsVisible = false;
                PhotoPreview.Source = null;
            }

            LoadingOverlay.IsVisible = false;
            LoadingAnimation.IsPlaying = false;
            LoadingAnimation.Pause();
        }
    }
}
