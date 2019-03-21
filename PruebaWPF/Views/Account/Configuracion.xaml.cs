using PruebaWPF.Clases;
using PruebaWPF.Referencias;
using PruebaWPF.Views.Main;
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

namespace PruebaWPF.Views.Account
{
    /// <summary>
    /// Lógica de interacción para Configuracion.xaml
    /// </summary>
    public partial class Configuracion : Window
    {
        private clsValidateInput validate;
        clsConfiguration cf;

        public Configuracion()
        {
            validate = new clsValidateInput();
            InitializeComponent();

            cf = clsConfiguration.Actual();
            DataContext = cf;
            Diseñar();
            ActivarValidadorCampos();
        }

        private void Diseñar()
        {
            clsutilidades.Dialog_ModalDesign(this);

        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<TextBox, int> c = new Dictionary<TextBox, int>();
                c.Add(txtTopRow, clsValidateInput.OnlyNumber);

                if (ValidarNumericos(c))
                {
                    clsutilidades.OpenMessage(Guardar(), this);
                    Finalizar();
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void Finalizar()
        {
            frmMain.Refrescar();
            Close();
        }

        private Operacion Guardar()
        {
            cf.Save();
            return new Operacion { Mensaje = clsReferencias.MESSAGE_Exito_Save, OperationType = clsReferencias.TYPE_MESSAGE_Exito };
        }

        private void Restablecer()
        {
            clsConfiguration defaultConf = clsConfiguration.Default();

            cf.TopRow = defaultConf.TopRow;
            cf.AutoLoad = defaultConf.AutoLoad;
            cf.Sleep = defaultConf.Sleep;
            cf.AutoLoad = defaultConf.rememberMe;

            clsutilidades.UpdateControl(txtTopRow);
            clsutilidades.UpdateControl(Autoload);
            clsutilidades.UpdateControl(RememberMe);
            clsutilidades.UpdateControl(sldSleep);
        }

        private void ActivarValidadorCampos()
        {
            clsValidateInput.Validate(txtTopRow, clsValidateInput.OnlyNumber);
            validate.AsignarBorderNormal(new Control[] { txtTopRow });
        }

        private bool ValidarNumericos(Dictionary<TextBox, int> campos)
        {
            return clsValidateInput.ValidateNumerics(campos);
        }

        private void btnRestablecer_Click(object sender, RoutedEventArgs e)
        {
            Restablecer();
        }
    }
}
