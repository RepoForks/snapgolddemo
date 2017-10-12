using System;
using ObjCRuntime;
using PhotoSharingApp.Forms.Abstractions;
using PhotoSharingApp.Forms.iOS.Services;

[assembly: Xamarin.Forms.Dependency(typeof(EnvironmentService))]
namespace PhotoSharingApp.Forms.iOS.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        public bool IsRunningInRealWorld()
        {
#if DEBUG
        return false;
#endif

            if (Runtime.Arch == Arch.SIMULATOR ||
                Environment.GetEnvironmentVariable("XAMARIN_TEST_CLOUD") != null)
                return false;

            return true;
        }
    }
}