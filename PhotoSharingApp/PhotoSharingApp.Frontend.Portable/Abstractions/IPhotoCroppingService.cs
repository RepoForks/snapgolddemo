using System;
using System.IO;

namespace PhotoSharingApp.Frontend.Portable.Abstractions
{
    public interface IPhotoCroppingService
    {
        byte[] ResizeImage(byte[] source, int width, int height);
    }
}
