using System;
using System.Collections.Generic;
using System.Linq;
using FFImageLoading.Forms.Touch;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Microsoft.WindowsAzure.MobileServices;
using UIKit;

namespace PhotoSharingApp.Forms.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            UITabBar.Appearance.SelectedImageTintColor = UIColor.FromRGB(250, 168, 25);

            global::Xamarin.Forms.Forms.Init();
            ImageCircleRenderer.Init();
            CachedImageRenderer.Init();

            // Initialize Azure Mobile App Client for the current platform
            CurrentPlatform.Init();

            // Code for starting up the Xamarin Test Cloud Agent
#if DEBUG
            Xamarin.Calabash.Start();
#endif

            LoadApplication(new App());

            var result = base.FinishedLaunching(app, options);
            return result;
        }
    }
}
