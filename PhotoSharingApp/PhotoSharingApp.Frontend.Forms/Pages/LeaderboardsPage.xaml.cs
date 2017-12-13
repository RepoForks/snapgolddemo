using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;
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
