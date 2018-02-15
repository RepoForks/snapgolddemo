﻿using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using FFImageLoading.Forms.Droid;
using Plugin.Permissions;
using Microsoft.WindowsAzure.MobileServices;
using PhotoSharingApp.Frontend.Portable.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoSharingApp.Frontend.Portable.Services;
using PhotoSharingApp.Frontend.Portable.Models;
using Plugin.SecureStorage;
using PhotoSharingApp.Portable.DataContracts;
using System.Net;
using System.Net.Http;
using PhotoSharingApp.Frontend.Portable.ContractModelConverterExtensions;
using Lottie.Forms.Droid;
using CarouselView.FormsPlugin.Android;
using Acr.UserDialogs;
using Android.Content.Res;
using Android.Media;
using Microsoft.AppCenter;

namespace PhotoSharingApp.Forms.Droid
{
    [Activity(Label = "SnapGold", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticationHandler
    {
        public List<MobileServiceAuthenticationProvider> AuthenticationProviders { get; set; }

        public MainActivity()
        {
            SecureStorageImplementation.StoragePassword = "SnapGold";
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;


            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            CachedImageRenderer.Init(false);
            AnimationViewRenderer.Init();
            CarouselViewRenderer.Init();
            UserDialogs.Init(() => (Activity)global::Xamarin.Forms.Forms.Context);
            Microsoft.AppCenter.Push.Push.SetSenderId("714883573816");

            // Initialize Azure Mobile App Client for the current platform
            CurrentPlatform.Init();

            LoadApplication(new App(this));

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
                // Login with website
                var user = await AzureAppService.Current.LoginAsync(this, provider);

                // Store credentials in secure storage
                CrossSecureStorage.Current.SetValue("userId", user.UserId);
                CrossSecureStorage.Current.SetValue("authToken", user.MobileServiceAuthenticationToken);
            }
            catch (Exception)
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
            CrossSecureStorage.Current.DeleteKey("userId");
            CrossSecureStorage.Current.DeleteKey("authToken");
        }

        public async Task<User> RestoreSignInStatus()
        {
            var userId = CrossSecureStorage.Current.GetValue("userId", null);
            var authToken = CrossSecureStorage.Current.GetValue("authToken", null);

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
