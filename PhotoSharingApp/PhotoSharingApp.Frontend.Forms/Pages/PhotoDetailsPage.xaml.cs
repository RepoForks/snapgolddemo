﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using PhotoSharingApp.Frontend.Portable;
using GalaSoft.MvvmLight.Ioc;
using PhotoSharingApp.Frontend.Portable.Models;
using Acr.UserDialogs;
using Microsoft.AppCenter.Analytics;

namespace PhotoSharingApp.Forms
{
    public partial class PhotoDetailsPage : ContentPage
    {
        private PhotoDetailsViewModel viewModel;

        public PhotoDetailsPage()
        {
            InitializeComponent();
            Analytics.TrackEvent("Navigate to Photo Details");
        }

        public PhotoDetailsPage(Photo photo)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");

            viewModel = SimpleIoc.Default.GetInstance<PhotoDetailsViewModel>();
            viewModel.Photo = photo;
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.RefreshAsync();

            // Hide controls if photo is not owned by current user
            if (!viewModel.IsCurrentUsersPhoto)
            {
                ToolbarItems.Remove(DeleteToolbarItem);
                ToolbarItems.Remove(SetAsProfilePictureToolbarItem);
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }

        async void GiveGold_Clicked(object sender, System.EventArgs e)
        {
            Analytics.TrackEvent("Give Gold Button Clicked");
            await GoldIndicator.ScaleTo(1.1, 100);
            await GoldIndicator.ScaleTo(1.0, 100);

            var gold = await UserDialogs.Instance.PromptAsync("How much gold do you want to give?", "Gold", "Ok", "Cancel", "", InputType.Number);
            if (gold.Ok && !string.IsNullOrEmpty(gold.Value))
            {
                var text = await UserDialogs.Instance.PromptAsync("What do you think?", "Comment");
                if (!string.IsNullOrEmpty(text.Value))
                {
                    var annotation = new Annotation { Text = text.Value, GoldCount = Convert.ToInt32(gold.Value) };
                    viewModel.AddAnnotationCommand.Execute(annotation);
                }
            }
        }
    }
}
