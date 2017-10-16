using System;
using System.Collections.Generic;
using MvvmHelpers;

namespace PhotoSharingApp.Frontend.Helpers
{
    public class GroupedRangeCollection<T> : ObservableRangeCollection<T>
    {
        public string Name { get; set; }
        public string ShortName { get; set; }

        public GroupedRangeCollection(IEnumerable<T> items, string name, string shortName)
        {
            Name = name;
            ShortName = shortName;

            foreach (var item in items)
                this.Items.Add(item);
        }
    }
}
