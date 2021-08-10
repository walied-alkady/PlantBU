using PlantBU.DataModel;
using Syncfusion.DataSource;
using Syncfusion.DataSource.Extensions;
using System;
using System.Linq;

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PlantBU.Utilities
{
    class GroupHeaderCountFinishedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var area = value as string;
            if (!string.IsNullOrEmpty(area))
                return DBManager.realm.All<Schedule>().Where(x => x.Area == area && x.StatusSchedule == true).ToList().Count;
            else
                return 0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
