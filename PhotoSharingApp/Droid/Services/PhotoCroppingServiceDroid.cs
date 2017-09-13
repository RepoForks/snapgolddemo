using System;
using PhotoSharingApp.Forms.Droid.Services;
using PhotoSharingApp.Frontend.Portable.Abstractions;

[assembly: Xamarin.Forms.Dependency(typeof(PhotoCroppingServiceDroid))]
namespace PhotoSharingApp.Forms.Droid.Services
{
    public class PhotoCroppingServiceDroid : IPhotoCroppingService
    {
        public byte[] ResizeImage(byte[] source, int width, int height)
        {
            return null;
        }
    }
}
