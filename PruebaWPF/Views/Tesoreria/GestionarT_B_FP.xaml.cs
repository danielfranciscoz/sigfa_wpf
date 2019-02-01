using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using PruebaWPF.Views.Recibo;
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

namespace PruebaWPF.Views.Tesoreria
{
    /// <summary>
    /// Lógica de interacción para GestionarTarjeta.xaml
    /// </summary>
    public partial class GestionarT_B_FP : Window
    {
        clsValidateInput validate = new clsValidateInput();
        TesoreriaViewModel controller;
        string gestion;

        private Pantalla pantalla;
        private CiaTarjetaCredito tarjeta;
        private Banco banco;
        private FormaPago formapago;
        private FuenteFinanciamiento ff;
        private Moneda moneda;
        private IdentificacionAgenteExterno identificacion;
        private InfoRecibo info;
        private InfoReciboSon son;
        public GestionarT_B_FP()
        {
            InitializeComponent();
        }

        public GestionarT_B_FP(CiaTarjetaCredito tarjeta, Pantalla pantalla)
        {
            gestion = "Tarjeta";
            this.tarjeta = tarjeta;
            this.pantalla = pantalla;
            controller = new TesoreriaViewModel(pantalla);

            InitializeComponent();
            Diseñar();
            TituloCuerpo(tarjeta.IdCiaTarjetaCredito.ToString());
            DataContext = tarjeta;
        }

        public GestionarT_B_FP(Banco banco, Pantalla pantalla)
        {
            gestion = "Banco";
            this.banco = banco;
            this.pantalla = pantalla;
            controller = new TesoreriaViewModel(pantalla);

            InitializeComponent();
            Diseñar();
            TituloCuerpo(banco.IdBanco.ToString());
            DataContext = banco;
        }

        public GestionarT_B_FP(FormaPago formaPago, Pantalla pantalla)
        {
            gestion = "Forma de Pago";
            this.formapago = formaPago;
            this.pantalla = pantalla;
            controller = new TesoreriaViewModel(pantalla);

            InitializeComponent();
            Diseñar();
            TituloCuerpo(formaPago.IdFormaPago.ToString());
            DataContext = formaPago;
        }

        public GestionarT_B_FP(FuenteFinanciamiento ff, Pantalla pantalla)
        {
            gestion = "Fuente de Financiamiento";
            this.ff = ff;
            this.pantalla = pantalla;
            controller = new TesoreriaViewModel(pantalla);

            InitializeComponent();
            Diseñar();
            TituloCuerpo(ff.IdFuenteFinanciamiento.ToString());
            CargarCombo();
            DataContext = ff;
        }

        public GestionarT_B_FP(Moneda moneda, Pantalla pantalla)
        {
            gestion = "Moneda";
            this.moneda = moneda;
            this.pantalla = pantalla;
            controller = new TesoreriaViewModel(pantalla);

            InitializeComponent();
            Diseñar();
            TituloCuerpo(moneda.IdMoneda.ToString());
            DataContext = moneda;
        }

        public GestionarT_B_FP(IdentificacionAgenteExterno identificacion, Pantalla pantalla)
        {
            gestion = "Documento de Identidad";
            this.identificacion = identificacion;
            this.pantalla = pantalla;
            controller = new TesoreriaViewModel(pantalla);

            InitializeComponent();
            Diseñar();
            TituloCuerpo(identificacion.IdIdentificacion.ToString());
            DataContext = identificacion;
        }

        public GestionarT_B_FP(InfoRecibo info, InfoReciboSon son, Pantalla pantalla, string PermisoName)
        {
            gestion = "Encabezado y Pie de Recibo";
            this.info = info;
            this.son = son;
            this.pantalla = pantalla;
            controller = new TesoreriaViewModel(pantalla);

            InitializeComponent();
            Diseñar();
            TituloCuerpo(info.IdInfoRecibo.ToString());
            DataContext = info;
            CargarRecintos(PermisoName);
        }

        private void Diseñar()
        {
            clsutilidades.Dialog_ModalDesign(this);
        }

        private void CargarCombo()
        {
            cboFuentesSIPPSI.ItemsSource = controller.FindAllFuentesSIPSSI();
            cboFuentesSIPPSI.SelectedValue = ff.IdFuente_SIPPSI;

        }
        private void CargarRecintos(string PermisoName)
        {

            if (info.IdInfoRecibo != 0)
            {

                vw_RecintosRH recinto = new vw_RecintosRH() { IdRecinto = info.IdRecinto, Siglas = son.Recinto };
                cboRecinto.Items.Add(recinto);
                cboRecinto.IsEnabled = false;
            }
            else
            {
                List<vw_RecintosRH> recintos = controller.RecintosInfo(PermisoName);
                cboRecinto.ItemsSource = recintos;
            }
            cboRecinto.SelectedValue = info.IdRecinto;
        }


        private void TituloCuerpo(string id)
        {
            //El id lo paso string debido a que en ocasiones es byte y en otras int

            switch (gestion)
            {
                case "Tarjeta":
                    panelTarjeta.Visibility = Visibility.Visible;
                    validate.AsignarBorderNormal(new Control[] { txtTarjeta, txtSiglasTarjeta });
                    break;
                case "Banco":
                    panelBanco.Visibility = Visibility.Visible;
                    validate.AsignarBorderNormal(new Control[] { txtBanco, txtSiglasBanco });
                    break;
                case "Forma de Pago":
                    panelFormaPago.Visibility = Visibility.Visible;
                    validate.AsignarBorderNormal(new Control[] { txtFormaPago });
                    break;
                case "Fuente de Financiamiento":
                    panelFF.Visibility = Visibility.Visible;
                    validate.AsignarBorderNormal(new Control[] { txtNombreFF, txtSiglasFF });
                    break;
                case "Moneda":
                    panelMoneda.Visibility = Visibility.Visible;
                    validate.AsignarBorderNormal(new Control[] { txtMoneda, txtSimbolo });
                    break;

                case "Documento de Identidad":
                    panelIdentificacion.Visibility = Visibility.Visible;
                    clsValidateInput.Validate(txtMaxCaracteres, clsValidateInput.OnlyNumber);
                    validate.AsignarBorderNormal(new Control[] { txtIdentificacion, txtMaxCaracteres });
                    break;

                case "Encabezado y Pie de Recibo":
                    panelInfoRecibo.Visibility = Visibility.Visible;
                    btnPreview.Visibility = Visibility.Visible;
                    validate.AsignarBorderNormal(new Control[] { txtEncabezado, txtPie, cboRecinto });
                    break;
                default:
                    Close();
                    break;
            }

            txtTitle.Text = id.Equals("0") ? string.Format("Agregar {0}", gestion) : string.Format("Editar {0}", gestion);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCampos())
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
            Tesoreria.Cambios = true;
            frmMain.Refrescar();
            Close();
        }

        private Operacion Guardar()
        {
            Operacion o;
            switch (gestion)
            {
                case "Tarjeta":
                    controller.SaveUpdateTarjeta(tarjeta);
                    break;
                case "Banco":
                    controller.SaveUpdateBanco(banco);
                    break;
                case "Forma de Pago":
                    controller.SaveUpdateFormaPago(formapago);
                    break;
                case "Fuente de Financiamiento":
                    if (cboFuentesSIPPSI.SelectedIndex != -1)
                    {
                        ff.IdFuente_SIPPSI = byte.Parse(cboFuentesSIPPSI.SelectedValue.ToString());
                    }
                    controller.SaveUpdateFF(ff);
                    break;
                case "Moneda":
                    controller.SaveUpdateMoneda(moneda);
                    break;

                case "Documento de Identidad":
                    controller.SaveUpdateIdentificacion(identificacion);
                    break;
                case "Encabezado y Pie de Recibo":
                    info.IdRecinto = int.Parse(cboRecinto.SelectedValue.ToString());
                    controller.SaveInfoRecibo(info);
                    break;
                default:
                    break;
            }

            o = new Operacion { Mensaje = clsReferencias.MESSAGE_Exito_Save, OperationType = clsReferencias.TYPE_MESSAGE_Exito };

            return o;
        }

        private bool ValidarCampos()
        {
            Boolean flag;
            switch (gestion)
            {
                case "Tarjeta":
                    flag = clsValidateInput.ValidateALL(new Control[] { txtTarjeta, txtSiglasTarjeta });
                    break;
                case "Banco":
                    flag = clsValidateInput.ValidateALL(new Control[] { txtBanco, txtSiglasBanco });
                    break;
                case "Forma de Pago":
                    flag = clsValidateInput.ValidateALL(new Control[] { txtFormaPago });
                    break;
                case "Fuente de Financiamiento":
                    flag = clsValidateInput.ValidateALL(new Control[] { txtNombreFF, txtSiglasFF });
                    break;
                case "Moneda":
                    flag = clsValidateInput.ValidateALL(new Control[] { txtMoneda, txtSimbolo });
                    break;
                case "Documento de Identidad":
                    flag = clsValidateInput.ValidateALL(new Control[] { txtIdentificacion, txtMaxCaracteres });
                    if (flag)
                    {
                        if (identificacion.MaxCaracteres == 0)
                        {
                            txtMaxCaracteres.BorderBrush = clsutilidades.BorderError();
                            flag = false;
                        }
                    }
                    break;
                case "Encabezado y Pie de Recibo":
                    flag = clsValidateInput.ValidateALL(new Control[] { txtEncabezado, txtPie, cboRecinto });
                    break;
                default:
                    flag = true;
                    break;
            }
            return flag;
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos())
            {
                rptRecibo boucher = new rptRecibo(info, true);
                boucher.ShowDialog();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            switch (gestion)
            {
                case "Tarjeta":

                    break;
                case "Banco":

                    break;
                case "Forma de Pago":

                    break;
                case "Fuente de Financiamiento":

                    break;
                case "Moneda":

                    break;
                case "Documento de Identidad":

                    break;
                case "Encabezado y Pie de Recibo":
                    if (cboRecinto.Items.Count == 0)
                    {
                        clsutilidades.OpenMessage(new Operacion() { Mensaje = "Actualmente no puede agregar un nuevo registro debido a que no se ha encontrado un nuevo recinto al cual crear el encabezado y pie de recibo, esta información es basada en sus permisos de usuario.", OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                        Close();
                    }
                    break;
                default:

                    break;
            }
        }
    }
}