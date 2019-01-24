using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PruebaWPF.Clases
{
    class clsValidateInput : TextBox
    {
        public const int OnlyNumber = 0;
        public const int DecimalNumber = 2;

        SolidColorBrush c = clsutilidades.BorderNormal();

        public static void Validate(TextBox textbox, int validationtype)
        {

            textbox.PreviewTextInput += (sender, e) =>
            {
                Regex regex;
                Boolean flag;
                switch (validationtype)
                {
                    case OnlyNumber:
                        regex = new Regex("[^0-9]+");
                        e.Handled = regex.IsMatch(e.Text);
                        break;
                    case DecimalNumber:
                        //CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                        //regex = new Regex("[^0-9" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToString() + "]+");
                        //e.Handled = regex.IsMatch(e.Text);
                        char punto = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                        char c = char.Parse(e.Text);
                        if (!Char.IsDigit(c) && c != punto)
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }
                        e.Handled = flag;

                        break;
                }
            };

        }

        public void AsignarBorderNormal(Control[] campos)
        {

            for (int i = 0; i < campos.Length; i++)
            {
                TextBox txt = null;
                ComboBox cbo = null;

                if (campos[i] is TextBox)
                {
                    txt = (TextBox)campos[i];
                    txt.GotFocus += (sender, e) =>
                   {
                        txt.BorderBrush = c;
                    };
                }
                else
                if (campos[i] is ComboBox)
                {
                    cbo = (ComboBox)campos[i];

                    cbo.SelectionChanged += (sender, e) =>
                    {
                        cbo.BorderBrush = c;
                    };
                }
            }

        }

        public static bool ValidarSeleccion(ComboBox combo)
        {
            bool flag = true;
            if (combo.SelectedIndex == -1)
            {
                combo.BorderBrush = clsutilidades.BorderError();
                flag = false;
            }

            return flag;
        }

        public static void ActivateBorderError(Control campo)
        {
            campo.BorderBrush = clsutilidades.BorderError();
        }

        public static bool ValidateALL(Control[] campos)
        {
            bool flag = true;
            TextBox txt;
            ComboBox cbo;
            for (int i = 0; i < campos.Length; i++)
            {
                if (campos[i] is TextBox)
                {
                    txt = (TextBox)campos[i];
                    if (txt.Text.Equals(""))
                    {
                        flag = false;
                        txt.BorderBrush = clsutilidades.BorderError();
                    }
                }
                else
                if (campos[i] is ComboBox)
                {
                    cbo = (ComboBox)campos[i];
                    if (!ValidarSeleccion(cbo))
                    {
                        flag = false;
                    }
                }
            }

            return flag;
        }

        public static bool ValidateNumerics(Dictionary<TextBox, int> campos)
        {
            bool flag = true;
            TextBox txt;
            double valorDouble;
            int valorInt;
            foreach (KeyValuePair<TextBox, int> field in campos)
            {
                txt = (TextBox)field.Key;
                switch (field.Value)
                {
                    case OnlyNumber:
                        if (!int.TryParse(txt.Text, out valorInt))
                        {
                            flag = false;
                            txt.BorderBrush = clsutilidades.BorderError();
                        }
                        break;

                    case DecimalNumber:
                        if (!double.TryParse(txt.Text, out valorDouble))
                        {
                            flag = false;
                            txt.BorderBrush = clsutilidades.BorderError();
                        }
                        break;
                }
            }

            return flag;
        }

        public static void CleanALL(Control[] campos)
        {

            TextBox txt;
            ComboBox cbo;
            for (int i = 0; i < campos.Length; i++)
            {
                if (campos[i] is TextBox)
                {
                    txt = (TextBox)campos[i];
                    txt.Text = "";
                }
                else
                if (campos[i] is ComboBox)
                {
                    cbo = (ComboBox)campos[i];
                    cbo.SelectedValue = null;
                }
            }
        }

    }
}
