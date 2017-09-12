using System;
using System.Threading.Tasks;

namespace PhotoSharingApp.Frontend.Portable.Abstractions
{
    public interface IDialogService
    {
        Task DisplayDialogAsync(string title, string message, string cancel);
        Task<bool> DisplayDialogAsync(string title, string message, string accept, string cancel);
    }
}
