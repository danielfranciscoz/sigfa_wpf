using System;
using System.Globalization;
using System.Linq;

namespace PruebaWPF.Clases
{
    public class BoolToStringConverter : System.Windows.Data.IValueConverter
    {
        public char Separator { get; set; } = ';';

        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            string[] strings;

            bool boolValue = false;

            if (value != null)
            {
                boolValue = (bool)value;
            }


            if (parameter.ToString().Contains(';'))
            {
                strings = ((string)parameter).Split(Separator);

                if (boolValue)
                {
                    return strings[0]; //Verdadero
                }
                else
                {
                    return strings[1]; //Falso
                }
            }
            else
            {

                return "";

            }

            //var trueString = strings[0];
            //var falseString = strings[1];


            //if (boolValue)
            //{
            //    return trueString;
            //}
            //else
            //{
            //    return falseString;
            //}
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            var strings = ((string)parameter).Split(Separator);
            var trueString = strings[0];
            var falseString = strings[1];

            var stringValue = (string)value;
            if (stringValue == trueString)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
