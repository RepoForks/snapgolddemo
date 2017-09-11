using System;
using System.Threading.Tasks;
using PhotoSharingApp.Frontend.Portable.Abstractions;
using PhotoSharingApp.Frontend.Portable.Models;

namespace PhotoSharingApp.Forms.Services
{
    public class AuthenticationHandler : IAuthenticationHandler
    {
        public AuthenticationHandler()
        {
        }

        public System.Collections.Generic.List<Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider> AuthenticationProviders => throw new NotImplementedException();

        public Task AuthenticateAsync(Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider provider)
        {
            //throw new NotImplementedException();
            return null;
        }

        public Task LogoutAsync()
        {
            //throw new NotImplementedException();
            return null;
        }

        public void ResetPasswordVault()
        {
            //throw new NotImplementedException();
        }

        public Task<User> RestoreSignInStatus()
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}
