using System;
using System.IO;
using Android.Graphics;
using PhotoSharingApp.Forms.Droid.Services;
using PhotoSharingApp.Frontend.Portable.Abstractions;

[assembly: Xamarin.Forms.Dependency(typeof(PhotoCroppingServiceDroid))]
namespace PhotoSharingApp.Forms.Droid.Services
{
    public class PhotoCroppingServiceDroid : IPhotoCroppingService
    {
        public byte[] ResizeImage(byte[] source, int width, int height)
        {
            using (var originalImage = BitmapFactory.DecodeByteArray(source, 0, source.Length))
            {
                // Determine the ratio
                var ratioX = (double)width / originalImage.Width;
                var ratioY = (double)height / originalImage.Height;
                var ratio = Math.Min(ratioX, ratioY);

                // Calculate the new width/height
                var newWidth = (int)(originalImage.Width * ratio);
                var newHeight = (int)(originalImage.Height * ratio);

                using (var resizedImage = Bitmap.CreateScaledBitmap(originalImage, newWidth, newHeight, false))
                {
                    using (var ms = new MemoryStream())
                    {
                        resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                        return ms.ToArray();
                    }
                }
            }
        }
    }
}
