using System;
using System.Collections.Generic;
using PhotoSharingApp.Frontend.Helpers;
using PhotoSharingApp.Frontend.Portable.Models;

namespace PhotoSharingApp.Frontend.Portable.Helpers
{
    public class GroupedCategoryPreview : GroupedRangeCollection<PhotoThumbnail>
    {
        public CategoryPreview CategoryPreview;

        public GroupedCategoryPreview(IEnumerable<PhotoThumbnail> items, string name, string shortName, CategoryPreview categoryPreview) : base(items, name, shortName)
        {
            this.CategoryPreview = categoryPreview;
        }
    }
}
