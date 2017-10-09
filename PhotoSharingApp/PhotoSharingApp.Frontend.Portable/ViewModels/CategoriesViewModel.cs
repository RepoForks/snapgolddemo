using PhotoSharingApp.Frontend.Portable.Services;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MvvmHelpers;
using PhotoSharingApp.Frontend.Portable.Models;
using System.Linq;
using GalaSoft.MvvmLight.Views;
using PhotoSharingApp.Frontend.Portable.Exceptions;
using PhotoSharingApp.Frontend.Portable.Helpers;
using PhotoSharingApp.Frontend.Portable.Abstractions;
using IDialogService = PhotoSharingApp.Frontend.Portable.Abstractions.IDialogService;

namespace PhotoSharingApp.Frontend.Portable.ViewModels
{
    public class CategoriesViewModel : AsyncViewModelBase
    {
        private INavigationService navigationService;
        private IDialogService dialogService;
        private IConnectivityService connectivityService;
        private IPhotoService photoService;

        private ObservableRangeCollection<Photo> heroImages;
        public ObservableRangeCollection<Photo> HeroImages
        {
            get { return heroImages; }
            set { heroImages = value; RaisePropertyChanged(); }
        }

        private ObservableRangeCollection<GroupedCategoryPreview> topCategories;
        public ObservableRangeCollection<GroupedCategoryPreview> TopCategories
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

        public CategoriesViewModel(INavigationService navigationService, IDialogService dialogService, IConnectivityService connectivityService, IPhotoService photoService)
        {
            this.navigationService = navigationService;
            this.dialogService = dialogService;
            this.connectivityService = connectivityService;
            this.photoService = photoService;

            heroImages = new ObservableRangeCollection<Photo>();
            topCategories = new ObservableRangeCollection<GroupedCategoryPreview>();


        }

        public async Task RefreshAsync(bool force = false)
        {
            // Check connectivity
            if (!connectivityService.IsConnected())
            {
                await ShowNoConnectionDialog(dialogService);
                return;
            }

            // Check if ViewModel is already loaded or refreshing
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
                CurrentUser = null;
            }

            // Load hero images
            var heroes = await photoService.GetHeroImages(5);
            HeroImages.ReplaceRange(heroes);

            // Load categories
            var topCat = await photoService.GetTopCategories(6);
            var grouped =
                from category in topCat
                group category by category.Name into categoryGroup
                select new GroupedCategoryPreview(categoryGroup.First()?.PhotoThumbnails, categoryGroup.Key, categoryGroup.Key.Substring(0, 1), categoryGroup.FirstOrDefault());
            TopCategories.ReplaceRange(grouped);

            // Check if loading went right
            IsLoaded = HeroImages.Count > 0 || TopCategories.Count > 0;

            IsRefreshing = false;
        }
    }
}
