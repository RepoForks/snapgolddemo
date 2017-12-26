using System;
using System.Threading.Tasks;

namespace PhotoSharingApp.Frontend.Portable.Abstractions
{
    public interface ISettingsService
    {
        bool IsAnalyticsAllowed { get; set; }
    }
}
