using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using PhotoSharingApp.Forms.iOS.CustomRenderers;
using UIKit;

// Disabled, because PrefersLargeTitles is still buggy in combination with Xamarin.Forms' TabbedPage
//[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]
namespace PhotoSharingApp.Forms.iOS.CustomRenderers
{
    public class CustomNavigationRenderer : NavigationRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            //NavigationBar.PrefersLargeTitles |= UIDevice.CurrentDevice.CheckSystemVersion(11, 0);
            //UINavigationBar.Appearance.LargeTitleTextAttributes = new UIStringAttributes { ForegroundColor = UIColor.White }; ;
        }
    }
}
