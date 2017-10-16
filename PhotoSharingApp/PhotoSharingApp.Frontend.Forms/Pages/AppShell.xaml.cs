using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PhotoSharingApp.Forms
{
    public partial class AppShell : TabbedPage
    {
        public AppShell()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }
    }
}
