using System;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace PhotoSharingApp.Forms.UITests
{
	public class LeaderboardsPage : BasePage
	{
		protected override PlatformQuery Trait => new PlatformQuery
		{
			Android = x => x.Marked("Home"),
			iOS = x => x.Marked("Home")
		};

		public LeaderboardsPage()
		{
			if (OnAndroid)
			{
			}

			if (OniOS)
			{
			}
		}

		public void DoSomething()
		{
		}
	}
}