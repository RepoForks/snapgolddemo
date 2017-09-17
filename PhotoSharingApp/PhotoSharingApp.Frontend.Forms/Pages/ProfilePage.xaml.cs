using System;
using System.Collections.Generic;

using Xamarin.Forms;
using PhotoSharingApp.Frontend.Portable.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using PhotoSharingApp.Frontend.Portable.Models;

namespace PhotoSharingApp.Forms
{
    public partial class ProfilePage : ContentPage
    {
        private ProfileViewModel viewModel;

        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = viewModel = SimpleIoc.Default.GetInstance<ProfileViewModel>();

            // Manage visibility to Log out button
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
