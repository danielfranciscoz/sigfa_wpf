using Confortex.Clases;
using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Collections.Specialized;
using PruebaWPF.Views.Main;
using PruebaWPF.Helper;

namespace PruebaWPF.Views.Recibo
{
    /// <summary>
    /// Lógica de interacción para CrearRecibo.xaml
    /// </summary>
    public partial class CrearRecibo : Window
    {
        private ObservableCollection<DetOrdenPagoSon> items { get; set; }
        private ReciboViewModel controller;
        private Operacion o;
        private OrdenPagoSon orden;
        private List<Moneda> monedas;
        private ObservableCollection<ReciboPagoSon> formaPago;
        private clsValidateInput validate;
        private ReciboSon recibo;

        private bool isOrdenPago = false;
        public CrearRecibo()
        {

            InitializeComponent();
            this.orden = new OrdenPagoSon();
            DataContext = orden;
            Inicializar();
        }

        public CrearRecibo(Pantalla pantalla)
        {
            InitializeComponent();
            Inicializar();
            this.orden = new OrdenPagoSon();
            DataContext = orden;
            Diseñar();
        }

        public CrearRecibo(OrdenPagoSon op)
        {
            InitializeComponent();
            this.orden = op;
            Inicializar();
            DataContext = op;
            BloqueoOrdenPago();
            isOrdenPago = true;
            Diseñar();
        }

        private void BloqueoOrdenPago()
        {
            txtRecibimos.IsEnabled = false;
            txtConcepto.IsEnabled = false;
            txtMonto.IsEnabled = false;
            txtExonerado.IsEnabled = false;
            txtArea.IsEnabled = false;
            txtPorCuenta.IsEnabled = false;
            txtIdentificador.IsEnabled = false;

            cboArancel.IsEnabled = false;
            cboMonedaDeuda.IsEnabled = false;
            cboTipoDeposito.IsEnabled = false;

            btnSelectArea.IsEnabled = false;
            btnFindTipoDeposito.IsEnabled = false;
            btnSelectTipoDeposito.IsEnabled = false;
            btnAdd.IsEnabled = false;

            tblDetalles.Columns[0].Visibility = Visibility.Hidden; //Oculto la columna de eliminar registros de aranceles puesto que no será posible realizar cambios a las órdenes de pago.
        }

        private void Inicializar()
        {
            controller = new ReciboViewModel();
            validate = new clsValidateInput();
            recibo = new ReciboSon();

            items = new ObservableCollection<DetOrdenPagoSon>();

            formaPago = new ObservableCollection<ReciboPagoSon>();
            formaPago.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Pagos_CollectionChanged);

            monedas = controller.ObtenerMonedas();
            ActivarValidadorCampos();
        }

        private void Diseñar()
        {
            clsutilidades.Dialog_Perfomance(this);
        }
        private void CargarAranceles(string idarea, int idtipodeposito)
        {
            if (idarea != null && (idtipodeposito != -1 && idtipodeposito != 0))
            {
                cboArancel.ItemsSource = controller.ObtenerAranceles(idarea, idtipodeposito).ToList();
            }
        }

        private void ActivarValidadorCampos()
        {
            clsValidateInput.Validate(txtMontoPago, clsValidateInput.DecimalNumber);
            clsValidateInput.Validate(txtMonto, clsValidateInput.DecimalNumber);
            validate.AsignarBorderNormal(new Control[] { txtArea, cboTipoDeposito, txtIdentificador, txtPorCuenta, txtRecibimos, cboFuenteFinanciamiento, cboArancel, txtMonto, cboMonedaDeuda, cboFormaPago, txtMontoPago, cboMonedaPago });

        }

        void Categories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {

            }
            ContarRegistros();
        }

        private void Pagos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {

            }
            CalcularMontosPago();
            ContarRegistrosPay();

        }

        private void ContarRegistros()
        {
            lblCantidadRegistros.Text = items.Count.ToString();
        }

        private void ContarRegistrosPay()
        {
            lblCantidadRegistrosPay.Text = formaPago.Count.ToString();
        }

        private void Load()
        {
            if (CargarCodigoRecibo())
            {
                try
                {
                    if (orden != null)
                    {
                        if (orden.IdOrdenPago != 0)
                        {
                            items = new ObservableCollection<DetOrdenPagoSon>(controller.FindAllDetailsOrderPay(orden));
                        }
                    }

                    items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Categories_CollectionChanged);
                    ContarRegistros();
                    tblDetalles.ItemsSource = items;

                    CargarFuentesFinanciamiento();
                    CargarMonedas();
                    CargarFormasPago();
                    CargarTiposDeposito();
                    if (isOrdenPago)
                    {
                        cboTipoDeposito.SelectedValue = orden.IdTipoDeposito;
                        AsignarDatos(InformacionResult(orden.IdTipoDeposito, orden.Identificador, true));
                    }
                }
                catch (Exception ex)
                {
                    clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
                }

                tblDetallesPay.ItemsSource = formaPago;
            }
            else
            {
                o = new Operacion(clsReferencias.TYPE_MESSAGE_Error, "No hemos podido obtener un código de recibo válido, es probable que desde este ordenador no sea posible generar recibos, por favor revise la configuración de tesorería.");
                clsutilidades.OpenMessage(o);
                Close();
            }

        }

        private void CargarTiposDeposito()
        {
            cboTipoDeposito.ItemsSource = controller.ObtenerTipoCuenta();
        }

        private void CargarFormasPago()
        {
            cboFormaPago.ItemsSource = controller.ObtenerFormasPago();
        }

        private void CargarMonedas()
        {
            cboMonedaDeuda.ItemsSource = monedas;
            cboMonedaPago.ItemsSource = monedas;

        }

        private void CargarFuentesFinanciamiento()
        {
            cboFuenteFinanciamiento.ItemsSource = controller.ObtenerFuentesFinanciamiento();
            cboFuenteFinanciamiento.DisplayMemberPath = "Nombre";
            cboFuenteFinanciamiento.SelectedValuePath = "IdFuenteFinanciamiento";
        }

        private bool CargarCodigoRecibo()
        {
            try
            {
                string[] codigo = controller.ObtenerCodigoRecibo();
                if (codigo != null)
                {
                    recibo.IdRecibo = int.Parse(codigo[0]);
                    recibo.Serie = codigo[1];
                    recibo.IdCaja = int.Parse(codigo[2]);
                    recibo.IdPeriodoEspecifico = clsSessionHelper.periodoEspecifico.IdPeriodoEspecifico;
                    recibo.IdInfoRecibo = int.Parse(codigo[3]);
                    recibo.IdOrdenPago = orden.IdOrdenPago;
                    txtCodRecibo.Text = codigo[0] + codigo[1];

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void CalcularMontosDeuda()
        {
            lstSumatoria.Items.Clear();
            lstEquivalenciaTotal.Items.Clear();

            var SumTotal = items.GroupBy(a => new { a.ArancelPrecio.Moneda }).Select(b => new { Moneda = b.Key.Moneda.Simbolo, IdMoneda = b.Key.Moneda.IdMoneda, Monto = b.Sum(c => c.Total) }).OrderBy(o => o.IdMoneda).ToList();

            List<MonedaMonto> Totales = new List<MonedaMonto>();
            foreach (var item in SumTotal)
            {
                lstSumatoria.Items.Add("Detalles en " + item.Moneda + " " + string.Format("{0:N}", item.Monto));

                foreach (var i in monedas)
                {
                    if (i.IdMoneda == item.IdMoneda)
                    {
                        Totales.Add(new MonedaMonto { Valor = (Double)item.Monto, IdMoneda = item.IdMoneda, Moneda = item.Moneda });
                    }
                    else
                    {
                        Totales.Add(new MonedaMonto
                        {
                            Valor = controller.ConvertirDivisa(item.IdMoneda, i.IdMoneda, (Double)item.Monto),
                            IdMoneda = i.IdMoneda,
                            Moneda = i.Simbolo
                        });
                    }
                }
            }

            var SumETotal = Totales.GroupBy(a => new { a.IdMoneda, a.Moneda }).Select(b => new { Moneda = b.Key.Moneda, IdMoneda = b.Key.IdMoneda, Monto = b.Sum(c => c.Valor) }).OrderBy(o => o.IdMoneda).ToList();
            foreach (var item in SumETotal)
            {
                lstEquivalenciaTotal.Items.Add("Total en " + item.Moneda + " " + string.Format("{0:N}", item.Monto));
            }

            InabilitarListas(false);
        }

        private void CalcularMontosPago()
        {
            lstSumatoriaPay.Items.Clear();
            lstEquivalenciaTotalPay.Items.Clear();

            var SumTotal = formaPago.GroupBy(a => new { a.Moneda }).Select(b => new { Moneda = b.Key.Moneda.Simbolo, IdMoneda = b.Key.Moneda.IdMoneda, Monto = b.Sum(c => c.Monto) }).OrderBy(o => o.IdMoneda).ToList();

            List<MonedaMonto> Totales = new List<MonedaMonto>();
            foreach (var item in SumTotal)
            {
                lstSumatoriaPay.Items.Add("Pagos en " + item.Moneda + " " + string.Format("{0:N}", item.Monto));

                foreach (var i in monedas)
                {
                    if (i.IdMoneda == item.IdMoneda)
                    {
                        Totales.Add(new MonedaMonto { Valor = (Double)item.Monto, IdMoneda = item.IdMoneda, Moneda = item.Moneda });
                    }
                    else
                    {
                        Totales.Add(new MonedaMonto
                        {
                            Valor = controller.ConvertirDivisa(item.IdMoneda, i.IdMoneda, (Double)item.Monto),
                            IdMoneda = i.IdMoneda,
                            Moneda = i.Simbolo
                        });
                    }
                }
            }

            var SumETotal = Totales.GroupBy(a => new { a.IdMoneda, a.Moneda }).Select(b => new { Moneda = b.Key.Moneda, IdMoneda = b.Key.IdMoneda, Monto = b.Sum(c => c.Valor) }).OrderBy(o => o.IdMoneda).ToList();
            foreach (var item in SumETotal)
            {
                lstEquivalenciaTotalPay.Items.Add("Total en " + item.Moneda + " " + string.Format("{0:N}", item.Monto));
            }

            InabilitarListas(true);
        }

        private void InabilitarListas(Boolean isPago)
        {
            if (isPago)
            {
                if (lstSumatoriaPay.IsHitTestVisible)
                {
                    lstSumatoriaPay.IsHitTestVisible = false;
                    lstEquivalenciaTotalPay.IsHitTestVisible = false;
                }
            }
            else
            if (lstSumatoria.IsHitTestVisible)
            {
                lstSumatoria.IsHitTestVisible = false;
                lstEquivalenciaTotal.IsHitTestVisible = false;
            }

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (clsValidateInput.ValidateALL(new Control[] { cboArancel, txtMonto, cboMonedaDeuda }))
            {
                Dictionary<TextBox, int> c = new Dictionary<TextBox, int>();
                c.Add(txtMonto, clsValidateInput.DecimalNumber);

                if (ValidarNumericos(c))
                {
                    AddArancel();
                }

            }

        }

        private void btnAddPay_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarFormaPago())
            {
                Dictionary<TextBox, int> c = new Dictionary<TextBox, int>();
                c.Add(txtMontoPago, clsValidateInput.DecimalNumber);

                if (ValidarNumericos(c))
                {
                    AddFormaPago();
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 0;
            btnNext.Content = "SIGUIENTE";
            btnBack.Visibility = Visibility.Hidden;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            items.Remove((DetOrdenPagoSon)tblDetalles.CurrentItem);


        }

        private void btnSelectArea_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarArea();

        }

        private void SeleccionarArea()
        {
            AreasRRHH areas = new AreasRRHH();
            areas.ShowDialog();

            if (areas.SelectedArea != null)
            {
                orden.Area = areas.SelectedArea.descripcion;
                orden.IdArea = areas.SelectedArea.codigo;
                ActualizarCampo(new TextBox[] { txtArea });
                txtArea.Focus();
                cboTipoDeposito.Focus();
                CargarAranceles(orden.IdArea, orden.IdTipoDeposito);
            }

        }

        private void ActualizarCampo(TextBox[] textbox)
        {
            for (int i = 0; i < textbox.Length; i++)
            {
                clsutilidades.UpdateControl(textbox[i]);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void AddFormaPago()
        {
            FormaPago fp = (FormaPago)cboFormaPago.SelectedItem;
            Moneda m = (Moneda)cboMonedaPago.SelectedItem;
            formaPago.Add(new ReciboPagoSon { IdFormaPago = fp.IdFormaPago, FormaPago = fp, Moneda = m, IdMoneda = m.IdMoneda, Monto = Decimal.Parse(txtMontoPago.Text) });
            LimpiarCampos(new Control[] { cboFormaPago, txtMontoPago, cboMonedaPago });
        }


        private void AddArancel()
        {
            items.Add(new DetOrdenPagoSon()
            {
                PrecioVariable = Decimal.Parse(txtMonto.Text.ToString()),
                Concepto = txtConcepto.Text,
                Descuento = Decimal.Parse(txtExonerado.Text.Equals("") ? "0.00" : txtExonerado.Text),
                ArancelPrecio = (ArancelPrecio)cboArancel.SelectedItem
            });
            LimpiarCampos(new Control[] { cboArancel, txtMonto, cboMonedaDeuda, txtConcepto, txtExonerado });
            txtMonto.IsEnabled = true;
        }

        private void LimpiarCampos(Control[] control)
        {
            clsValidateInput.CleanALL(control);
        }

        private bool ValidarFormaPago()
        {
            return clsValidateInput.ValidateALL(new Control[] { cboFormaPago, txtMontoPago, cboMonedaPago });
        }

        private bool ValidarNumericos(Dictionary<TextBox, int> campos)
        {

            return clsValidateInput.ValidateNumerics(campos);
        }

        private void btnSelectTipoDeposito_Click(object sender, RoutedEventArgs e)
        {
            if (clsValidateInput.ValidarSeleccion(cboTipoDeposito))
            {
                SeleccionarTipoDeposito();
            }
        }

        private void SeleccionarTipoDeposito()
        {
            searchTipoDeposito search = new searchTipoDeposito(int.Parse(cboTipoDeposito.SelectedValue.ToString()), cboTipoDeposito.Text);
            search.ShowDialog();

            if (search.SelectedResult != null)
            {
                orden.IdTipoDeposito = int.Parse(cboTipoDeposito.SelectedValue.ToString());
                AsignarDatos(search.SelectedResult);

            }

        }

        private void AsignarDatos(fn_ConsultarInfoExterna_Result selectedResult)
        {
            orden.Identificador = selectedResult.Id;
            orden.PorCuenta = selectedResult.Nombre;

            if (string.IsNullOrEmpty(orden.Recibimos))
            {
                orden.Recibimos = selectedResult.Nombre;
            }

            ActualizarCampo(new TextBox[] { txtIdentificador, txtPorCuenta, txtRecibimos });

            txtPorCuenta.Focus();
            txtRecibimos.Focus();
        }

        private void cboTipoDeposito_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LimpiarCargar();
        }

        private void LimpiarCargar()
        {
            if (orden != null)
            {
                if (!isOrdenPago)
                {
                    orden.Identificador = "";
                    orden.PorCuenta = "";
                    orden.IdTipoDeposito = int.Parse(cboTipoDeposito.SelectedValue.ToString());
                    ActualizarCampo(new TextBox[] { txtIdentificador, txtPorCuenta, txtRecibimos });
                    CargarAranceles(orden.IdArea, orden.IdTipoDeposito);
                    items.Clear(); //Limpio los detalles a pagar porque se ha cambiado al tipo de depositante, y estos no poseen los mismos aranceles de pago
                }
                else
                {
                    isOrdenPago = true;
                }

            }
        }

        private void btnFindTipoDeposito_Click(object sender, RoutedEventArgs e)
        {
            BuscarInformacion();
        }

        private void txtIdentificador_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                BuscarInformacion();
            }
        }

        private fn_ConsultarInfoExterna_Result InformacionResult(int id, string Identificador, bool BusquedaInterna)
        {
            return (new SearchTipoDepositoViewModel().ObtenerTipoDeposito(id, Identificador, BusquedaInterna, "", 1).FirstOrDefault());
        }

        private void BuscarInformacion()
        {
            if (clsValidateInput.ValidateALL(new Control[] { cboTipoDeposito, txtIdentificador }))
            {
                try
                {
                    fn_ConsultarInfoExterna_Result data = InformacionResult(int.Parse(cboTipoDeposito.SelectedValue.ToString()), txtIdentificador.Text, false);
                    AsignarDatos(data);
                }
                catch (Exception ex)
                {
                    clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
                }
            }
        }

        private void btnNext_CLick(object sender, RoutedEventArgs e)
        {
            try
            {
                //Validación de controles vacíos
                if (clsValidateInput.ValidateALL(new Control[] { txtArea, cboTipoDeposito, txtIdentificador, txtPorCuenta, txtRecibimos, cboFuenteFinanciamiento }))
                {
                    //validación de tabla vacía
                    if (items.Count > 0)
                    {
                        //Validación de montos a pagar inferiores a cero
                        if (items.Where(w => w.Total < 0).Count() == 0)
                        {
                            if (tabControl.SelectedIndex == 1)
                            {
                                GenerarRecibo();
                            }
                            else
                            {
                                tabControl.SelectedIndex = 1;
                                btnBack.Visibility = Visibility.Visible;
                                btnNext.Content = "GENERAR RECIBO";
                                CalcularMontosDeuda();
                            }
                        }
                        else
                        {
                            clsutilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_Total_Menor_Cero, OperationType = clsReferencias.TYPE_MESSAGE_Error });
                        }
                    }
                    else
                    {
                        clsutilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_Cero_Registro_Table, OperationType = clsReferencias.TYPE_MESSAGE_Error });
                    }
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void GenerarRecibo()
        {
            recibo = controller.GenerarRecibo(recibo, orden, items.ToList(), formaPago.ToList());
            rptRecibo boucher = new rptRecibo(recibo);
            Close();
            boucher.ShowDialog();
        }

        private void btnDeletePay_Click(object sender, RoutedEventArgs e)
        {
            formaPago.Remove((ReciboPagoSon)tblDetallesPay.CurrentItem);
        }

        private void cboArancel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarDatosArancel();
        }

        private void CargarDatosArancel()
        {
            var arancel = (ArancelPrecio)cboArancel.SelectedItem;
            if (arancel is null)
            {
                txtMonto.Text = "";
                cboMonedaDeuda.SelectedValue = null;
                cboMonedaDeuda.IsEnabled = true;
            }
            else
            {
                txtMonto.Text = arancel.Precio.ToString();
                cboMonedaDeuda.SelectedValue = arancel.IdMoneda;

                txtMonto.IsEnabled = true;
                txtMonto.Focus();
                txtMonto.IsEnabled = arancel.Arancel.isPrecioVariable;

                //Las 3 lineas anteriores son necesarias para borrar el borde de error en caso de que el campo lo haya estado marcando
                cboMonedaDeuda.IsEnabled = arancel.Arancel.isPrecioVariable;
            }
        }

        private void tblDetalles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void cboFuenteFinanciamiento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recibo.IdFuenteFinanciamiento = byte.Parse(cboFuenteFinanciamiento.SelectedValue.ToString());
        }
    }


    class MonedaMonto
    {
        public int IdMoneda { get; set; }
        public string Moneda { get; set; }
        public double Valor { get; set; }
    }
}


