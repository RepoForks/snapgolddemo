using System;
using System.Drawing;
using System.IO;
using CoreGraphics;
using PhotoSharingApp.Forms.iOS.Services;
using PhotoSharingApp.Frontend.Portable.Abstractions;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(PhotoCroppingServiceiOS))]
namespace PhotoSharingApp.Forms.iOS.Services
{
    public class PhotoCroppingServiceiOS : IPhotoCroppingService
    {
        public byte[] ResizeImage(byte[] source, int width, int height)
        {
            using (var originalImage = new UIImage(Foundation.NSData.FromArray(source)))
            {
                // Determine the ratio
                var ratioX = (double)width / originalImage.Size.Width;
                var ratioY = (double)height / originalImage.Size.Height;
                var ratio = Math.Min(ratioX, ratioY);

                // Calculate the new width/height
                var newWidth = (int)(originalImage.Size.Width * ratio);
                var newHeight = (int)(originalImage.Size.Height * ratio);

                UIGraphics.BeginImageContext(new SizeF(newWidth, newHeight));
                originalImage.Draw(new RectangleF(0, 0, newWidth, newHeight));
                var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();

                var bytesImagen = resizedImage.AsJPEG().ToArray();
                resizedImage.Dispose();
                return bytesImagen;
            }
        }
    }
}
