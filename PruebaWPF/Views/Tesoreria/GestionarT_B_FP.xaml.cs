using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using PruebaWPF.Views.Recibo;
using PruebaWPF.Views.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
        private CuentaContableVariacion variacion;
        private CuentaContable selectedCuenta;
        private MovimientoIngreso movimiento;
        private ObservableCollection<DetalleMovimientoIngreso> detalleMovimientoIngreso;
        private List<DetalleMovimientoIngreso> detalleDeleted;

        #region Gestiones
        private const string Tarjeta = "Tarjeta";
        private const string Banco = "Banco";
        private const string FormadePago = "Forma de Pago";
        private const string FuentedeFinanciamiento = "Fuente de Financiamiento";
        private const string Moneda = "Moneda";
        private const string DocumentodeIdentidad = "Documento de Identidad";
        private const string EncabezadoyPiedeRecibo = "Encabezado y Pie de Recibo";
        private const string ParametrizaciondeVariaciones = "Parametrización de Variaciones";
        private const string ParametrizaciondeMovimientosdeIngreso = "Parametrización de Movimientos de Ingreso";
        #endregion
        public GestionarT_B_FP()
        {
            InitializeComponent();
        }

        public GestionarT_B_FP(CiaTarjetaCredito tarjeta, Pantalla pantalla)
        {
            gestion = Tarjeta;
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
            gestion = Banco;
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
            gestion = FormadePago;
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
            gestion = FuentedeFinanciamiento;
            this.ff = ff;
            this.pantalla = pantalla;
            controller = new TesoreriaViewModel(pantalla);

            InitializeComponent();
            Diseñar();
            TituloCuerpo(ff.IdFuenteFinanciamiento.ToString());
            CargarFuentes();
            DataContext = ff;
        }

        public GestionarT_B_FP(Moneda moneda, Pantalla pantalla)
        {
            gestion = Moneda;
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
            gestion = DocumentodeIdentidad;
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
            gestion = EncabezadoyPiedeRecibo;
            this.info = info;
            this.son = son;
            this.pantalla = pantalla;
            controller = new TesoreriaViewModel(pantalla);

            InitializeComponent();
            Diseñar();
            TituloCuerpo(info.IdInfoRecibo.ToString());
            DataContext = info;
            CargarRecintos(PermisoName, cboRecinto, info.IdInfoRecibo == 0 ? null : new vw_RecintosRH() { IdRecinto = info.IdRecinto, Siglas = son.Recinto }, info);
        }

        public GestionarT_B_FP(CuentaContableVariacion variacion, Pantalla pantalla)
        {

            gestion = ParametrizaciondeVariaciones;
            this.variacion = variacion;
            this.pantalla = pantalla;
            controller = new TesoreriaViewModel(pantalla);

            InitializeComponent();
            Diseñar();
            TituloCuerpo(variacion.cuenta == null ? "0" : variacion.cuenta.IdCuentaContable.ToString());
            DataContext = variacion;
            CargarTipoVariacion(variacion?.variacion);
        }

        public GestionarT_B_FP(MovimientoIngreso movimiento, List<DetalleMovimientoIngreso> detalleMovimientoIngreso, Pantalla pantalla, string PermisoName)
        {
            //TODO CRUD Parametrizacion contable
            gestion = ParametrizaciondeMovimientosdeIngreso;
            this.movimiento = movimiento;
            this.pantalla = pantalla;
            controller = new TesoreriaViewModel(pantalla);

            InitializeComponent();
            InicializarDetalleMovimientoIngreso(detalleMovimientoIngreso);
            Diseñar();
            TituloCuerpo(movimiento.IdMovimientoIngreso.ToString());

            CargarRecintos(PermisoName, cboRecintoParametrizacion, movimiento.IdMoneda == 0 ? null : new vw_RecintosRH() { IdRecinto = movimiento.IdRecinto, Siglas = movimiento.Recinto }, movimiento);

        }

        private void InicializarDetalleMovimientoIngreso(List<DetalleMovimientoIngreso> detalle)
        {
            if (detalle != null)
            {
                detalleMovimientoIngreso = new ObservableCollection<DetalleMovimientoIngreso>(detalle);
                CalcularTotales();
            }
            else
            {
                detalleMovimientoIngreso = new ObservableCollection<DetalleMovimientoIngreso>();
            }
            detalleDeleted = new List<DetalleMovimientoIngreso>();
            detalleMovimientoIngreso.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Detalles_CollectionChanged);
        }

        private void Detalles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                DetalleMovimientoIngreso d = (DetalleMovimientoIngreso)tblDetalleMovimientos.CurrentItem;
                if (d.IdDetalleMovimientoIngreso != 0)
                {
                    detalleDeleted.Add(d);
                }
            }
            CalcularTotales();
        }

        private void CalcularTotales()
        {
            var sumas = detalleMovimientoIngreso.GroupBy(g => 0).Select(s => new { Debe = s.Sum(c => c.Debe ?? 0), Haber = s.Sum(c => c.Haber ?? 0) }).ToArray();
            txtDebe.Text = sumas[0]?.Debe.ToString("0.00%");
            txtHaber.Text = sumas[0]?.Haber.ToString("0.00%");
        }

        private void Diseñar()
        {
            clsUtilidades.Dialog_ModalDesign(this);
        }

        private void CargarTipoVariacion(Configuracion llave)
        {
            cboLlavesVariacion.ItemsSource = controller.FindAllTipoVariacion(llave);
            if (llave != null)
            {
                cboLlavesVariacion.SelectedValue = llave.Llave;
                cboLlavesVariacion.IsEnabled = false;

                SeleccionarCuentaVariacion(variacion.cuenta);
            }
        }

        private void CargarFuentes()
        {
            cboFuentesSIPPSI.ItemsSource = controller.FindAllFuentesSIPSSI();
            cboFuentesSIPPSI.SelectedValue = ff.IdFuente_SIPPSI;

        }
        private void CargarRecintos(string PermisoName, ComboBox cbo, vw_RecintosRH recinto, Object element)
        {

            if (recinto != null)
            {
                cbo.Items.Add(recinto);
                cbo.IsEnabled = false;
            }
            else
            {
                List<vw_RecintosRH> recintos;

                if (element.GetType() == typeof(InfoRecibo))
                {
                    recintos = controller.RecintosInfo(PermisoName);
                }
                else
                {
                    recintos = controller.Recintos(PermisoName);
                }
                cbo.ItemsSource = recintos;
            }
            cbo.SelectedValue = recinto?.IdRecinto;
        }

        private void CargaUpdateParametrizacion()
        {
            if (movimiento.IdMovimientoIngreso != 0)
            {
                cboFormaPago.Items.Add(movimiento.FormaPago);
                cboMoneda.Items.Add(movimiento.Moneda);

                cboFormaPago.SelectedValue = movimiento.IdFormaPago;
                cboMoneda.SelectedValue = movimiento.IdMoneda;
                cboMoneda.IsEnabled = false;
                cboFormaPago.IsEnabled = false;
            }
            else
            {
                detalleMovimientoIngreso.Insert(0, new DetalleMovimientoIngreso()
                {
                    canDelete = false,
                    Naturaleza = clsReferencias.Haber,
                    FactorPorcentual = 1,
                    CuentaContable = new CuentaContable()
                    {
                        CuentaContable1 = "4",
                        Descripcion = "INGRESOS DEL RECIBO",
                        TipoCuenta = new TipoCuenta()
                        {
                            TipoCuenta1 = "INGRESOS (Autogenerado)"
                        }
                    }
                });
            }

            tblDetalleMovimientos.ItemsSource = detalleMovimientoIngreso;
        }

        private void TituloCuerpo(string id)
        {
            //El id lo paso string debido a que en ocasiones es byte y en otras int

            switch (gestion)
            {
                case Tarjeta:
                    panelTarjeta.Visibility = Visibility.Visible;
                    validate.AsignarBorderNormal(new Control[] { txtTarjeta, txtSiglasTarjeta });
                    break;
                case Banco:
                    panelBanco.Visibility = Visibility.Visible;
                    validate.AsignarBorderNormal(new Control[] { txtBanco, txtSiglasBanco });
                    break;
                case FormadePago:
                    panelFormaPago.Visibility = Visibility.Visible;
                    validate.AsignarBorderNormal(new Control[] { txtFormaPago });
                    break;
                case FuentedeFinanciamiento:
                    panelFF.Visibility = Visibility.Visible;
                    validate.AsignarBorderNormal(new Control[] { txtNombreFF, txtSiglasFF });
                    break;
                case Moneda:
                    panelMoneda.Visibility = Visibility.Visible;
                    validate.AsignarBorderNormal(new Control[] { txtMoneda, txtSimbolo });
                    break;

                case DocumentodeIdentidad:
                    panelIdentificacion.Visibility = Visibility.Visible;
                    clsValidateInput.Validate(txtMaxCaracteres, clsValidateInput.OnlyNumber);
                    validate.AsignarBorderNormal(new Control[] { txtIdentificacion, txtMaxCaracteres });
                    break;

                case EncabezadoyPiedeRecibo:
                    panelInfoRecibo.Visibility = Visibility.Visible;
                    btnPreview.Visibility = Visibility.Visible;
                    validate.AsignarBorderNormal(new Control[] { txtEncabezado, txtPie, cboRecinto });
                    break;
                case ParametrizaciondeVariaciones:
                    panelVariacion.Visibility = Visibility.Visible;
                    validate.AsignarBorderNormal(new Control[] { txtCuenta, cboLlavesVariacion });
                    break;
                case ParametrizaciondeMovimientosdeIngreso:
                    panelParametrizacion.Visibility = Visibility.Visible;
                    clsValidateInput.Validate(txtPorcentaje, clsValidateInput.DecimalNumber);
                    validate.AsignarBorderNormal(new Control[] { txtPorcentaje, cboNaturaleza, txtCuentaParametrizacion, cboRecintoParametrizacion, cboFormaPago, cboMoneda });
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
                    clsUtilidades.OpenMessage(Guardar(), this);
                    Finalizar();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
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
                case Tarjeta:
                    controller.SaveUpdateTarjeta(tarjeta);
                    break;
                case Banco:
                    controller.SaveUpdateBanco(banco);
                    break;
                case FormadePago:
                    controller.SaveUpdateFormaPago(formapago);
                    break;
                case FuentedeFinanciamiento:
                    if (cboFuentesSIPPSI.SelectedIndex != -1)
                    {
                        ff.IdFuente_SIPPSI = byte.Parse(cboFuentesSIPPSI.SelectedValue.ToString());
                    }
                    controller.SaveUpdateFF(ff);
                    break;
                case Moneda:
                    controller.SaveUpdateMoneda(moneda);
                    break;

                case DocumentodeIdentidad:
                    controller.SaveUpdateIdentificacion(identificacion);
                    break;
                case EncabezadoyPiedeRecibo:
                    info.IdRecinto = int.Parse(cboRecinto.SelectedValue.ToString());
                    controller.SaveInfoRecibo(info);
                    break;
                case ParametrizaciondeVariaciones:
                    Configuracion c = (Configuracion)cboLlavesVariacion.SelectedItem;
                    controller.SaveUpdateParametrosVariacion(selectedCuenta, c);
                    break;
                case ParametrizaciondeMovimientosdeIngreso:
                    if (movimiento.IdMovimientoIngreso == 0)
                    {
                        movimiento.IdRecinto = int.Parse(cboRecintoParametrizacion.SelectedValue.ToString());
                        movimiento.IdFormaPago = int.Parse(cboFormaPago.SelectedValue.ToString());
                        movimiento.IdMoneda = int.Parse(cboMoneda.SelectedValue.ToString());
                    }
                    controller.SaveUpdateParametrizacion(movimiento, detalleMovimientoIngreso.ToList(), detalleDeleted);

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
                case Tarjeta:
                    flag = clsValidateInput.ValidateALL(new Control[] { txtTarjeta, txtSiglasTarjeta });
                    break;
                case Banco:
                    flag = clsValidateInput.ValidateALL(new Control[] { txtBanco, txtSiglasBanco });
                    break;
                case FormadePago:
                    flag = clsValidateInput.ValidateALL(new Control[] { txtFormaPago });
                    break;
                case FuentedeFinanciamiento:
                    flag = clsValidateInput.ValidateALL(new Control[] { txtNombreFF, txtSiglasFF });
                    break;
                case Moneda:
                    flag = clsValidateInput.ValidateALL(new Control[] { txtMoneda, txtSimbolo });
                    break;
                case DocumentodeIdentidad:
                    flag = clsValidateInput.ValidateALL(new Control[] { txtIdentificacion, txtMaxCaracteres });
                    if (flag)
                    {
                        if (identificacion.MaxCaracteres == 0)
                        {
                            txtMaxCaracteres.BorderBrush = clsUtilidades.BorderError();
                            flag = false;
                        }
                    }
                    break;
                case EncabezadoyPiedeRecibo:
                    flag = clsValidateInput.ValidateALL(new Control[] { txtEncabezado, txtPie, cboRecinto });
                    break;
                case ParametrizaciondeVariaciones:
                    flag = clsValidateInput.ValidateALL(new Control[] { txtCuenta, cboLlavesVariacion });
                    break;

                case ParametrizaciondeMovimientosdeIngreso:
                    flag = clsValidateInput.ValidateALL(new Control[] { cboRecintoParametrizacion, cboFormaPago, cboMoneda });
                    if (flag)
                    {
                        flag = ValidarPartidaDoble();
                    }
                    break;
                default:
                    flag = false;
                    break;
            }
            return flag;
        }

        private bool ValidarPartidaDoble()
        {
            //Hay partida doble si el campo del DEBE es igual al campo del Haber
            bool flag = txtDebe.Text.Equals(txtHaber.Text);

            if (!flag)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = "El movimiento no será generador de partida doble, para poder continuar debe asegurarse que los totales de débito y crédito sean los mismos", OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
            }
            return flag;
        }

        private void AddParametrizacion()
        {
            detalleMovimientoIngreso.Add(new DetalleMovimientoIngreso()
            {
                IdMovimientoIngreso = movimiento.IdMovimientoIngreso,
                canDelete = true,
                Naturaleza = cboNaturaleza.SelectedIndex == 1,
                FactorPorcentual = Math.Round(decimal.Parse(txtPorcentaje.Text.ToString()) / 100, 4),
                CuentaContable = selectedCuenta

            });

            clsValidateInput.CleanALL(new Control[] { txtCuentaParametrizacion, cboNaturaleza, txtPorcentaje });
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos())
            {
                rptRecibo boucher = new rptRecibo(info);
                boucher.ShowDialog();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            switch (gestion)
            {

                case EncabezadoyPiedeRecibo:
                    if (cboRecinto.Items.Count == 0)
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = "Actualmente no puede agregar un nuevo registro debido a que no se ha encontrado un nuevo recinto al cual crear el encabezado y pie de recibo, esta información es basada en sus permisos de usuario.", OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                        Close();
                    }
                    break;

                case ParametrizaciondeMovimientosdeIngreso:
                    CargaUpdateParametrizacion();

                    break;

                default:

                    break;
            }
        }

        private void BtnFindAccount_Click(object sender, RoutedEventArgs e)
        {
            CatalogoCuentas c = new CatalogoCuentas();
            c.ShowDialog();

            if (c.SelectedCuentaContable != null)
            {
                SeleccionarCuentaVariacion(c.SelectedCuentaContable);
            }
        }

        private void SeleccionarCuentaVariacion(CuentaContable c)
        {
            selectedCuenta = c;
            if (panelVariacion.Visibility == Visibility.Visible)
            {
                txtCuenta.Text = selectedCuenta.CuentaCodigo;
                txtCuenta.Focus();
            }
            else
            {
                txtCuentaParametrizacion.Text = selectedCuenta.CuentaCodigo;
                txtCuentaParametrizacion.Focus();
            }
        }

        private void BtnAddParametrizacion_Click(object sender, RoutedEventArgs e)
        {
            if (clsValidateInput.ValidateALL(new Control[] { txtCuentaParametrizacion, cboNaturaleza, txtPorcentaje }))
            {
                Dictionary<TextBox, int> c = new Dictionary<TextBox, int>();
                c.Add(txtPorcentaje, clsValidateInput.Porcentaje);

                if (clsValidateInput.ValidateNumerics(c))
                {
                    AddParametrizacion();
                }
            }
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            detalleMovimientoIngreso.Remove((DetalleMovimientoIngreso)tblDetalleMovimientos.CurrentItem);
        }

        private void CboRecintoParametrizacion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (movimiento.IdMovimientoIngreso == 0)
            {
                cboMoneda.ItemsSource = controller.FindAllMonedas();
            }
        }

        private void CboMoneda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (movimiento.IdMovimientoIngreso == 0)
            {
                vw_RecintosRH recinto = (vw_RecintosRH)cboRecintoParametrizacion.SelectedItem;
                Moneda moneda = (Moneda)cboMoneda.SelectedItem;

                cboFormaPago.ItemsSource = controller.FindFormaPagoMovimientoMoneda(recinto.IdRecinto, moneda.IdMoneda);
            }
        }
    }
}