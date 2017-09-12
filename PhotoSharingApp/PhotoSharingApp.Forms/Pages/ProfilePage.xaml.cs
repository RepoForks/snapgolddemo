using System;
using System.Collections.Generic;

using Xamarin.Forms;
using PhotoSharingApp.Frontend.Portable.ViewModels;
using GalaSoft.MvvmLight.Ioc;

namespace PhotoSharingApp.Forms
{
    public partial class ProfilePage : ContentPage
    {
        private ProfileViewModel viewModel;

        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = viewModel = SimpleIoc.Default.GetInstance<ProfileViewModel>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.InitAsync();
        }
    }
}
