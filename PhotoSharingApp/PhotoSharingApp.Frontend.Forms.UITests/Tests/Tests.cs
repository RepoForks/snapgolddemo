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
		public void BrowsePhotos()
		{
          //  app.Repl();

            new HomePage()
                .SelectPhoto()
                .ScrollThroughPhotos();


            //new MyProfilePage()
            //    .DoSomething();

            //new LeaderboardsPage()
                //.DoSomething();
        }

        [Test]
        public void NewTest()
        {
            app.Tap(x => x.Class("AppCompatImageView"));
            app.Tap(x => x.Class("AppCompatImageView").Index(1));
            app.Tap(x => x.Class("AppCompatImageView").Index(2));
            app.Tap(x => x.Class("AppCompatImageView").Index(3));
           
            app.Tap(x => x.Class("CachedImageView").Index(1));

        }

            
            //app.Query(x => x.Class("UITableTextAccessibilityElement"));
            //app.Query(x => x.Class("UITableViewCellAccessibilityElement"));
            //app.Query(x => x.Class("UIButton"));

        //    [Test]
        //public void NewTest()
        //{
            //app.Tap(x => x.Class("CachedImageView").Index(1));
            //app.Screenshot("Tapped on view with class: CachedImageView");
            //app.Tap(x => x.Class("AppCompatImageButton"));
            //app.Screenshot("Tapped on view with class: AppCompatImageButton");
            //app.Tap(x => x.Class("AppCompatImageView").Index(1));
            //app.Screenshot("Tapped on view with class: AppCompatImageView");
            //app.EnterText(x => x.Class("EditText"), "D");
            //app.Tap(x => x.Class("AppCompatImageView").Index(2));
            //app.Screenshot("Tapped on view with class: AppCompatImageView");
            //app.Tap(x => x.Class("AppCompatImageView").Index(3));
            //app.Screenshot("Tapped on view with class: AppCompatImageView");
            //app.Tap(x => x.Class("AppCompatImageView").Index(1));
            //app.Screenshot("Tapped on view with class: AppCompatImageView");
            //app.ClearText(x => x.Class("EditText").Text("Demos"));
            //app.EnterText(x => x.Class("EditText"), "D");
            //app.Tap(x => x.Text("Take a picture"));
            //app.Screenshot("Tapped on view with class: AppCompatButton with text: Take a picture");
            //app.ClearText(x => x.Class("EditText").Text("Demos"));
            //app.EnterText(x => x.Class("EditText"), "D");
            //app.Tap(x => x.Class("FormsEditText"));
            //app.Screenshot("Tapped on view with class: FormsEditText");
            //app.EnterText(x => x.Class("FormsEditText"), "Caption");
            //app.PressEnter();
            //app.Tap(x => x.Text("Upload"));
            //app.Screenshot("Tapped on view with class: AppCompatButton with text: Upload");
            //app.Tap(x => x.Id("button2"));
            //app.Screenshot("Tapped on view with class: AppCompatButton with id: button2 with text: Ok");
       // }


    }
}
