using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace XRayOperatorTool.Converters
{
    [ValueConversion(typeof(string), typeof(BitmapFrame))]
    public class ImageSourceLoader : IValueConverter
    {
        public static ImageSourceLoader Instance = new ImageSourceLoader();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value)
            {
                return null;
            }
            // var path = string.Format((string)parameter, value.ToString());
            var path = (string)value;
            return BitmapFrame.Create(new Uri(path, UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
