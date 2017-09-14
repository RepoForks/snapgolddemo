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

                using (var context = new CGBitmapContext(IntPtr.Zero, newWidth, newHeight, 8, (4 * newWidth), CGColorSpace.CreateDeviceRGB(), CGImageAlphaInfo.PremultipliedFirst))
                {
                    var imageRect = new RectangleF(0, 0, newWidth, newHeight);

                    // draw the image
                    context.DrawImage(imageRect, originalImage.CGImage);

                    // save the image as a jpeg
                    var resizedImage = UIImage.FromImage(context.ToImage(), 0, originalImage.Orientation);
                    return resizedImage.AsJPEG().ToArray();
                }
            }
        }
    }
}
