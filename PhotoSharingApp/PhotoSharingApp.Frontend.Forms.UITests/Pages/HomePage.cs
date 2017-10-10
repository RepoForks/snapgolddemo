using System;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace PhotoSharingApp.Forms.UITests
{
	public class HomePage : BasePage
	{
        readonly Query firstPhotoButton;

		protected override PlatformQuery Trait => new PlatformQuery
		{
            Android = x => x.Class("AppCompatImageView"),
			iOS = x => x.Marked("Home")
		};

		public HomePage()
		{
			if (OnAndroid)
			{
                firstPhotoButton = x => x.Class("CachedImageView").Index(2);
			}

			if (OniOS)
			{
			}
		}

        internal HomePage ScrollThroughPhotos()
        {
            app.ScrollDown();
            app.ScrollDown();
            return this;
        }

        public void DoSomething()
		{
		}

        internal HomePage SelectPhoto()
        {
            app.Tap(firstPhotoButton);
            //app.WaitForElement();
            
            return this;
        }
    }
}