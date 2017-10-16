using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Analytics;
using Plugin.VersionTracking;
using Xamarin.Forms;

namespace PhotoSharingApp.Forms.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            Analytics.TrackEvent("Navigate to Settings");
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");

            var version = CrossVersionTracking.Current;
            VersionText.Text = $"{version.CurrentVersion} (Build {version.CurrentBuild})";
        }
    }
}
