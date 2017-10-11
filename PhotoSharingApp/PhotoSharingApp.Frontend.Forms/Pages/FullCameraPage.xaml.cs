using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Plugin.Media;

namespace PhotoSharingApp.Forms.Pages
{
    public partial class FullCameraPage : Controls.CameraPage
    {
        public FullCameraPage()
        {
            InitializeComponent();
            OnPhotoResult += Handle_OnPhotoResult;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (CrossMedia.Current.IsCameraAvailable)
                InitializeCameraStream();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DisposeCameraStream();
        }

        async void Handle_OnPhotoResult(Controls.PhotoResultEventArgs result)
        {
            if (result.Success)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {

                    await Navigation.PushAsync(new UploadPage(result), false);
                });
            }
        }

        async void LibraryButton_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UploadPage(null), false);
        }
    }
}
