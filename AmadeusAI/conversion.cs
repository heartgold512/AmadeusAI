using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace AmadeusAI
{
    public class Conversion : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double scaleY)
            {
                // Assuming baseTopMargin is the base top margin you want to apply
                double baseTopMargin = 10; // Replace with your actual base margin
                return new Thickness(0, baseTopMargin * scaleY, 0, 0);
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
