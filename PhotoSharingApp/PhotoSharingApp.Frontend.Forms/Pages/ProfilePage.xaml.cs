﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using PhotoSharingApp.Frontend.Portable.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using PhotoSharingApp.Frontend.Portable.Models;
using Microsoft.AppCenter.Analytics;
using PhotoSharingApp.Forms.Pages;

namespace PhotoSharingApp.Forms
{
    public partial class ProfilePage : ContentPage
    {
        private ProfileViewModel viewModel;

        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = viewModel = SimpleIoc.Default.GetInstance<ProfileViewModel>();

            // Manage visibility to Log out button by subscribing the according property of the ViewModel
            // This has to be done, as Xamarin.Forms does not support a bindable IsVisible property for Toolbar Items, yet
            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName.Equals(nameof(viewModel.IsLoggedIn)))
                {
                    if (viewModel.IsLoggedIn && !ToolbarItems.Contains(SignOutButton))
                        ToolbarItems.Add(SignOutButton);
                    else if (!viewModel.IsLoggedIn && ToolbarItems.Contains(SignOutButton))
                        ToolbarItems.Remove(SignOutButton);
                }
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.InitAsync();
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Analytics.TrackEvent("Attempt to Login non-Facebook", new Dictionary<string, string> { { "Provider", (sender as Button).Text } });
            DisplayAlert("Not implemented yet", "Please use Facebook Login for now. Thanks.", "Ok");
        }

        void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (e.Item is Photo photo)
            {
                (sender as ListView).SelectedItem = null;
                viewModel.NavigateToPhotoCommand.Execute(photo);
            }
        }
    }
}
