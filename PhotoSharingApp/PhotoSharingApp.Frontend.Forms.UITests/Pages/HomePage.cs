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
                firstPhotoButton = x => x.Class("UIImageView").Index(3);
			}
		}

        internal HomePage ScrollThroughPhotos()
        {
            app.ScrollDown();
            return this;
        }


        internal HomePage SelectPhoto()
        {
            app.Tap(firstPhotoButton);
            //app.WaitForElement();
            
            return this;
        }

        internal void DeletePhoto()
        {
            //TODO This should be implemented on a new PhotoDetailsPage Class
           
        }

        internal void GiveGold(int amount)
        {
            //TODO This should be implemented on a new PhotoDetailsPage Class
        }

        internal void BuyGold(int v)
        {
            
        }
    }
}