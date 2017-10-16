using System;
using Foundation;
using PhotoSharingApp.Forms.Controls;
using PhotoSharingApp.Forms.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(ContentPage), typeof(CustomPageRenderer))]
namespace PhotoSharingApp.Forms.iOS.CustomRenderers
{
    public class CustomPageRenderer : PageRenderer
    {
        private NSObject _observerHideKeyboard;
        private NSObject _observerShowKeyboard;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Repaint Refesh Control
            //UIRefreshControl.Appearance.TintColor = UIColor.FromRGB(250, 168, 25);

            // Fix Xamarin.Forms Keyboard issue
            var cp = Element as KeyboardResizingAwareContentPage;
            if (cp != null && !cp.CancelsTouchesInView)
            {
                foreach (var g in View.GestureRecognizers)
                {
                    g.CancelsTouchesInView = false;
                }
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            _observerHideKeyboard = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            _observerShowKeyboard = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            NSNotificationCenter.DefaultCenter.RemoveObserver(_observerHideKeyboard);
            NSNotificationCenter.DefaultCenter.RemoveObserver(_observerShowKeyboard);
        }

        void OnKeyboardNotification(NSNotification notification)
        {
            if (!IsViewLoaded) return;

            var frameBegin = UIKeyboard.FrameBeginFromNotification(notification);
            var frameEnd = UIKeyboard.FrameEndFromNotification(notification);

            var page = Element as ContentPage;
            if (page != null && !(page.Content is ScrollView))
            {
                var padding = page.Padding;
                page.Padding = new Thickness(padding.Left, padding.Top, padding.Right, padding.Bottom + frameBegin.Top - frameEnd.Top);
            }
        }
    }
}