using System;
using System.Threading.Tasks;
using PhotoSharingApp.Frontend.Portable.Services;
using Xamarin.Forms;

namespace PhotoSharingApp.Forms.Services
{
    public class FormsDialogService : IDialogService
    {
        public async Task DisplayDialogAsync(string title, string message, string cancel)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }

        public async Task<bool> DisplayDialogAsync(string title, string message, string accept, string cancel)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }
    }
}
