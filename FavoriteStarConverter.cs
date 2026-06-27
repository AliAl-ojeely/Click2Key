using System;
using System.Globalization;
using System.Windows.Data;

namespace Click2Key
{
    public class FavoriteStarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        
            if(value is bool isFav)
                return isFav ? "★" : "☆";
            return "☆";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
