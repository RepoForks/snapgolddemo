using System;
using PhotoSharingApp.Frontend.Portable.Models;

namespace PhotoSharingApp.Frontend.Portable.Helpers
{
    public class CategoryThumbnailBundle
    {
        public CategoryPreview Preview { get; set; }
        public PhotoThumbnail Thumbnail { get; set; }

        public CategoryThumbnailBundle(CategoryPreview preview, PhotoThumbnail thumbnail)
        {
            this.Preview = preview;
            this.Thumbnail = thumbnail;
        }
    }
}
