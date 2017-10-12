using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FFImageLoading.Forms.Touch;
using Foundation;
using Microsoft.WindowsAzure.MobileServices;
using PhotoSharingApp.Frontend.Portable.Abstractions;
using PhotoSharingApp.Frontend.Portable.Models;
using PhotoSharingApp.Frontend.Portable.Services;
using UIKit;
using Plugin.SecureStorage.Abstractions;
using Plugin.SecureStorage;
using System.Net.Http;
using System.Net;
using PhotoSharingApp.Portable.DataContracts;
using PhotoSharingApp.Frontend.Portable.ContractModelConverterExtensions;
using Lottie.Forms.iOS.Renderers;
using CarouselView.FormsPlugin.iOS;
using Microsoft.Azure.Mobile.Distribute;

namespace PhotoSharingApp.Forms.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IAuthenticationHandler
    {
        private SecureStorageImplementation secureStorage = new SecureStorageImplementation() { };
        public List<MobileServiceAuthenticationProvider> AuthenticationProviders { get; set; }


        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            AuthenticationProviders = new List<MobileServiceAuthenticationProvider>
            {
                MobileServiceAuthenticationProvider.Facebook,
                //MobileServiceAuthenticationProvider.MicrosoftAccount,
                //MobileServiceAuthenticationProvider.Google,
                //MobileServiceAuthenticationProvider.Twitter
            };

            UITabBar.Appearance.SelectedImageTintColor = UIColor.FromRGB(250, 168, 25);

            global::Xamarin.Forms.Forms.Init();
            CachedImageRenderer.Init();
            AnimationViewRenderer.Init();
            CarouselViewRenderer.Init();


            // Initialize Azure Mobile App Client for the current platform
            CurrentPlatform.Init();

            // Code for starting up the Xamarin Test Cloud Agent
            Xamarin.Calabash.Start();

            // Disable Mobile Center updates when in Debug Mode
            Distribute.DontCheckForUpdatesInDebug();

            LoadApplication(new App(this));

            var result = base.FinishedLaunching(app, options);
            return result;
        }

        #region IAuthenticationHandler implementation

        public async Task AuthenticateAsync(MobileServiceAuthenticationProvider provider)
        {
            try
            {
                // Login with website
                var user = await AzureAppService.Current.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController, provider);

                // Store credentials in secure storage
                secureStorage.SetValue("userId", user.UserId);
                secureStorage.SetValue("authToken", user.MobileServiceAuthenticationToken);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task LogoutAsync()
        {
            await AzureAppService.Current.LogoutAsync();
            ResetPasswordVault();
        }

        public void ResetPasswordVault()
        {
            secureStorage.DeleteKey("userId");
            secureStorage.DeleteKey("authToken");
        }

        public async Task<User> RestoreSignInStatus()
        {
            var userId = secureStorage.GetValue("userId", null);
            var authToken = secureStorage.GetValue("authToken", null);

            if (userId == null || authToken == null)
                return null;

            var user = new MobileServiceUser(userId);
            user.MobileServiceAuthenticationToken = authToken;
            AzureAppService.Current.CurrentUser = user;

            try
            {
                var userContract = await AzureAppService.Current.InvokeApiAsync<UserContract>("User", HttpMethod.Get, null);
                return userContract.ToDataModel();
            }
            catch (MobileServiceInvalidOperationException invalidOperationException)
            {
                if (invalidOperationException.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Remove the credentials.
                    ResetPasswordVault();
                    AzureAppService.Current.CurrentUser = null;
                }
            }

            return null;
        }

        #endregion
    }
}
