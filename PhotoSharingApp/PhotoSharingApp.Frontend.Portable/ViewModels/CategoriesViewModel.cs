using System;
using GalaSoft.MvvmLight;
using PhotoSharingApp.Frontend.Portable.Services;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MvvmHelpers;
using PhotoSharingApp.Frontend.Portable.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Views;
using PhotoSharingApp.Frontend.Portable.Exceptions;
using PhotoSharingApp.Frontend.Portable.Helpers;

namespace PhotoSharingApp.Frontend.Portable.ViewModels
{
    public class CategoriesViewModel : AsyncViewModelBase
    {
        private INavigationService navigationService;
        private IPhotoService photoService;

        private ObservableCollection<Photo> heroImages;
        public ObservableCollection<Photo> HeroImages
        {
            get { return heroImages; }
            set { heroImages = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<GroupedCategoryPreview> topCategories;
        public ObservableCollection<GroupedCategoryPreview> TopCategories
        {
            get { return topCategories; }
            set { topCategories = value; RaisePropertyChanged(); }
        }

        private User currentUser;
        public User CurrentUser
        {
            get { return currentUser; }
            set { currentUser = value; RaisePropertyChanged(); }
        }

        private RelayCommand refreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                return refreshCommand ?? (refreshCommand = new RelayCommand(async () =>
                {
                    await RefreshAsync(true);
                }));
            }
        }

        private RelayCommand<Photo> showPhotoDetailsCommand;
        public RelayCommand<Photo> ShowPhotoDetailsCommand
        {
            get
            {
                return showPhotoDetailsCommand ?? (showPhotoDetailsCommand = new RelayCommand<Photo>((Photo photo) =>
                {
                    navigationService.NavigateTo(ViewNames.PhotoDetailsPage, photo);
                }));
            }
        }

        private RelayCommand<PhotoThumbnail> showCategoryStreamCommand;
        public RelayCommand<PhotoThumbnail> ShowCategoryStreamCommand
        {
            get
            {
                return showCategoryStreamCommand ?? (showCategoryStreamCommand = new RelayCommand<PhotoThumbnail>((PhotoThumbnail photoThumbnail) =>
                {
                    var categoryPreviews = TopCategories.SingleOrDefault(c => c.Contains(photoThumbnail));
                    if (categoryPreviews != null)
                    {
                        navigationService.NavigateTo(ViewNames.StreamPage, new CategoryThumbnailBundle(categoryPreviews.CategoryPreview, photoThumbnail));
                    }
                }));
            }
        }

        public CategoriesViewModel(INavigationService navigationService, IPhotoService photoService)
        {
            this.navigationService = navigationService;
            this.photoService = photoService;

            heroImages = new ObservableCollection<Photo>();
            topCategories = new ObservableCollection<GroupedCategoryPreview>();

            // Design Data
            //topCategories.Add(new GroupedCategoryPreview(new List<PhotoThumbnail>
            //{
            //    new PhotoThumbnail { ImageUrl = "http://i0.wp.com/www.saint-mary.church/wp-content/uploads/2016/09/lorem-ipsum-logo.jpg?ssl=1" },
            //    new PhotoThumbnail { ImageUrl = "http://i0.wp.com/www.saint-mary.church/wp-content/uploads/2016/09/lorem-ipsum-logo.jpg?ssl=1" }
            //}, "Test", "T", null));
        }

        public async Task RefreshAsync(bool force = false)
        {
            if (IsRefreshing || (IsLoaded && !force))
                return;

            IsRefreshing = true;

            // Get current user
            try
            {
                // Try to restore user credentials
                if (AzureAppService.Current.CurrentUser == null)
                    await photoService.RestoreSignInStatusAsync();

                CurrentUser = await photoService.GetCurrentUser();
            }
            catch (UnauthorizedException)
            {
                // User not logged in yet
            }


            // Load hero images
            var heroes = await photoService.GetHeroImages(5);
            for (var i = 0; i < HeroImages.Count(); i++)
            {
                var existing = heroes.FirstOrDefault(img => img.Id == HeroImages[i].Id);
                if (existing != null)
                    HeroImages[i] = existing;
                else
                    HeroImages.RemoveAt(i);
            }
            foreach (var heroImage in heroes)
            {
                var existing = HeroImages.FirstOrDefault(img => img.Id == heroImage.Id);
                if (existing == null)
                    HeroImages.Insert(0, heroImage);
            }

            // Load categories
            var topCat = await photoService.GetTopCategories(5);
            var grouped =
                from category in topCat
                group category by category.Name into categoryGroup
                select new GroupedCategoryPreview(categoryGroup.First()?.PhotoThumbnails, categoryGroup.Key, categoryGroup.Key.Substring(0, 1), categoryGroup.FirstOrDefault());

            TopCategories.Clear();
            foreach (var grp in grouped)
            {
                TopCategories.Add(grp);
            }

            //TopCategories.ReplaceRange(grouped);

            // Check if loading right
            if (HeroImages.Count > 0 || TopCategories.Count > 0)
            {
                IsLoaded = true;
            }
            else
            {
                IsLoaded = false;
            }

            IsRefreshing = false;
        }
    }
}
