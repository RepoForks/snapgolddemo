using System;
using GalaSoft.MvvmLight;
using PhotoSharingApp.Frontend.Portable.Abstractions;
using System.Threading.Tasks;

namespace PhotoSharingApp.Frontend.Portable
{
    public class AsyncViewModelBase : ViewModelBase
    {
        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { isRefreshing = value; RaisePropertyChanged(); }
        }

        private bool isLoaded;
        public bool IsLoaded
        {
            get { return isLoaded; }
            set { isLoaded = value; RaisePropertyChanged(); }
        }

        protected async Task ShowNoConnectionDialog(IDialogService dialogService)
        {
            await dialogService.DisplayDialogAsync("Not connected.", "Looks like you have no connection to the internet. Please establish a connection and try again.", "Ok");
        }
    }
}
