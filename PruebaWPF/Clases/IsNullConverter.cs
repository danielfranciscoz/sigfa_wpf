using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PruebaWPF.Clases
{
    public class IsNullConverter : IValueConverter
    {
        public char Separator { get; set; } = ';';

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {

            }
            var strings = ((string)parameter).Split(Separator);
            var trueString = strings[0];
            var falseString = strings[1];

            if (value == null)
            {
                return trueString;
            }
            else if (string.IsNullOrEmpty(value.ToString()))
            {
                return trueString;
            }
            {
                return falseString;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Esta operación no se encuentra implementada.");
        }
    }
}
