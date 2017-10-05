using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSharingApp.AppService.Shared.Models
{
    static class Settings
    {
        private static string imageBaseUrl = null;
        public static string ImageBaseUrl {
            get
            {
                if (imageBaseUrl == null)
                    imageBaseUrl = ConfigurationManager.AppSettings["ImageBaseUrl"];

                return imageBaseUrl;
            }
            set
            {
                imageBaseUrl = value;
            }
        }
    }
}
