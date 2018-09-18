using PruebaWPF.Clases;
using PruebaWPF.Referencias;
using PruebaWPF.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PruebaWPF.Views.Shared
{
    /// <summary>
    /// Lógica de interacción para BoxMessage.xaml
    /// </summary>
    public partial class BoxMessage : Window
    {
        private Operacion operacion;
        public bool result = false;
        public BoxMessage(Operacion operacion)
        {
            InitializeComponent();
            InitializeEventsControl();
            this.operacion = operacion;
            Diseñar();

        }

        private void Diseñar()
        {
            clsutilidades.Dialog_ModalDesign(this);
        }

        private void InitializeEventsControl()
        {
            Msj.btnOk.Click += (sender, e) => Button_Click("Ok");
            Msj.btnNo.Click += (sender, e) => Button_Click("No");
            Msj.btnYes.Click += (sender, e) => Button_Click("Yes");
            Msj.btnCancel.Click += (sender, e) => Button_Click("Cancel");
        }

        private void Button_Click(string v)
        {
            switch (v)
            {
                case "Yes":
                    result = true;
                    this.Close();
                    break;
                default:
                    this.Close();
                    break;

            }
        }

        private void OpenMessage()
        {
            MessageCard mcard = new MessageCard(operacion.Mensaje, operacion.OperationType);
            this.LayoutMessage.DataContext = mcard;
            Iniciar();
        }

        private void OpenQuestionMessage()
        {
            MessageCard mcard = new MessageCard(operacion.Mensaje, operacion.OperationType);
            this.LayoutMessage.DataContext = mcard;
            Iniciar();
        }

        private void Iniciar()
        {
            SolidColorBrush colorFondo;

            switch (operacion.OperationType)
            {
                case clsReferencias.TYPE_MESSAGE_Exito:
                    {
                        colorFondo = (SolidColorBrush)FindResource("OkMsj");

                        break;
                    }
                case clsReferencias.TYPE_MESSAGE_Error:
                    {
                        colorFondo = (SolidColorBrush)FindResource("ErrorMsj");
                        break;
                    }
                case clsReferencias.TYPE_MESSAGE_Advertencia:
                    {
                        colorFondo = (SolidColorBrush)FindResource("WarningMsj");
                         break;
                    }
                case clsReferencias.TYPE_MESSAGE_Question:
                    {
                        colorFondo = (SolidColorBrush)FindResource("QuestionMsj");
                        break;
                    }
                default:
                    {
                        colorFondo = (SolidColorBrush)FindResource("DarkGray");
                        break;
                    }

            }

            Msj.icon.Kind = operacion.Icon();

            if (operacion.OperationType == clsReferencias.TYPE_MESSAGE_Question)
            {
                Msj.btnOk.Visibility = Visibility.Collapsed;
                Msj.btnYes.Foreground = new SolidColorBrush(colorFondo.Color);
                Msj.btnNo.Foreground = new SolidColorBrush(colorFondo.Color);
                Msj.btnCancel.Focus();
            }
            else
            {
                Msj.btnYes.Visibility = Visibility.Hidden;
                Msj.btnNo.Visibility = Visibility.Hidden;
                Msj.btnCancel.Visibility = Visibility.Hidden;
                Msj.btnOk.Focus();
            }

            Msj.btnOk.Foreground = new SolidColorBrush(colorFondo.Color);
            Msj.btnCancel.Foreground = new SolidColorBrush(colorFondo.Color);
            Msj.icon.Foreground = new SolidColorBrush(colorFondo.Color);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenMessage();
        }

    }
}
