using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using PruebaWPF.Views.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PruebaWPF.Views.Recibo
{
    /// <summary>
    /// Lógica de interacción para CrearRecibo.xaml
    /// </summary>
    public partial class CrearRecibo : Window
    {
        private ObservableCollection<DetOrdenPagoSon> items { get; set; }
        private ReciboViewModel controller;
        private OrdenPagoSon orden;
        private List<Moneda> monedas;
        private ObservableCollection<ReciboPagoSon> formaPago;
        private clsValidateInput validate;
        private ReciboSon recibo;
        List<MonedaMonto> aPagar;
        List<MonedaMonto> pendiente;
        private ObservableCollection<ArancelPrecio> aranceles;
        private fn_ConsultarInfoExterna_Result infoExterna;
        private TipoArancel tipoArancel;
        private int? IdMatricula = null;
        private int? IdPreMatricula = null;

        //TODO Cambiar la forma de los recibos, ya no serán series sino recintos

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
            CargarTiposDeposito(op.IdArea);
            isOrdenPago = true;
            Diseñar();
        }

        private void BloqueoOrdenPago()
        {
            txtRecibimos.IsEnabled = false;
            txtConcepto.IsEnabled = false;
            txtMonto.IsEnabled = false;
            cboTipoArancel.IsEnabled = false;
            txtArea.IsEnabled = false;
            txtPorCuenta.IsEnabled = false;
            txtIdentificador.IsEnabled = false;

            cboArancel.IsEnabled = false;
            cboMonedaDeuda.IsEnabled = false;
            cboTipoDeposito.IsEnabled = false;

            btnSelectArea.IsEnabled = false;
            //btnFindTipoDeposito.IsEnabled = false;
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
            clsUtilidades.Dialog_ModalDesign(this);
        }


        private void CargarAranceles(string idarea, int idtipodeposito)
        {
            if (idarea != null && (idtipodeposito != -1 && idtipodeposito != 0))
            {
                tipoArancel = (TipoArancel)cboTipoArancel.SelectedItem;

                if (tipoArancel != null)
                {
                    aranceles = new ObservableCollection<ArancelPrecio>(controller.ObtenerAranceles(idarea, idtipodeposito, tipoArancel.IdTipoArancel, txtIdentificador.Text, IdPreMatricula, IdMatricula).ToList());
                    cboArancel.ItemsSource = aranceles;
                }
            }
        }

        private void CargarTipoAranceles()
        {
            cboTipoArancel.ItemsSource = controller.ObtenerTipoArancel();
        }


        private void ActivarValidadorCampos()
        {
            clsValidateInput.Validate(txtMontoPago, clsValidateInput.DecimalNumber);
            clsValidateInput.Validate(txtMonto, clsValidateInput.DecimalNumber);
            clsValidateInput.Validate(txtNumeroCK, clsValidateInput.OnlyNumber);
            clsValidateInput.Validate(txtAutorizacion, clsValidateInput.OnlyNumber);
            clsValidateInput.Validate(txtTarjeta, clsValidateInput.OnlyNumber);

            validate.AsignarBorderNormal(new Control[] { txtArea, cboTipoDeposito, txtIdentificador, txtPorCuenta, txtRecibimos, cboFuenteFinanciamiento, cboTipoArancel, cboArancel, txtMonto, cboMonedaDeuda, cboFormaPago, txtMontoPago, cboMonedaPago, txtEmisor, txtBono, cboBanco, txtCuenta, txtNumeroCK, cboTarjeta, txtAutorizacion, txtTarjeta, cboTipo, txtTransaccion });

        }

        void Categories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (items.Any())
                {
                    CamposReciboEnable(false);
                }
                else
                {
                    CamposReciboEnable(true);
                }
            }
            //ContarRegistros();
        }

        private void CamposReciboEnable(bool v)
        {
            cboTipoArancel.IsEnabled = v;
            cboTipoDeposito.IsEnabled = v;
            txtIdentificador.IsEnabled = v;

            if (tipoArancel.IdDepositanteUnico == null)
            {
                btnSelectArea.IsEnabled = v;
            }
            else
            {
                btnSelectTipoDeposito.IsEnabled = v;
            }
        }

        private void Pagos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
            }
            if (tblDetallesPay.Items.Count == 0)
            {
                CalcularMontosDeuda();
            }
            else
            {
                CalcularMontosPago();
            }
            ContarRegistrosPay();

        }

        //private void ContarRegistros()
        //{
        //    lblCantidadRegistros.Text = items.Count.ToString();
        //}

        private void ContarRegistrosPay()
        {
            lblCantidadRegistrosPay.Text = formaPago.Count.ToString();
        }

        private void Load()
        {

            try
            {
                CargarCodigoRecibo();

                if (orden != null)
                {
                    if (orden.IdOrdenPago != 0)
                    {
                        items = new ObservableCollection<DetOrdenPagoSon>(controller.FindAllDetailsOrderPay(orden));
                    }
                }

                items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Categories_CollectionChanged);
                //ContarRegistros();
                tblDetalles.ItemsSource = items;

                CargarFuentesFinanciamiento();
                CargarMonedas();
                CargarFormasPago();
                CargarTipoAranceles();
                if (isOrdenPago)
                {
                    cboTipoDeposito.SelectedValue = orden.IdTipoDeposito;
                    AsignarDatos(InformacionResult(orden.IdTipoDeposito, orden.Identificador, true));
                    cboTipoArancel.SelectedValue = orden.DetOrdenPagoArancel.First().ArancelPrecio.ArancelArea.Arancel.IdTipoArancel; //Selecciono el primero TipoArancel debido a que todos los aranceles deberan en una orden de pago deben perternecer al mismo tipo de arancel
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
                Close();
            }

            tblDetallesPay.ItemsSource = formaPago;

        }

        private void CargarTiposDeposito(string IdArea)
        {
            if (cboTipoArancel.SelectedIndex != -1)
            {
                tipoArancel = (TipoArancel)cboTipoArancel.SelectedItem;
                cboTipoDeposito.ItemsSource = controller.ObtenerTipoCuenta(IdArea, tipoArancel);
            }
        }


        private void CargarBancos()
        {
            cboBanco.ItemsSource = controller.ObtenerBancos();
        }

        private void CargarTarjetas()
        {
            cboTarjeta.ItemsSource = controller.ObtenerTarjetas();
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

        private void CargarCodigoRecibo()
        {

            string[] codigo = controller.ObtenerCodigoRecibo();
            if (codigo != null)
            {
                recibo.IdRecibo = int.Parse(codigo[0]);
                recibo.Serie = codigo[1];
                recibo.IdDetAperturaCaja = int.Parse(codigo[2]);
                //recibo.IdPeriodoEspecifico = clsSessionHelper.periodoEspecifico.IdPeriodoEspecifico;
                recibo.IdInfoRecibo = int.Parse(codigo[3]);
                recibo.IdOrdenPago = orden.IdOrdenPago;
                txtCodRecibo.Text = codigo[0] + codigo[1];
            }

        }

        private void CalcularMontosDeuda()
        {
            lstSumatoria.Items.Clear();
            lstEquivalenciaTotal.Items.Clear();
            lstSumatoriaPay.Items.Clear();
            lstEquivalenciaTotalPay.Items.Clear();

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
            aPagar = new List<MonedaMonto>();
            pendiente = new List<MonedaMonto>();
            if (lstEquivalenciaTotalPay.Items.Count == 0)
            {
                lstPendiente.Items.Clear();
            }
            foreach (var item in SumETotal)
            {
                lstEquivalenciaTotal.Items.Add("Total en " + item.Moneda + " " + string.Format("{0:N}", item.Monto));
                aPagar.Add(new MonedaMonto() { Moneda = item.Moneda, Valor = item.Monto });

                if (lstEquivalenciaTotalPay.Items.Count == 0)
                {
                    lstPendiente.Items.Add("Saldo en " + item.Moneda + " " + string.Format("{0:N}", item.Monto));
                    pendiente.Add(new MonedaMonto() { Moneda = item.Moneda, Valor = Math.Round(item.Monto, 2) });
                }
            }

            InabilitarListas(false);
        }

        private void CalcularMontosPago()
        {
            var SumTotal = formaPago.GroupBy(a => new { a.Moneda }).Select(b => new { Moneda = b.Key.Moneda.Simbolo, IdMoneda = b.Key.Moneda.IdMoneda, Monto = b.Sum(c => c.Monto) }).OrderBy(o => o.IdMoneda).ToList();

            if (lstEquivalenciaTotalPay.Items.Count > 0 || SumTotal.Count > 0)
            {
                lstPendiente.Items.Clear();
            }
            lstSumatoriaPay.Items.Clear();
            lstEquivalenciaTotalPay.Items.Clear();


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
            pendiente = new List<MonedaMonto>();
            if (SumETotal.Count > 0)
            {
                foreach (var item in SumETotal)
                {
                    lstEquivalenciaTotalPay.Items.Add("Total en " + item.Moneda + " " + string.Format("{0:N}", item.Monto));

                    MonedaMonto a = aPagar.Where(w => w.Moneda == item.Moneda).First();

                    lstPendiente.Items.Add("Saldo en " + item.Moneda + " " + string.Format("{0:N}", (a.Valor - item.Monto)));
                    pendiente.Add(new MonedaMonto() { Moneda = item.Moneda, Valor = Math.Round(a.Valor - item.Monto, 2) });
                }
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
            if (clsValidateInput.ValidarSeleccion(cboFormaPago))
            {
                int formapago = int.Parse(cboFormaPago.SelectedValue.ToString());
                if (ValidarFormaPago(formapago))
                {
                    Dictionary<TextBox, int> c = new Dictionary<TextBox, int>();
                    c.Add(txtMontoPago, clsValidateInput.DecimalNumber);
                    AgregarValidacionAdicional(c, formapago);

                    if (ValidarNumericos(c))
                    {
                        AddFormaPago();
                    }
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
            aranceles.Add(((DetOrdenPagoSon)tblDetalles.CurrentItem).ArancelPrecio);
            items.Remove((DetOrdenPagoSon)tblDetalles.CurrentItem);

        }

        private void btnSelectArea_Click(object sender, RoutedEventArgs e)
        {
            if (clsValidateInput.ValidarSeleccion(cboTipoArancel))
            {
                tipoArancel = (TipoArancel)cboTipoArancel.SelectedItem;
                SeleccionarArea(tipoArancel.IdTipoArancel);
            }
        }

        private void SeleccionarArea(int idTipoArancel)
        {
            AreasRRHH areas = new AreasRRHH(idTipoArancel);
            areas.ShowDialog();

            if (areas.SelectedArea != null)
            {
                orden.Area = areas.SelectedArea.descripcion;
                orden.IdArea = areas.SelectedArea.codigo;
                ActualizarCampo(new TextBox[] { txtArea });
                txtArea.Focus();
                cboTipoArancel.Focus();
                CargarTiposDeposito(orden.IdArea);
                CargarAranceles(orden.IdArea, orden.IdTipoDeposito);
            }

        }

        private void ActualizarCampo(TextBox[] textbox)
        {
            for (int i = 0; i < textbox.Length; i++)
            {
                clsUtilidades.UpdateControl(textbox[i]);
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
            Object[] o = ObtenerObjetoAdicional(fp.IdFormaPago);

            formaPago.Add(new ReciboPagoSon
            {
                IdFormaPago = fp.IdFormaPago,
                FormaPago = fp,
                Moneda = m,
                IdMoneda = m.IdMoneda,
                Monto = Math.Round(Decimal.Parse(txtMontoPago.Text), 2),
                DetalleAdicional = o[0],
                InfoAdicional = o[1].ToString()
            });


            LimpiarCampos(new Control[] { cboFormaPago });
            LimpiarCampos(CamposAValidar(fp.IdFormaPago));

            VerCamposAdicionales(0); //Ocultará los campos adicionales al enviarle un id que no existe
            cboFormaPago.Focus();
        }


        private void AddArancel()
        {
            ArancelPrecio a = (ArancelPrecio)cboArancel.SelectedItem;
            Exoneracion exoneracion = controller.FindExoneracionArancel(orden.Identificador, orden.IdTipoDeposito, a.IdArancelPrecio);
            items.Add(new DetOrdenPagoSon()
            {
                PrecioVariable = Decimal.Parse(txtMonto.Text.ToString()),
                Concepto = txtConcepto.Text,
                Exoneracion = exoneracion,
                ArancelPrecio = a
            });

            aranceles.Remove((ArancelPrecio)cboArancel.SelectedItem);
            //aranceles.RemoveAt(cboArancel.SelectedIndex);
            //cboArancel.ItemsSource = null;
            //cboArancel.ItemsSource = aranceles;

            LimpiarCampos(new Control[] { cboArancel, txtMonto, cboMonedaDeuda, txtConcepto });
            txtMonto.IsEnabled = true;
        }

        #region Esta region contiene las validaciones y cargas en dependencia de la forma de pago seleccionada


        private void VerCamposAdicionales(int formapago)
        {
            switch (formapago)
            {
                case 2: //Cheque
                    CargarBancos();
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
                    OcultarVerAdicionales(Deposito, new Panel[] { Tarjeta, Cheque, EspacioVacio, Bono });
                    break;
                default:
                    OcultarVerAdicionales(EspacioVacio, new Panel[] { Tarjeta, Bono, Cheque, Deposito });
                    break;
                    //Efectivo
            }
        }

        private object[] ObtenerObjetoAdicional(int formapago)
        {
            Object[] o = new Object[2];
            switch (formapago)
            {
                case 2: //Cheque
                    ReciboPagoCheque rc = new ReciboPagoCheque()
                    {
                        Banco = (Banco)cboBanco.SelectedItem,
                        IdBanco = Byte.Parse(cboBanco.SelectedValue.ToString()),
                        NumeroCK = int.Parse(txtNumeroCK.Text.ToString()),
                        Cuenta = txtCuenta.Text
                    };

                    o[0] = rc;
                    o[1] = string.Format("{0}, Cuenta {1}, Cheque No.{2}", rc.Banco.Nombre, rc.Cuenta, rc.NumeroCK);
                    break;
                case 3: //Tarjeta
                    ReciboPagoTarjeta rt = new ReciboPagoTarjeta()
                    {
                        CiaTarjetaCredito = (CiaTarjetaCredito)cboTarjeta.SelectedItem,
                        IdTarjeta = Byte.Parse(cboTarjeta.SelectedValue.ToString()),
                        Tarjeta = txtTarjeta.Text,
                        Autorizacion = int.Parse(txtAutorizacion.Text.ToString())
                    };

                    o[0] = rt;
                    o[1] = string.Format("{0}, Tarjeta ****{1} Autorización {2}", rt.CiaTarjetaCredito.Nombre, rt.Tarjeta, rt.Autorizacion);
                    break;
                case 4: //Bono
                    ReciboPagoBono rb = new ReciboPagoBono()
                    {
                        Emisor = txtEmisor.Text,
                        Numero = txtBono.Text
                    };

                    o[0] = rb;
                    o[1] = string.Format("Emitipo por {0}, Bono No.{1}", rb.Emisor, rb.Numero);
                    break;
                case 5: //Deposito
                    ReciboPagoDeposito rd = new ReciboPagoDeposito()
                    {
                        Tipo = cboTipo.SelectedIndex == 0 ? false : true,
                        Transaccion = txtTransaccion.Text,
                        Observacion = txtObservación.Text
                    };

                    o[0] = rd;
                    o[1] = string.Format("{0}, Transacción No.{1}, Obs. {2}", rd.Tipo ? "Transferencia" : "Minuta", rd.Transaccion, rd.Observacion);
                    break;
                default:
                    o[0] = null;
                    o[1] = "";
                    break;
                    //Efectivo
            }

            return o;
        }

        private Control[] CamposAValidar(int formapago)
        {
            Control[] campos = new Control[5];

            campos[0] = txtMontoPago;
            campos[1] = cboMonedaPago;

            switch (formapago)
            {
                case 2: //Cheque
                    campos[2] = cboBanco;
                    campos[3] = txtCuenta;
                    campos[4] = txtNumeroCK;
                    break;
                case 3: //Tarjeta
                    campos[2] = cboTarjeta;
                    campos[3] = txtTarjeta;
                    campos[4] = txtAutorizacion;
                    break;
                case 4: //Bono
                    campos[2] = txtEmisor;
                    campos[3] = txtBono;
                    break;
                case 5: //Deposito
                    campos[2] = cboTipo;
                    campos[3] = txtTransaccion;
                    break;
                default:
                    break;
                    //Efectivo
            }

            return campos;

        }

        private void AgregarValidacionAdicional(Dictionary<TextBox, int> c, int formapago)
        {
            switch (formapago)
            {
                case 2: //Cheque
                    c.Add(txtNumeroCK, clsValidateInput.OnlyNumber);
                    break;
                case 3: //Tarjeta
                    c.Add(txtTarjeta, clsValidateInput.OnlyNumber);
                    c.Add(txtAutorizacion, clsValidateInput.OnlyNumber);
                    break;
                case 5: //Deposito
                    c.Add(txtTransaccion, clsValidateInput.OnlyNumber);
                    break;
                default:
                    break;
                    //Efectivo
            }

        }

        #endregion

        private void LimpiarCampos(Control[] control)
        {
            clsValidateInput.CleanALL(control);
        }

        private bool ValidarFormaPago(int formapago)
        {
            return clsValidateInput.ValidateALL(CamposAValidar(formapago));
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
            searchTipoDeposito search = new searchTipoDeposito(int.Parse(cboTipoDeposito.SelectedValue.ToString()), cboTipoDeposito.Text, tipoArancel.IdTipoArancel);
            search.Owner = this;
            search.ShowDialog();

            if (search.SelectedResult != null)
            {
                orden.IdTipoDeposito = int.Parse(cboTipoDeposito.SelectedValue.ToString());
                AsignarDatos(search.SelectedResult);

            }

        }

        private void AsignarDatos(fn_ConsultarInfoExterna_Result selectedResult)
        {
            infoExterna = selectedResult;
            orden.Identificador = selectedResult.Id;
            orden.PorCuenta = selectedResult.Nombre;

            recibo.Recibimos = selectedResult.Nombre;
            txtRecibimos.Text = recibo.Recibimos;

            ActualizarCampo(new TextBox[] { txtIdentificador, txtPorCuenta });

            //Si los campos estan rojos entonces les paso el foco para que cambien a azul
            txtIdentificador.Focus();
            txtPorCuenta.Focus();
            txtRecibimos.Focus();
        }

        private void cboTipoDeposito_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            LimpiarCargar();
        }

        private void SeleccionarPreOrMatricula(int idTipoArancel)
        {
            IdMatricula = controller.isMatricula(idTipoArancel) ? idTipoArancel : (int?)null;

            if (IdMatricula == null)
            {
                IdPreMatricula = controller.isPreMatricula(idTipoArancel) ? idTipoArancel : (int?)null;
            }
        }

        private void LimpiarCargar()
        {
            if (orden != null)
            {
                if (!isOrdenPago)
                {
                    orden.Identificador = "";
                    orden.PorCuenta = "";
                    if (cboTipoDeposito.SelectedValue != null)
                    {
                        orden.IdTipoDeposito = int.Parse(cboTipoDeposito.SelectedValue.ToString());

                        CargarAranceles(orden.IdArea, orden.IdTipoDeposito);

                    }
                    ActualizarCampo(new TextBox[] { txtIdentificador, txtPorCuenta, txtRecibimos });
                    items.Clear(); //Limpio los detalles a pagar porque se ha cambiado al tipo de depositante, y estos no poseen los mismos aranceles de pago
                }
                else
                {
                    isOrdenPago = true;
                }

            }
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
            if (cboTipoArancel.SelectedIndex != -1)
            {
                tipoArancel = (TipoArancel)cboTipoArancel.SelectedItem;
                return (new SearchTipoDepositoViewModel().ObtenerTipoDeposito(id, Identificador, BusquedaInterna, "", 1, tipoArancel.IdTipoArancel).FirstOrDefault());
            }
            else
            {
                return (new SearchTipoDepositoViewModel().ObtenerTipoDeposito(id, Identificador, BusquedaInterna, "", 1, null).FirstOrDefault());

            }
        }

        private void BuscarInformacion()
        {
            if (clsValidateInput.ValidateALL(new Control[] { cboTipoDeposito, txtIdentificador }))
            {
                try
                {
                    fn_ConsultarInfoExterna_Result data = InformacionResult(int.Parse(cboTipoDeposito.SelectedValue.ToString()), txtIdentificador.Text, false);
                    if (data != null)
                    {
                        if (IdPreMatricula != null || IdMatricula != null)
                        {
                            CargarAranceles(orden.IdArea, orden.IdTipoDeposito);
                        }

                        AsignarDatos(data);
                    }
                }
                catch (Exception ex)
                {
                    cboArancel.ItemsSource = null;
                    clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
                }
            }
        }

        private void btnNext_CLick(object sender, RoutedEventArgs e)
        {
            try
            {
                //Validación de controles vacíos
                if (clsValidateInput.ValidateALL(new Control[] { (btnSelectArea.IsEnabled ? txtArea : null), cboTipoDeposito, txtIdentificador, txtPorCuenta, txtRecibimos, cboFuenteFinanciamiento, cboTipoArancel }))
                {
                    //validación de tabla vacía
                    if (items.Count > 0)
                    {
                        //Validación de montos a pagar inferiores a cero
                        if (items.Where(w => w.Total < 0).Count() == 0)
                        {
                            if (tabControl.SelectedIndex == 1)
                            {
                                var a = pendiente.Where(w => w.Valor != 0).ToList();

                                if (a.Count() > 0)
                                {
                                    string info = "";
                                    foreach (var item in a)
                                    {
                                        info = item.Moneda + "\t" + item.Valor + "\t" + (item.Valor < 0 ? "(Debe restar esta cantidad a alguna forma de pago)" : "El usuario aún debe entregar esta cantidad a la caja") + "\n" + info;
                                    }
                                    clsUtilidades.OpenMessage(new Operacion()
                                    {
                                        Mensaje = "\nPara generar el recibo se requiere que los saldos de la columna PENDIENTE sean iguales a cero (0.00).\nPor favor corrija la siguiente información: \n\n" + info + "\nEn algunas ocasiones, habrán decimales de mas o de menos, esto se debe a que la conversión\nde moneda genera ciertos variaciones por la cantidad de decimales que se usan en las operaciones.",
                                        OperationType = clsReferencias.TYPE_MESSAGE_Advertencia
                                    });
                                }
                                else
                                {
                                    GenerarRecibo();
                                }
                            }
                            else
                            {
                                tabControl.SelectedIndex = 1;
                                btnBack.Visibility = Visibility.Visible;
                                btnNext.Content = "GENERAR RECIBO";
                                CalcularMontosDeuda();
                                CalcularMontosPago();
                            }
                        }
                        else
                        {
                            clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_Total_Menor_Cero, OperationType = clsReferencias.TYPE_MESSAGE_Error });
                        }
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_Cero_Registro_Table, OperationType = clsReferencias.TYPE_MESSAGE_Error });
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void GenerarRecibo()
        {
            orden.Identificador = infoExterna.IdInterno; //Le asigno el id interno de las tablas, porque el que tiene asignado previamente es el id visible a los usuarios
            recibo = controller.GenerarRecibo(tipoArancel, recibo, orden, items.ToList(), formaPago.ToList(), IdMatricula, IdPreMatricula);
            rptRecibo boucher = new rptRecibo(recibo, true);
            Finalizar();
            boucher.ShowDialog();
        }

        private void Finalizar()
        {
            if (orden != null)
            {
                OrdenPago.OrdenPago.isOpening = true;
            }
            frmMain.Refrescar();
            Close();
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
                txtMonto.IsEnabled = arancel.ArancelArea.Arancel.isPrecioVariable;

                //Las 3 lineas anteriores son necesarias para borrar el borde de error en caso de que el campo lo haya estado marcando
                cboMonedaDeuda.IsEnabled = false;
            }
        }

        private void tblDetalles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void cboFuenteFinanciamiento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recibo.IdFuenteFinanciamiento = byte.Parse(cboFuenteFinanciamiento.SelectedValue.ToString());
        }

        private void cboFormaPago_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VerCamposAdicionales(int.Parse(cboFormaPago.SelectedValue.ToString()));
        }

        private void OcultarVerAdicionales(Panel visible, Panel[] ocultos)
        {
            visible.Visibility = Visibility.Visible;
            for (int i = 0; i < ocultos.Length; i++)
            {
                ocultos[i].Visibility = Visibility.Collapsed;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CboTipoArancel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BotonBusquedaArea();
            LimpiarArea();
            LimpiarCargar();

        }

        private void BotonBusquedaArea()
        {
            if (cboTipoArancel.SelectedIndex != -1)
            {
                tipoArancel = (TipoArancel)cboTipoArancel.SelectedItem;
                if (tipoArancel.IdDepositanteUnico == null)
                {
                    btnSelectArea.IsEnabled = true;
                    cboTipoDeposito.ItemsSource = new List<TipoArancel>();
                }
                else
                {
                    btnSelectArea.IsEnabled = false;
                    CargarTiposDeposito("");
                }

                SeleccionarPreOrMatricula(tipoArancel.IdTipoArancel);
            }
        }

        private void LimpiarArea()
        {
            orden.IdArea = "";
            orden.Area = "";
            ActualizarCampo(new TextBox[] { txtArea });
        }
    }

    class MonedaMonto
    {
        public int IdMoneda { get; set; }
        public string Moneda { get; set; }
        public double Valor { get; set; }
    }
}


