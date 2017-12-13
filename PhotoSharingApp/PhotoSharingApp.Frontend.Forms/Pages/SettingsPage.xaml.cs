using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.AppCenter.Analytics;
using PhotoSharingApp.Frontend.Portable.Models;
using PhotoSharingApp.Frontend.Portable.ViewModels;
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

            // Set Version and Build
            VersionLabel.Text = $"{CrossVersionTracking.Current.CurrentVersion} (Build {CrossVersionTracking.Current.CurrentBuild})";

            BindingContext = SimpleIoc.Default.GetInstance<SettingsViewModel>();
        }

        void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is ThirdPartyLibrary selectedLibrary)
                Device.OpenUri(new Uri(selectedLibrary.Url));

            (sender as ListView).SelectedItem = null;
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Device.OpenUri(new Uri("https://github.com/robinmanuelthiel/Pollenalarm"));



        }
    }
}
