using System;
using System.Globalization;
using Xamarin.Forms;

namespace PlantBU.Utilities
{
    class BoolToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return ImageSource.FromResource("PlantBU.Images.GroupExpand.png");
            else
                return ImageSource.FromResource("PlantBU.Images.GroupCollapse.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
