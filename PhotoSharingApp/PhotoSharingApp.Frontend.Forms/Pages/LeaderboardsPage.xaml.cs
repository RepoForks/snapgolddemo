using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Analytics;
using Xamarin.Forms;

namespace PhotoSharingApp.Forms
{
    public partial class LeaderboardsPage : ContentPage
    {
        public LeaderboardsPage()
        {
            Analytics.TrackEvent("Navigate To Leaderboards");
            InitializeComponent();
        }
    }
}
