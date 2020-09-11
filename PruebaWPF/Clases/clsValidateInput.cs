using MaterialDesignExtensions.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;

namespace PruebaWPF.Clases
{
    class clsValidateInput : TextBox
    {
        public const int OnlyNumber = 0;
        public const int DecimalNumber = 2;
        public const int Porcentaje = 3;
        public const int Required = 4;

        SolidColorBrush c = clsUtilidades.BorderNormal();

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
                    case Required:
                        flag = string.IsNullOrEmpty(e.Text);
                        break;
                        e.Handled = flag;
                }
            };

        }

        public void AsignarBorderNormal(Control[] campos)
        {

            for (int i = 0; i < campos.Length; i++)
            {
                TextBox txt = null;
                PasswordBox pss = null;
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
                else if (campos[i] is PasswordBox)
                {
                    pss = (PasswordBox)campos[i];
                    pss.GotFocus += (sender, e) =>
                    {
                        pss.BorderBrush = c;
                    };
                }
            }

        }

        public static bool ValidarSeleccion(ComboBox combo)
        {
            bool flag = true;
            if (combo.SelectedIndex == -1)
            {
                combo.BorderBrush = clsUtilidades.BorderError();
                flag = false;
            }

            return flag;
        }

        public static bool ValidarSeleccion(Autocomplete combo)
        {
            bool flag = true;
            if (combo.SelectedItem == null)
            {
                combo.BorderBrush = clsUtilidades.BorderError();
                flag = false;
            }

            return flag;
        }

        public static void ActivateBorderError(Control campo)
        {
            campo.BorderBrush = clsUtilidades.BorderError();
        }

        public static bool ValidateALL(Control[] campos)
        {
            bool flag = true;
            TextBox txt;
            PasswordBox pss;
            ComboBox cbo;
            for (int i = 0; i < campos.Length; i++)
            {
                if (campos[i] is TextBox)
                {
                    txt = (TextBox)campos[i];
                    if (txt.Text.Equals(""))
                    {
                        flag = false;
                        txt.BorderBrush = clsUtilidades.BorderError();
                    }
                }
                else if (campos[i] is ComboBox)
                {
                    cbo = (ComboBox)campos[i];
                    if (!ValidarSeleccion(cbo))
                    {
                        flag = false;
                    }
                }
                else if (campos[i] is PasswordBox)
                {
                    pss = (PasswordBox)campos[i];
                    if (pss.Password.Equals(""))
                    {
                        flag = false;
                        pss.BorderBrush = clsUtilidades.BorderError();
                    }
                }
                else if (campos[i] is Autocomplete)
                {
                    Autocomplete auto = (Autocomplete)campos[i];
                    if (!ValidarSeleccion(auto))
                    {
                        flag = false;
                    }

                }
            }

            return flag;
        }

        public static bool ValidateMayorCero(TextBox[] campos)
        {
            bool flag = true;
            TextBox txt;
            for (int i = 0; i < campos.Length; i++)
            {
                txt = (TextBox)campos[i];
                if (txt.Text.Equals("0"))
                {
                    flag = false;
                    txt.BorderBrush = clsUtilidades.BorderError();
                }
            }

            return flag;
        }


        public static bool ValidateLength(TextBox campo, int MaxLength, bool isMaxMin)
        {
            bool flag = true;

            if (campo.Text.Length > MaxLength)
            {
                flag = false;
                campo.BorderBrush = clsUtilidades.BorderError();
            }
            else if (campo.Text.Length < MaxLength && isMaxMin)
            {
                flag = false;
                campo.BorderBrush = clsUtilidades.BorderError();
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
                            txt.BorderBrush = clsUtilidades.BorderError();
                        }
                        break;

                    case DecimalNumber:
                        if (!double.TryParse(txt.Text, out valorDouble))
                        {
                            flag = false;
                            txt.BorderBrush = clsUtilidades.BorderError();
                        }
                        break;

                    case Porcentaje:
                        if (!double.TryParse(txt.Text, out valorDouble))
                        {
                            flag = false;
                            txt.BorderBrush = clsUtilidades.BorderError();
                        }
                        else
                        {
                            if (valorDouble > 100)
                            {
                                flag = false;
                                txt.BorderBrush = clsUtilidades.BorderError();
                            }
                        }
                        break;
                }
            }

            return flag;
        }

        public static bool ValidateEmail(TextBox campo)
        {
            bool flag = true;

            String expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(campo.Text, expresion))
            {
                if (Regex.Replace(campo.Text, expresion, String.Empty).Length == 0)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                    campo.BorderBrush = clsUtilidades.BorderError();
                }
            }
            else
            {
                flag = false;
                campo.BorderBrush = clsUtilidades.BorderError();
            }

            return flag;
        }

        public static bool ValidateEmail(string campo)
        {
            bool flag = true;

            String expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(campo, expresion))
            {
                if (Regex.Replace(campo, expresion, String.Empty).Length == 0)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                flag = false;
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
