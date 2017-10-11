using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace PhotoSharingApp.Forms.UITests
{
    public class Tests: BaseTestFixture
    {
        public Tests(Platform platform): base(platform)
        {
        }

		[Test]
		public void Repl()
		{
			if (TestEnvironment.IsTestCloud)
				Assert.Ignore("Local only");

			app.Repl();
		}

        [Test]
        public void LoginAndUploadPhoto()
        {
            new HomePage()
                .NavigateToMyProfile();

            new MyProfilePage()
                .FacebookLogin()
                .NavigateToUpload();

            new UploadPage()
                .UploadPicture();
            
        }

        [Test]
        public void LoginAndDeletePhoto()
        {
            new HomePage()
                .NavigateToMyProfile();

            new MyProfilePage()
                .FacebookLogin()
                .NavigateToHome();

            new HomePage()
                .SelectPhoto()
                .DeletePhoto();
            
        }

        [Test]
        public void LoginAndGiveGold()
        {
            new HomePage()
                .NavigateToMyProfile();

            new MyProfilePage()
                .FacebookLogin()
                .NavigateToHome();

            new HomePage()
                .SelectPhoto()
                .GiveGold(10);
        }

        [Test]
        public void LoginAndTakePhoto()
        {
            new HomePage()
                .NavigateToMyProfile();

            new MyProfilePage()
                .FacebookLogin()
                .NavigateToUpload();

            new UploadPage()
                .TakePicture();
            
        }

        [Test]
        public void LoginAndBuyGold()
        {
            new HomePage()
          .NavigateToMyProfile();

            new MyProfilePage()
                .FacebookLogin()
                .NavigateToHome();
            new HomePage()
              .BuyGold(10);
        }

      

        [Test]
		public void BrowsePhotos()
		{
            new HomePage()
                .SelectPhoto()
                .ScrollThroughPhotos();
        }

        [Test]
        public void BrowseLeaderboard()
        {
            new HomePage()
                .NavigateToLeaderboard();
        }
    }
}
