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
		public void TestSomething()
		{
			
			new HomePage()
				.DoSomething();

            new MyProfilePage()
                .DoSomething();

            new LeaderboardsPage()
                .DoSomething();
		}
    }
}
