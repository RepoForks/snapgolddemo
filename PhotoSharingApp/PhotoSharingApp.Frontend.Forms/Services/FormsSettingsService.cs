using System;
using System.Threading.Tasks;
using PhotoSharingApp.Frontend.Portable.Abstractions;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using PhotoSharingApp.Frontend.Portable.Models;
namespace PhotoSharingApp.Forms.Services
{
    public class FormsSettingsService : ISettingsService
    {
        public bool IsAnalyticsAllowed
        {
            get => CrossSettings.Current.GetValueOrDefault(nameof(IsAnalyticsAllowed), true);
            set => CrossSettings.Current.AddOrUpdateValue(nameof(IsAnalyticsAllowed), value);
        }
    }
}
