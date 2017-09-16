using System;
using Xamarin.Forms;

namespace PhotoSharingApp.Forms.Controls
{
    public class CameraViewFinder : View
    {
        public Action PhotoButtonToggled;
        public Action InitializeCalled;


        public void Init()
        {
            InitializeCalled();
        }
    }
}
