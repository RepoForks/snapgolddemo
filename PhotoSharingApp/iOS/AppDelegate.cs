using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FFImageLoading.Forms.Touch;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Microsoft.WindowsAzure.MobileServices;
using PhotoSharingApp.Frontend.Portable.Abstractions;
using PhotoSharingApp.Frontend.Portable.Models;
using PhotoSharingApp.Frontend.Portable.Services;
using UIKit;

namespace PhotoSharingApp.Forms.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IAuthenticationHandler
    {
        public List<MobileServiceAuthenticationProvider> AuthenticationProviders { get; set; }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            AuthenticationProviders = new List<MobileServiceAuthenticationProvider>
            {
                MobileServiceAuthenticationProvider.Facebook,
                MobileServiceAuthenticationProvider.MicrosoftAccount,
                MobileServiceAuthenticationProvider.Google,
                MobileServiceAuthenticationProvider.Twitter
            };

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

            LoadApplication(new App(this));

            var result = base.FinishedLaunching(app, options);
            return result;
        }

        public async Task AuthenticateAsync(MobileServiceAuthenticationProvider provider)
        {
            try
            {
                var user = await AzureAppService.Current.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController, provider);
            }
            catch (Exception ex)
            {

            }
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public void ResetPasswordVault()
        {
            throw new NotImplementedException();
        }

        public Task<User> RestoreSignInStatus()
        {
            throw new NotImplementedException();
        }
    }
}
