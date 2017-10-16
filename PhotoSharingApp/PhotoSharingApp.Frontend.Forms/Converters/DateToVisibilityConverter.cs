using System;
using System.Globalization;
using Xamarin.Forms;

namespace PhotoSharingApp.Forms.Converters
{
    public class DateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
                return !((DateTime)value == default(DateTime));

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
