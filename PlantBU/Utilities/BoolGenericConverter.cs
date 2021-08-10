using System;
using System.Globalization;
using Xamarin.Forms;

namespace PlantBU.Utilities
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BoolGenericConverter<T>'
    public class BoolGenericConverter<T> : IValueConverter
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BoolGenericConverter<T>'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BoolGenericConverter<T>.TrueObject'
        public T TrueObject { set; get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BoolGenericConverter<T>.TrueObject'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BoolGenericConverter<T>.FalseObject'
        public T FalseObject { set; get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BoolGenericConverter<T>.FalseObject'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BoolGenericConverter<T>.Convert(object, Type, object, CultureInfo)'
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BoolGenericConverter<T>.Convert(object, Type, object, CultureInfo)'
        {
            return (bool)value ? TrueObject : FalseObject;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BoolGenericConverter<T>.ConvertBack(object, Type, object, CultureInfo)'
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BoolGenericConverter<T>.ConvertBack(object, Type, object, CultureInfo)'
        {
            return ((T)value).Equals(TrueObject);
        }
    }
}
