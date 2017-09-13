using System;
using System.Collections.Generic;

using Xamarin.Forms;
using PhotoSharingApp.Frontend.Portable.Models;
using PhotoSharingApp.Frontend.Portable;
using GalaSoft.MvvmLight.Ioc;

namespace PhotoSharingApp.Forms
{
    public partial class StreamPage : ContentPage
    {
        private StreamPageViewModel viewModel;

        public StreamPage(CategoryPreview categoryPreview)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");

            viewModel = SimpleIoc.Default.GetInstance<StreamPageViewModel>();
            viewModel.Init(categoryPreview);
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.RefreshAsync();
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Photo item)
            {
                (sender as ListView).SelectedItem = null;
                viewModel.ShowPhotoDetailsCommand.Execute(item);
            }
        }
    }
}
