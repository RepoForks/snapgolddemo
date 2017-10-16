using System;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace PhotoSharingApp.Forms.UITests
{
    public class MyProfilePage : BasePage
    {
		protected override PlatformQuery Trait => new PlatformQuery
		{
            Android = x => x.Class("AppCompatImageView"),
			iOS = x => x.Marked("Home")
		};

		public MyProfilePage()
		{
			if (OnAndroid)
			{
            }

			if (OniOS)
			{
			}
		}

        internal MyProfilePage FacebookLogin()
        {
            //TODO Need a Backdoor login method
            return this;
        }
    }
}