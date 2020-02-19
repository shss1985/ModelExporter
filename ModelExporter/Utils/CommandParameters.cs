using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace ModelExporter.Utils
{
    public class CommandParameters : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<object> objects = new List<object> { values[0] };
            return objects;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
