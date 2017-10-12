using System;
using System.Collections.Generic;
using PhotoSharingApp.Frontend.Portable.Models;

namespace PhotoSharingApp.Frontend.Portable.Helpers
{
    public static class MockData
    {
        private static string mockImageUrl = "placeholder.jpg";

        public static List<Photo> GetHeroImages()
        {
            var mockHeroImage = new Photo()
            {
                CroppedUrl = mockImageUrl,
                HighResolutionUrl = mockImageUrl,
                StandardUrl = mockImageUrl,
                ThumbnailUrl = mockImageUrl
            };

            var list = new List<Photo>();
            //for (var i = 0; i < 5; i++)
            list.Add(mockHeroImage);

            return list;
        }

        public static List<GroupedCategoryPreview> GetTopCategories()
        {
            var mockPhotoThumbnail = new PhotoThumbnail { ImageUrl = mockImageUrl };
            var thumbnailList = new List<PhotoThumbnail>();
            for (var i = 0; i < 6; i++)
                thumbnailList.Add(mockPhotoThumbnail);

            var mockCategory = new CategoryPreview
            {
                Name = "",
                PhotoThumbnails = thumbnailList
            };

            var list = new List<GroupedCategoryPreview>();
            for (var i = 0; i < 8; i++)
            {
                list.Add(new GroupedCategoryPreview(thumbnailList, "", "", mockCategory));
            }

            return list;
        }
    }
}