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
    class GroupHeaderPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var area = value as string;
            double xvalue = DBManager.realm.All<Schedule>().Count(x => x.Area == area && x.StatusSchedule == true);
            double xvalue1 = DBManager.realm.All<Schedule>().Count(x => x.Area == area);
           
            if (!string.IsNullOrEmpty(area) && xvalue1!= 0)
            {
                int result = System.Convert.ToInt32(xvalue / xvalue1 * 100);
                return result;
            }
               
            else
                return 0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
