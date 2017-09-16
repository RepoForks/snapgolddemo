using System;
using System.Collections.Generic;
using Plugin.VersionTracking;
using Xamarin.Forms;

namespace PhotoSharingApp.Forms.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");

            var version = CrossVersionTracking.Current;
            VersionText.Text = $"{version.CurrentVersion} (Build {version.CurrentBuild})";

        }
    }
}
