using Kinpos.comm;
using Kinpos.Dcl;
using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PruebaWPF.Views.Tesoreria
{
    /// <summary>
    /// Lógica de interacción para POSCaja.xaml
    /// </summary>
    public partial class POSCaja : Window
    {

        private Pantalla pantalla;
        private Operacion operacion;
        private TesoreriaViewModel controller;
        private POSBanpro pos;
        clsValidateInput validate;

        public POSCaja()
        {
            InitializeComponent();

        }

        public POSCaja(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            operacion = new Operacion();
            controller = new TesoreriaViewModel(pantalla);
            validate = new clsValidateInput();

            InitializeComponent();
            Inicializar();
        }

        private void Inicializar()
        {
            Diseñar();
            CargarPuertosCOM();
            CamposNormales();


        }

        private void CamposNormales()
        {
            validate.AsignarBorderNormal(new Control[] { cboCOM });
        }

        private bool ValidarCampos()
        {
            return clsValidateInput.ValidateALL(new Control[] { cboCOM });
        }

        private void Diseñar()
        {
            clsUtilidades.Dialog_ModalDesign(this);
        }

        private void CargarPuertosCOM()
        {
            cboCOM.ItemsSource = clsUtilidades.GetSerialPorts();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos())
            {
                clsUtilidades.OpenMessage(Guardar(), this);
                Finalizar();
            }
        }

        private Operacion Guardar()
        {
            controller.SaveComPort(pos);
            return new Operacion { Mensaje = clsReferencias.MESSAGE_Exito_Save, OperationType = clsReferencias.TYPE_MESSAGE_Exito };
        }

        private void Finalizar()
        {
            Close();
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos())
            {
                SonarPOS();
            }
        }

        private void BtnCargarCOM_Click(object sender, RoutedEventArgs e)
        {
            CargarPuertosCOM();
        }

        private void CargarConfiguracionPorDefecto()
        {

            pos = controller.GetConfiguracionPOS();
            pos.Timeout = 10000; //altero el tiempo de respuesta solo para este caso porque no quiero que espere un minuto para comprobar la conexion cuando seleccionan el puerto incorrecto

            Configuracion.DataContext = pos;

        }

        public void SonarPOS()
        {
            DCL_RS232 dclRs232 = new DCL_RS232();

            dclRs232.Baudrate = pos.Baudrate;
            dclRs232.DataBits = pos.DataBits;
            dclRs232.Parity = pos.Parity;
            dclRs232.StopBits = pos.StopBits;
            dclRs232.Timeout = pos.Timeout;
            dclRs232.ProcessBIN = pos.ProcessBIN; //Este campo es false por defecto en el codigo de ejemplo, su valor no es ingresado por el usuario

            dclRs232.ComPort = pos.ComPort;

            DCL_Result returnedData = dclRs232.Beep();


            if (returnedData == null)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = "No hemos podido establecer comunicación con el POS, intenta cambiar el puerto COM, puedes recargar los puertos presionando el botón RECARGAR", OperationType = clsReferencias.TYPE_MESSAGE_Error, Titulo = "No se estableció la conexión" });
                btnSave.IsEnabled = false;
            }
            else
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = "Se logró establecer la conexión al POS, recuerde no mover el cable a otro puerto o podría ocasionar que los pagos con tarjeta no sean efectuados", OperationType = clsReferencias.TYPE_MESSAGE_Exito, Titulo = "Conexión exitosa" });
                btnSave.IsEnabled = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarConfiguracionPorDefecto();
        }

        private void CboCOM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnSave.IsEnabled = false;
            //pos.ComPort = cboCOM.SelectedItem.ToString();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (clsUtilidades.OpenDeleteQuestionMessage("Al eliminar la configuración ya no podrá realizar pagos automaticos entre el POS y el sistema, por lo tanto, ¿Realmente desea continuar?"))
            {
                pos.ComPort = null;
                clsUtilidades.OpenMessage(Guardar(), this);
                Finalizar();
            }
        }
    }
}

//VAL024PP