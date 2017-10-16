using System;
using PhotoSharingApp.Frontend.Portable.Abstractions;
using Plugin.Connectivity;

namespace PhotoSharingApp.Forms.Services
{
    public class FormsConnectivityService : IConnectivityService
    {
        public bool IsConnected()
        {
            return CrossConnectivity.Current.IsConnected;
        }
    }
}
