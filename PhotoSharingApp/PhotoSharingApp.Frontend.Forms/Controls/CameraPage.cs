using System;
using Xamarin.Forms;
namespace PhotoSharingApp.Forms.Controls
{
    public class CameraPage : ContentPage
    {
        public delegate void PhotoResultEventHandler(PhotoResultEventArgs result);
        public event PhotoResultEventHandler OnPhotoResult;

        public delegate void EventHandler(EventArgs args);
        public event EventHandler OnInitialize;
        public event EventHandler OnDispose;

        public void InitializeCameraStream()
        {
            OnInitialize?.Invoke(null);
        }

        public void DisposeCameraStream()
        {
            OnDispose?.Invoke(null);
        }

        public void SetPhotoResult(byte[] image, int width = -1, int height = -1)
        {
            OnPhotoResult?.Invoke(new PhotoResultEventArgs(image, width, height));
        }

        public void Cancel()
        {
            OnPhotoResult?.Invoke(new PhotoResultEventArgs());
        }
    }

    public class PhotoResultEventArgs
    {
        public bool Success { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public byte[] Image { get; private set; }

        public PhotoResultEventArgs(byte[] image, int width, int height)
        {
            Image = image;
            Width = width;
            Height = height;
            Success = image != null;
        }

        public PhotoResultEventArgs()
        {

        }
    }
}
