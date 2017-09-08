using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Plugin.Media;

namespace PhotoSharingApp.Forms
{
    public partial class CameraPage : ContentPage
    {
        public CameraPage()
        {
            InitializeComponent();
        }

        private async void Handle_Clicked(object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg"
            });

            if (file == null)
                return;




            //PreviewImage.Source = ImageSource.FromStream(() =>
            //{
            //    var stream = file.GetStream();
            //    file.Dispose();
            //    return stream;
            //});

        }
    }
}
