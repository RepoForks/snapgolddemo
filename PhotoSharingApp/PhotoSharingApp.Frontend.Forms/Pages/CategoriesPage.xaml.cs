using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Ioc;
using PhotoSharingApp.Frontend.Portable.ViewModels;
using Xamarin.Forms;
using PhotoSharingApp.Frontend.Portable.Models;
using System.Linq;
using Microsoft.Azure.Mobile.Analytics;

namespace PhotoSharingApp.Forms
{
    public partial class CategoriesPage : ContentPage
    {
        private CategoriesViewModel viewModel;

        public CategoriesPage()
        {
            Analytics.TrackEvent("Navigate to Categories");
            InitializeComponent();
            BindingContext = viewModel = SimpleIoc.Default.GetInstance<CategoriesViewModel>();
            AddPhotoButton.Clicked += AddPhotoButton_Clicked;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.RefreshAsync();
        }

        void Handle_FlowItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var selectedItem = e.Item as PhotoThumbnail;
            if (selectedItem != null && !selectedItem.ImageUrl.Contains("placeholder"))
                viewModel.ShowCategoryStreamCommand.Execute(selectedItem);
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            DisplayAlert("Not implemented yet", "", "Too bad");
        }

        void AddPhotoButton_Clicked(object sender, System.EventArgs e)
        {
            Analytics.TrackEvent("Add Photo Button Clicked");
            // Navigate to camera page
            App.AppShell.SelectedItem = null; // Hack: Xamarin.Forms Bug does not allow the same navigation twice otherwise
            App.AppShell.SelectedItem = App.AppShell.Children.ElementAt(1);
        }
    }
}
