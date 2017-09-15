# Xamarin App

## Libraries
MVVM Light Libs - http://www.mvvmlight.net/

https://www.nuget.org/packages/Xamarin.UITest.
https://github.com/alexrainman/CarouselView

## Icons
<a href="https://icons8.com/icon/43507/Camera">Camera icon credits</a> 
<a href="https://icons8.com/icon/14099/Settings">Settings icon credits</a>
<a href="https://icons8.com/icon/43764/Image-File">Image file icon credits</a>
<a href="https://icons8.com/icon/51791/Name">Name icon credits</a>

WinPhone Image cropper
```csharp
public class MediaService : IMediaService
{
    private MediaImplementation mi = new MediaImplementation();

    public byte[] ResizeImage(byte[] imageData, float width, float height)
    {
        byte[] resizedData;

        using (MemoryStream streamIn = new MemoryStream(imageData))
        {
            WriteableBitmap bitmap = PictureDecoder.DecodeJpeg(streamIn, (int)width, (int)height);

            float Height = 0;
            float Width = 0;

            float originalHeight = bitmap.PixelHeight;
            float originalWidth = bitmap.PixelWidth;

            if (originalHeight > originalWidth)
            {
                Height = height;
                float ratio = originalHeight / height;
                Width = originalWidth / ratio;
            }
            else
            {
                Width = width;
                float ratio = originalWidth / width;
                Height = originalHeight / ratio;
            }

            using (MemoryStream streamOut = new MemoryStream())
            {
                bitmap.SaveJpeg(streamOut, (int)Width, (int)Height, 0, 100);
                resizedData = streamOut.ToArray();
            }
        }
        return resizedData;
    }
}
```