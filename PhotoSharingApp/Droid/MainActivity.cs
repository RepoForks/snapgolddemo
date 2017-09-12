using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ImageCircle.Forms.Plugin.Droid;
using FFImageLoading.Forms.Droid;
using Plugin.Permissions;
using Microsoft.WindowsAzure.MobileServices;
using PhotoSharingApp.Frontend.Portable.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoSharingApp.Frontend.Portable.Services;
using PhotoSharingApp.Frontend.Portable.Models;

namespace PhotoSharingApp.Forms.Droid
{
    [Activity(Label = "PhotoSharingApp.Forms.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticationHandler
    {
        public List<MobileServiceAuthenticationProvider> AuthenticationProviders { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            ImageCircleRenderer.Init();
            CachedImageRenderer.Init();

            // Initialize Azure Mobile App Client for the current platform
            CurrentPlatform.Init();

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #region IAuthenticationHandler implementation

        public async Task AuthenticateAsync(MobileServiceAuthenticationProvider provider)
        {
            try
            {
                await AzureAppService.Current.LoginAsync(this, provider);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task LogoutAsync()
        {
            await AzureAppService.Current.LogoutAsync();
        }

        public void ResetPasswordVault()
        {
            throw new NotImplementedException();
        }

        public Task<User> RestoreSignInStatus()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
