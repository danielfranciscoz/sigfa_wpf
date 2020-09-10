using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
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

namespace PruebaWPF.Views.Arqueo
{
    /// <summary>
    /// Lógica de interacción para EditarPago.xaml
    /// </summary>
    public partial class EditarPago : Window
    {
        ArqueoViewModel controller;

        private ReciboPagoSon reciboPago;
        private clsValidateInput validate = new clsValidateInput();
        public EditarPago()
        {
            InitializeComponent();
            Diseñar();

        }


        public EditarPago(ReciboPagoSon reciboPago)
        {
            this.reciboPago = reciboPago;
            InitializeComponent();
            Diseñar();
            Inicializar();
        }

        private void Diseñar()
        {
            clsUtilidades.Dialog_ModalDesign(this);
        }


        private void Inicializar()
        {
            controller = new ArqueoViewModel();
            CargarCombo();
            PanelFormaActual.DataContext = reciboPago;
            CamposNormales();
        }


        private void CamposNormales()
        {
            clsValidateInput.Validate(txtMonto, clsValidateInput.DecimalNumber);
            validate.AsignarBorderNormal(new Control[] { txtObservacionCambio, cboBanco, txtCuenta, txtNumeroCK, txtTarjeta, txtAutorizacion, cboTarjeta, txtBono, txtEmisor, txtTransaccion, cboTipo });
        }

        private void CargarCombo()
        {
            cboFormaPago.ItemsSource = controller.ObtenerFormasPago();
            CargarDatos();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int formapago = int.Parse(cboFormaPago.SelectedValue.ToString());
                if (ValidarFormaPago(formapago))
                {
                    Dictionary<TextBox, int> c = new Dictionary<TextBox, int>();
                    AgregarValidacionAdicional(c, formapago);

                    if (ValidarNumericos(c))
                    {
                        RectificarPago(formapago);
                    }
                }

            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void RectificarPago(int formaPago)
        {

            ReciboPagoSon rectificacion = new ReciboPagoSon();
            rectificacion.IdFormaPago = formaPago;

            ObtenerObjetoAdicional(formaPago, rectificacion);

            controller.RectificarPago(rectificacion, reciboPago, txtObservacionCambio.Text);
            ArquearApertura.cambios = true;
            Close();

        }

        private void cboFormaPago_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FormaPago selected = (FormaPago)cboFormaPago.SelectedItem;

            VerCamposAdicionales(selected);
        }

        private void VerCamposAdicionales(FormaPago formaPago)
        {
            switch (formaPago.IdFormaPago)
            {
                case 2: //Cheque
                    CargarBancos(isDeposito: false);
                    OcultarVerAdicionales(Cheque, new Panel[] { Tarjeta, Bono, EspacioVacio, Deposito });
                    break;
                case 3: //Tarjeta
                    CargarTarjetas();
                    OcultarVerAdicionales(Tarjeta, new Panel[] { Cheque, Bono, EspacioVacio, Deposito });
                    break;
                case 4: //Bono
                    OcultarVerAdicionales(Bono, new Panel[] { Tarjeta, Cheque, EspacioVacio, Deposito });
                    break;
                case 5: //Deposito
                    CargarBancos(isDeposito: true);
                    OcultarVerAdicionales(Deposito, new Panel[] { Tarjeta, Cheque, EspacioVacio, Bono });
                    break;
                default:
                    OcultarVerAdicionales(EspacioVacio, new Panel[] { Tarjeta, Bono, Cheque, Deposito });
                    break;
                    //Efectivo
            }
        }

        private void CargarBancos(bool isDeposito)
        {
            if (isDeposito)
            {
                if (cboBancoDeposito.ItemsSource == null)
                {
                    cboBancoDeposito.ItemsSource = controller.ObtenerBancos();

                }
            }
            else
            {
                if (cboBanco.ItemsSource == null)
                {
                    cboBanco.ItemsSource = controller.ObtenerBancos();

                }
            }
        }

        private void CargarTarjetas()
        {
            if (cboTarjeta.ItemsSource == null)
            {
                cboTarjeta.ItemsSource = controller.ObtenerTarjetas();

            }
        }

        private void CargarDatos()
        {
            if (reciboPago.ReciboPagoBono != null)
            {

            }
            else if (reciboPago.ReciboPagoCheque != null)
            {
                CargarBancos(isDeposito:false);
                ReciboPagoCheque cheque = reciboPago.ReciboPagoCheque;

                Cheque.DataContext = cheque;
            }
            else if (reciboPago.ReciboPagoTarjeta != null)
            {
                CargarTarjetas();
                ReciboPagoTarjeta tarjeta = reciboPago.ReciboPagoTarjeta;

                Tarjeta.DataContext = tarjeta;

                //Se habilitaran o desabilitaran los campos solo en caso de que el pago no se haya generado por medio de POS automatico
                clsUtilidades.HabilitaDesabilitaTodo((tarjeta.IdVoucherBanco == null), new Control[] { cboTarjeta, txtAutorizacion, txtTarjeta });

                if (tarjeta.IdVoucherBanco != null)
                {
                    panelWarningTarjeta.Visibility = Visibility.Visible;
                }
            }
            else if (reciboPago.ReciboPagoDeposito != null)
            {
                CargarBancos(isDeposito: true);
            }
        }

        private void OcultarVerAdicionales(Panel visible, Panel[] ocultos)
        {
            clsUtilidades.OcultarVerAdicionales(visible, ocultos);
        }

        private void AgregarValidacionAdicional(Dictionary<TextBox, int> c, int formapago)
        {
            switch (formapago)
            {
                case 2: //Cheque
                    //c.Add(txtNumeroCK, clsValidateInput.OnlyNumber);
                    break;
                case 3: //Tarjeta
                    c.Add(txtTarjeta, clsValidateInput.OnlyNumber);
                    c.Add(txtAutorizacion, clsValidateInput.OnlyNumber);
                    break;
                case 5: //Deposito
                    c.Add(txtTransaccion, clsValidateInput.Required);
                    break;
                default:
                    break;
                    //Efectivo
            }

        }

        private bool ValidarFormaPago(int formapago)
        {
            return clsValidateInput.ValidateALL(CamposAValidar(formapago));
        }


        private Control[] CamposAValidar(int formapago, bool iscleaning = false)
        {
            Control[] campos = new Control[4];

            campos[0] = txtObservacionCambio;
            switch (formapago)
            {
                case 2: //Cheque
                    campos[1] = cboBanco;
                    campos[2] = txtCuenta;
                    campos[3] = txtNumeroCK;

                    break;
                case 3: //Tarjeta
                    campos[1] = txtTarjeta;
                    campos[2] = txtAutorizacion;
                    campos[3] = cboTarjeta;
                    break;

                case 4: //Bono
                    campos[1] = txtBono;
                    campos[2] = txtEmisor;
                    break;
                case 5: //Deposito
                    campos[1] = txtTransaccion;
                    campos[2] = cboTipo;
                    campos[3] = cboBancoDeposito;
                    break;
                default:
                    break;
                    //Efectivo
            }

            return campos;

        }

        private bool ValidarNumericos(Dictionary<TextBox, int> campos)
        {

            return clsValidateInput.ValidateNumerics(campos);
        }


        private void ObtenerObjetoAdicional(int formapago, ReciboPagoSon rps)
        {
            //Object[] o = new Object[2];
            switch (formapago)
            {
                case 2: //Cheque
                    ReciboPagoCheque rc = new ReciboPagoCheque()
                    {
                        Banco = (Banco)cboBanco.SelectedItem,
                        IdBanco = Byte.Parse(cboBanco.SelectedValue.ToString()),
                        NumeroCK = txtNumeroCK.Text,
                        Cuenta = txtCuenta.Text
                    };

                    //o[0] = rc;
                    //o[1] = string.Format("{0}, Cuenta {1}, Cheque No.{2}", rc.Banco.Nombre, rc.Cuenta, rc.NumeroCK);
                    rps.ReciboPagoCheque = rc;
                    break;
                case 3: //Tarjeta

                    ReciboPagoTarjeta rt = new ReciboPagoTarjeta()
                    {
                        CiaTarjetaCredito = (CiaTarjetaCredito)cboTarjeta.SelectedItem,
                        IdTarjeta = Byte.Parse(cboTarjeta.SelectedValue.ToString()),
                        Tarjeta = txtTarjeta.Text,
                        Autorizacion = int.Parse(txtAutorizacion.Text.ToString()),
                        IdVoucherBanco = null
                    };

                    //o[0] = rt;
                    //o[1] = string.Format("{0}, Tarjeta ****{1} Autorización {2}", rt.CiaTarjetaCredito.Nombre, rt.Tarjeta, rt.Autorizacion);
                    rps.ReciboPagoTarjeta = rt;
                    break;
                case 4: //Bono
                    ReciboPagoBono rb = new ReciboPagoBono()
                    {
                        Emisor = txtEmisor.Text,
                        Numero = txtBono.Text
                    };

                    //o[0] = rb;
                    //o[1] = string.Format("Emitipo por {0}, Bono No.{1}", rb.Emisor, rb.Numero);
                    rps.ReciboPagoBono = rb;
                    break;
                case 5: //Deposito
                    ReciboPagoDeposito rd = new ReciboPagoDeposito()
                    {
                        Tipo = cboTipo.SelectedIndex == 0 ? false : true,
                        IdBanco = Byte.Parse(cboBancoDeposito.SelectedValue.ToString()),
                        Transaccion = txtTransaccion.Text,
                        Observacion = txtObservación.Text
                    };

                    //o[0] = rd;
                    //o[1] = string.Format("{0}, Transacción No.{1}, Obs. {2}", rd.Tipo ? "Transferencia" : "Minuta", rd.Transaccion, rd.Observacion);
                    rps.ReciboPagoDeposito = rd;
                    break;
                default://Efectivo
                    //o[0] = null;
                    //o[1] = "";
                    break;

            }

            //return o;
        }

    }
}
