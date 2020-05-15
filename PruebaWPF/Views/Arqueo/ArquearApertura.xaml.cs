using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PruebaWPF.Views.Arqueo
{
    /// <summary>
    /// Lógica de interacción para ArquearApertura.xaml
    /// </summary>
    public partial class ArquearApertura : Window
    {
        private ArqueoViewModel controller;
        private Operacion operacion;
        private Pantalla pantalla;
        private DetAperturaCaja apertura;
        private Model.Arqueo arqueo;
        private BindingList<ArqueoEfectivoSon> efectivo;
        //private List<FormaPago> formasPagoDocumentos;
        //private ObservableCollection<ArqueoNoEfectivoSon> documentos;
        //private BindingList<DocumentosEfectivo> documentosNew;
        private System.Windows.Forms.BindingSource ArqueoEfectivoBindingSource = new System.Windows.Forms.BindingSource();
        private System.Windows.Forms.BindingSource DocumentosEfectivoBindingSource = new System.Windows.Forms.BindingSource();
        private List<Moneda> monedas;
        private ObservableCollection<Recibo1> recibos;
        private MaterialDesignExtensions.Controllers.StepperController tabmaterial;
        private DateTime? fecha = null;
        private clsValidateInput validate = new clsValidateInput();
        private List<fn_TotalesArqueo_Result> diferenciasTotales;
        //private int conteoDocEfectivo = 0;
        public static bool cambios = false;

        public ArquearApertura()
        {
            InitializeComponent();
        }

        public ArquearApertura(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            controller = new ArqueoViewModel(pantalla);

            InitializeComponent();
            Diseñar();
            Inicializar();
        }

        private void Inicializar()
        {
            operacion = new Operacion();
            tabmaterial = tabParent.Controller.ActiveStepViewModel.Controller;
            diferenciasTotales = new List<fn_TotalesArqueo_Result>();
            //CamposNormales();
        }

        //private void CamposNormales()
        //{
        //    clsValidateInput.Validate(txtMonto, clsValidateInput.DecimalNumber);
        //    validate.AsignarBorderNormal(new Control[] { cboRecibo, cboDocumento, txtNoDocumento, cboMoneda, txtMonto });
        //}

        //private bool ValidarCampos()
        //{
        //    return clsValidateInput.ValidateALL(new Control[] { cboRecibo, cboDocumento, txtNoDocumento, cboMoneda, txtMonto });
        //}
        private bool ValidarNumericos(Dictionary<TextBox, int> campos)
        {

            return clsValidateInput.ValidateNumerics(campos);
        }

        private void Diseñar()
        {
            clsUtilidades.Dialog_ModalDesign(this);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Object[] datos = controller.DetectarApertura();

                apertura = (DetAperturaCaja)datos[0];

                if (datos.Length > 1)
                {
                    panelWarningRecibo.Visibility = Visibility.Visible;
                    lblWarning.Text = "El proceso de arqueo se encuentra iniciado, continúe con las siguientes operaciones para finalizarlo.";
                    txtPleaseVerify.Visibility = Visibility.Collapsed;
                    btnArqueo.Content = "CONTINUAR ARQUEO";
                    arqueo = (Model.Arqueo)datos[1];
                }
                else
                {
                    arqueo = new Model.Arqueo();
                    arqueo.IdArqueoDetApertura = apertura.IdDetAperturaCaja;
                }

                datosIniciales.DataContext = apertura;
                lblRecuento.DataContext = apertura;

                //int[] data = controller.FindTotalPagos(apertura.IdDetAperturaCaja);
                //lblRecuentoPagos.DataContext = data[0];
                //panelConfirmacionPagos.DataContext = data[1];

            }
            catch (Exception ex)
            {
                panelInfo.Visibility = Visibility.Collapsed;

                lblErrorMesaje.Text = new clsException(ex).ErrorMessage();
                panelMensaje.Visibility = Visibility.Visible;
            }

        }

        private void codrecibo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                AddRecibo();
            }
        }

        private void AddRecibo()
        {
            string codigo = txtcodrecibo.Text;
            try
            {
                Recibo1 agregado = controller.ContabilizarRecibo(codigo, apertura);
                recibos.Insert(0, agregado);

                AsignarFecha(agregado);

                if (panelErrorRecibo.Visibility == Visibility.Visible)
                {
                    panelErrorRecibo.Visibility = Visibility.Hidden;
                }

                txtcodrecibo.Text = "";

                lstRecibos.SelectedItem = agregado;
            }
            catch (Exception ex)
            {
                lblErrorRecibo.Text = new clsException(ex).ErrorMessage();
                panelErrorRecibo.Visibility = Visibility.Visible; ;
            }

        }

        private void AsignarFecha(Recibo1 agregado)
        {
            //fecha para el tipo de cambio
            if (fecha == null)
            {
                fecha = agregado.Fecha;
                var tc = controller.FindTipoCambios(apertura.AperturaCaja).Select(s => string.Format(" {0} -> C$ {1}", s.SimboloMoneda, s.Valor)); ;


                txtTC.Text = string.Format("Fecha de Apertura= {0}, TC={1}", fecha.Value.ToString("dd/MM/yyyy"), string.Join(",", tc).ToString());
            }

        }

        private void btnArqueo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(arqueo.UsuarioArqueador))
                {
                    controller.Guardar(arqueo);

                }
                CargarRecibosContabilizados();
                tabmaterial.Continue();
                panelMensaje.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                lblErrorMesaje.Text = new clsException(ex).ErrorMessage();
                panelMensaje.Visibility = Visibility.Visible;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            tabmaterial.Back();
        }

        private void btnConteoRecibos_Click(object sender, RoutedEventArgs e)
        {
            if (efectivo == null)
            {
                CargarEfectivoContabilizado();
            }

            tabmaterial.Continue();
        }

        private void CargarRecibosContabilizados()
        {
            if (recibos == null)
            {
                recibos = new ObservableCollection<Recibo1>(controller.FindRecibosContabilizados(arqueo));
                recibos.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Recibos_CollectionChanged);
                lstRecibos.ItemsSource = recibos;
                VerificarConteoFinalizado();
                if (recibos.Any())
                {
                    AsignarFecha(recibos.FirstOrDefault());
                }
            }

            ContarConfirmados();


        }

        private void ContarConfirmados()
        {
            //panelConfirmacionDocumentos.Text = 
        }

        private void Recibos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                VerificarConteoFinalizado();
            }
        }

        private void Efectivo_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                tblEfectivo.Items.Refresh();
            }
        }



        private void VerificarConteoFinalizado()
        {

            if (recibos.Count == apertura.Recibo1.Count)
            {
                txtcodrecibo.IsEnabled = false;
                btnConteoRecibos.IsEnabled = true;
            }

        }

        private void btnConteoEfectivo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                controller.GuardarEfectivo(efectivo.ToList(), arqueo);
                efectivo = null;
                CargarEfectivoContabilizado();
                if (monedas == null)
                {
                    CargarMonedas();
                }
                TotalSaldoArqueo();
                //CargarRecibosArqueados();
                //if (documentos == null) //Lo que esta dentro de este if solo me interesa que se cargue una vez porque no cambiará
                //{
                //    CargarInfoIdentificador();
                //    CargarDocumentosPago();
                //    CargarDocumentosArqueados();
                //}

                //if (documentosNew == null)
                //{
                //    CargarDocumentosEfectivo();
                //}
                tabmaterial.Continue();
                panelErrorEfectivo.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                lblErrorEfectivo.Text = new clsException(ex).ErrorMessage();
                panelErrorEfectivo.Visibility = Visibility.Visible;
            }
        }

        //private void CargarDocumentosPago()
        //{

        //    cboDocumento.ItemsSource = formasPagoDocumentos;
        //}

        private void SaldosDocumentosEfectivo()
        {
            tblConsolidadoDocumentos.ItemsSource = controller.FindConsolidadoDocumentos(apertura.IdDetAperturaCaja);
        }
        //private void CargarInfoIdentificador()
        //{
        //    formasPagoDocumentos = controller.FindFormasPagoDocumentos(apertura.IdDetAperturaCaja);
        //    string informacion = null;
        //    foreach (FormaPago f in formasPagoDocumentos)
        //    {
        //        if (!string.IsNullOrEmpty(f.Identificador))
        //        {
        //            if (informacion == null)
        //            {
        //                informacion = string.Format("{0}: {1},", f.FormaPago1, f.Identificador);
        //            }
        //            else
        //            {
        //                informacion = informacion + string.Format(" {0}: {1},", f.FormaPago1, f.Identificador);
        //            }
        //        }
        //    }
        //    if (informacion != null)
        //    {
        //        if (informacion.Length > 0)
        //        {
        //            txtInfoIdentificador.Text = informacion.Substring(0, informacion.Length - 1);
        //        }
        //        else
        //        {
        //            txtInfoIdentificador.Text = "No hay información disponible";
        //        }
        //    }
        //}

        private void CargarEfectivoContabilizado()
        {
            if (efectivo == null)
            {
                efectivo = new BindingList<ArqueoEfectivoSon>(controller.FindConteoEfectivo(apertura.IdDetAperturaCaja, true));
                ArqueoEfectivoBindingSource.DataSource = efectivo;

                tblEfectivo.ItemsSource = ArqueoEfectivoBindingSource;
                efectivo.ListChanged += new ListChangedEventHandler(Ef_CollectionChanged);
                CalcularTotales();
            }
        }

        //private void CargarRecibosArqueados()
        //{
        //    cboRecibo.ItemsSource = recibos.Select(s => new ArqueoNoEfectivoSon { IdRecibo = s.IdRecibo, Serie = s.Serie }).Distinct();
        //    panelConteoDocumentos.Text = "" + controller.FindTotalDocumentos(apertura.IdDetAperturaCaja);
        //}

        private void CargarMonedas()
        {
            monedas = controller.FindMonedas();
            //cboMoneda.ItemsSource = monedas;
        }

        //private void CargarDocumentosArqueados()
        //{
        //    documentos = new ObservableCollection<ArqueoNoEfectivoSon>(controller.FindDocumentosArqueados(apertura.IdDetAperturaCaja));
        //    documentos.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Documentos_CollectionChanged);
        //    tblDocumentosEfectivo.ItemsSource = documentos;
        //    if (documentos != null)
        //    {
        //        CalcularTotalesDocumento();
        //    }
        //}

        //private void CargarDocumentosEfectivo()
        //{
        //    if (documentosNew == null)
        //    {
        //        documentosNew = new BindingList<DocumentosEfectivo>(controller.FindDocumentosEfectivo(apertura));
        //        DocumentosEfectivoBindingSource.DataSource = documentosNew;

        //        tblDocumentosEfectivo.ItemsSource = DocumentosEfectivoBindingSource;
        //        documentosNew.ListChanged += new ListChangedEventHandler(Doc_CollectionChanged);

        //        CalcularDocumentosNoConfirmados();
        //    }
        //}

        //private void CalcularDocumentosNoConfirmados()
        //{
        //    conteoDocEfectivo = documentosNew.Count(w => w.NoDocumento != null && w.NoDocumento != "");
        //    panelConteoDocumentos.Text = "" + conteoDocEfectivo;
        //}


        private void Ef_CollectionChanged(object sender, ListChangedEventArgs e)
        {
            tblEfectivo.CommitEdit();
            CalcularTotales();
            tblEfectivo.Items.Refresh();
        }

        //private void Doc_CollectionChanged(object sender, ListChangedEventArgs e)
        //{

        //    tblEfectivo.CommitEdit();
        //    CalcularTotalesDocumento();
        //    tblEfectivo.Items.Refresh();
        //}

        //private void CalcularTotalesDocumento()
        //{
        //    var total = documentos.GroupBy(g => new { g.FormaPago.FormaPago1, g.Moneda.Moneda1, g.Moneda.Simbolo }).Select(s1 => new { s1.Key.FormaPago1, TotalMoneda = s1.Key.Moneda1, TotalSimbolo = s1.Key.Simbolo, TotalEfectivo = s1.Sum(s => s.MontoFisico) }).OrderBy(o => o.FormaPago1).ThenBy(o => o.TotalSimbolo);

        //    lstDocumentos.ItemsSource = total;
        //}

        private void CalcularTotales()
        {
            var total = efectivo.GroupBy(g => new { g.Moneda.Moneda1, g.Moneda.Simbolo }).Select(s1 => new { TotalMoneda = s1.Key.Moneda1, TotalSimbolo = s1.Key.Simbolo, TotalEfectivo = s1.Sum(s => s.Total) });

            lstTotales.ItemsSource = total;
        }

        //private void btnConteoNoEfectivo_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        controller.GuardarNoEfectivo(documentos.ToList(), arqueo);
        //        documentos = null;
        //        CargarDocumentosArqueados();
        //        TotalSaldoArqueo();
        //        tabmaterial.Continue();
        //        panelErrorDocumentos.Visibility = Visibility.Hidden;
        //    }
        //    catch (Exception ex)
        //    {
        //        lblErrorDocumentos.Text = new clsException(ex).ErrorMessage();
        //        panelErrorDocumentos.Visibility = Visibility.Visible;
        //    }
        //}

        private void CargarResumenTotales()
        {
            throw new NotImplementedException();
        }

        //private void BtnDeletePay_Click(object sender, RoutedEventArgs e)
        //{
        //    documentos.Remove((ArqueoNoEfectivoSon)tblDocumentosEfectivo.CurrentItem);
        //    CalcularTotalesDocumento();
        //}

        //private void btnAdd_Click(object sender, RoutedEventArgs e)
        //{
        //    ValidaAgregaDocumento();
        //}

        //private void ValidaAgregaDocumento()
        //{
        //    if (ValidarCampos())
        //    {
        //        Dictionary<TextBox, int> c = new Dictionary<TextBox, int>();
        //        c.Add(txtMonto, clsValidateInput.DecimalNumber);

        //        if (ValidarNumericos(c))
        //        {
        //            AgregarDocumento();
        //        }
        //    }
        //}

        //private bool ValidarRepetido(ArqueoNoEfectivoSon noefectivo)
        //{
        //    return documentos.Any(a => a.Recibo == noefectivo.Recibo && a.IdFormaPago == noefectivo.IdFormaPago && a.NoDocumento == noefectivo.NoDocumento && a.MonedaFisica == noefectivo.MonedaFisica);
        //}

        //private void AgregarDocumento()
        //{
        //    ArqueoNoEfectivoSon noefectivo = new ArqueoNoEfectivoSon();
        //    var recibo = (ArqueoNoEfectivoSon)cboRecibo.SelectedItem;

        //    noefectivo.IdRecibo = recibo.IdRecibo;
        //    noefectivo.Serie = recibo.Serie;

        //    FormaPago formapago = (FormaPago)cboDocumento.SelectedItem;
        //    noefectivo.IdFormaPago = formapago.IdFormaPago;
        //    noefectivo.FormaPago = formapago;

        //    noefectivo.NoDocumento = txtNoDocumento.Text;

        //    Moneda moneda = (Moneda)cboMoneda.SelectedItem;
        //    noefectivo.MonedaFisica = moneda.IdMoneda;
        //    noefectivo.Moneda = moneda;

        //    noefectivo.MontoFisico = decimal.Parse(txtMonto.Text);

        //    if (!ValidarRepetido(noefectivo))
        //    {
        //        noefectivo.IdReciboPago = controller.FindReciboPago(noefectivo);

        //        documentos.Add(noefectivo);

        //        CalcularTotalesDocumento();
        //        LimpiarCamposDocumento();
        //        cboRecibo.Focus();
        //    }
        //    else
        //    {
        //        lblErrorDocumentos.Text = "El documento ya ha sido agregado";
        //        panelErrorDocumentos.Visibility = Visibility.Visible;
        //    }
        //}

        //private void LimpiarCamposDocumento()
        //{
        //    clsValidateInput.CleanALL(new Control[] { cboRecibo, cboDocumento, txtNoDocumento, cboMoneda, txtMonto });

        //    if (panelErrorDocumentos.Visibility == Visibility.Visible)
        //    {
        //        panelErrorDocumentos.Visibility = Visibility.Hidden;
        //    }
        //}

        //private void TxtMonto_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Return)
        //    {
        //        ValidaAgregaDocumento();
        //    }
        //}

        private void TotalSaldoArqueo()
        {
            var saldos = controller.SaldoTotalArqueo(apertura);
            diferenciasTotales = new List<fn_TotalesArqueo_Result>(); //Reseteo la lista cada vez que se recalculen los totales
            lstEfectivoRecibido.Items.Clear();
            lstEfectivoRecibidoEquivalente.Items.Clear();
            lstEfectivoArqueo.Items.Clear();
            lstEfectivoArqueoEquivalente.Items.Clear();
            lstDiferencias.Items.Clear();
            lstDiferenciasEquivalente.Items.Clear();
            SaldosEfectivo(saldos[0]);

            //lstPendienteDocumento.Items.Clear();
            //lstDocumentoMatch.Items.Clear();
            //DocumentosNoEnlazados(saldos[1]);
            //diferenciasTotales = diferenciasTotales.Union(saldos[1]).ToList();

            SaldosDocumentosEfectivo();
        }

        private void DocumentosNoEnlazados(List<fn_TotalesArqueo_Result> list)
        {

            //List<ArqueoNoEfectivoSon> documentosMatch = documentos.Where(w => w.IdReciboPago == null).Union(controller.FindDocumentosNoEnlazados(apertura.IdAperturaCaja)).ToList();
            //tblDocumentosMatch.ItemsSource = documentosMatch;

            //foreach (var item in list)
            //{
            //    if (item.Diferencia != 0)
            //    {
            //        lstPendienteDocumento.Items.Add(string.Format("{0}, {1} {2} {3}", item.Diferencia > 0 ? "Sobrante" : "Faltante", item.FormaPago == null ? item.FormaPagoArqueo : item.FormaPago, item.MonedaDiferencia, item.Diferencia));
            //    }
            //}
        }

        private void SaldosEfectivo(List<fn_TotalesArqueo_Result> list)
        {
            List<MonedaMonto> recibidoEquivalente;
            List<MonedaMonto> arqueoEquivalente;
            List<MonedaMonto> TotalesRecibido = new List<MonedaMonto>();
            List<MonedaMonto> TotalesArqueo = new List<MonedaMonto>();
            List<MonedaMonto> TotalesDiferencia = new List<MonedaMonto>();

            foreach (var item in list)
            {

                if (item.IdApertura != null)
                {
                    lstEfectivoRecibido.Items.Add(item.Simbolo + " " + string.Format("{0:N}", item.Monto));

                    foreach (var i in monedas)
                    {
                        if (i.IdMoneda == item.IdMoneda.Value)
                        {
                            TotalesRecibido.Add(new MonedaMonto { Valor = (Double)item.Monto.Value, IdMoneda = item.IdMoneda.Value, Moneda = item.Simbolo });
                        }
                        else
                        {
                            TotalesRecibido.Add(new MonedaMonto
                            {
                                Valor = new ReciboViewModel().ConvertirDivisa(item.IdMoneda.Value, i.IdMoneda, item.Monto, fecha),
                                IdMoneda = i.IdMoneda,
                                Moneda = i.Simbolo
                            });
                        }
                    }

                }
                if (item.IdArqueo != null)
                {
                    lstEfectivoArqueo.Items.Add(item.SimboloArqueo + " " + string.Format("{0:N}", item.MontoArqueo));
                    foreach (var i in monedas)
                    {
                        if (i.IdMoneda == item.IdMonedaArqueo.Value)
                        {
                            TotalesArqueo.Add(new MonedaMonto { Valor = (Double)item.MontoArqueo.Value, IdMoneda = item.IdMonedaArqueo.Value, Moneda = item.SimboloArqueo });
                        }
                        else
                        {
                            TotalesArqueo.Add(new MonedaMonto
                            {
                                Valor = new ReciboViewModel().ConvertirDivisa(item.IdMonedaArqueo.Value, i.IdMoneda, item.MontoArqueo, fecha),
                                IdMoneda = i.IdMoneda,
                                Moneda = i.Simbolo
                            });
                        }
                    }
                }

                lstDiferencias.Items.Add(item.MonedaDiferencia + " " + string.Format("{0:N}", item.Diferencia));
                foreach (var i in monedas)
                {
                    if (i.IdMoneda == item.IdMonedaDiferencia)
                    {
                        TotalesDiferencia.Add(new MonedaMonto { Valor = (Double)item.Diferencia.Value, IdMoneda = item.IdMonedaDiferencia.Value, Moneda = item.MonedaDiferencia });
                    }
                    else
                    {
                        TotalesDiferencia.Add(new MonedaMonto
                        {
                            Valor = new ReciboViewModel().ConvertirDivisa(item.IdMonedaDiferencia.Value, i.IdMoneda, item.Diferencia),
                            IdMoneda = i.IdMoneda,
                            Moneda = i.Simbolo
                        });
                    }
                }
            }

            //Cargando los totales equivalentes para pagos en efectivo
            var SumETotal = TotalesRecibido.GroupBy(a => new { a.IdMoneda, a.Moneda }).Select(b => new { Moneda = b.Key.Moneda, b.Key.IdMoneda, Monto = b.Sum(c => c.Valor) }).OrderBy(o => o.IdMoneda).ToList();

            recibidoEquivalente = new List<MonedaMonto>();

            foreach (var item in SumETotal)
            {
                lstEfectivoRecibidoEquivalente.Items.Add("Total " + item.Moneda + " " + string.Format("{0:N}", item.Monto));
                recibidoEquivalente.Add(new MonedaMonto() { Moneda = item.Moneda, Valor = item.Monto });
            }

            var SumETotalArqueo = TotalesArqueo.GroupBy(a => new { a.IdMoneda, a.Moneda }).Select(b => new { Moneda = b.Key.Moneda, b.Key.IdMoneda, Monto = b.Sum(c => c.Valor) }).OrderBy(o => o.IdMoneda).ToList();

            arqueoEquivalente = new List<MonedaMonto>();

            foreach (var item in SumETotalArqueo)
            {
                lstEfectivoArqueoEquivalente.Items.Add("Total " + item.Moneda + " " + string.Format("{0:N}", item.Monto));
                arqueoEquivalente.Add(new MonedaMonto() { Moneda = item.Moneda, Valor = item.Monto });
            }

            int efectivo = controller.IdEfectivo();

            for (int i = 0; i < SumETotalArqueo.Count; i++)
            {
                double diferencia = 0;
                if (SumETotal.Any())
                {
                    diferencia = Math.Round(SumETotalArqueo[i].Monto - SumETotal[i].Monto, 2);
                }
                else
                {
                    diferencia = Math.Round(SumETotalArqueo[i].Monto, 2);
                }

                if (diferencia != 0)
                {
                    lstDiferenciasEquivalente.Items.Add(string.Format("{0}, {1} {2}", diferencia < 0 ? "Faltante" : (diferencia == 0 ? "" : "Sobrante"), (SumETotal.Any() ? SumETotal[i].Moneda : SumETotalArqueo[i].Moneda), string.Format("{0:N}", diferencia)));
                    diferenciasTotales.Add(new fn_TotalesArqueo_Result() { IdFormaPago = efectivo, IdMoneda = SumETotalArqueo[i].IdMoneda, IdMonedaDiferencia = SumETotalArqueo[i].IdMoneda, Diferencia = (decimal)diferencia });
                }

            }
        }

        private void BtnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCredencialesCajero())
                {
                    Guargar();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }

        }

        private void Guargar()
        {
            arqueo.Observaciones = txtObservaciones.Text;
            arqueo.UsuarioArqueador = clsSessionHelper.usuario.Login;
            arqueo.FechaFinArqueo = System.DateTime.Now;

            controller.FinalizarArqueo(arqueo, diferenciasTotales);
            InformeArqueo();
            Close();
        }

        private void Finalizar()
        {
            Close();
        }

        private bool ValidarCredencialesCajero()
        {
            List<Usuario> cajerosPermitidos = recibos.Select(s => s.Usuario).Distinct().ToList();
            LoginValidate login = new LoginValidate(cajerosPermitidos.Any() ? clsReferencias.ConfirmarCajeroEntrega : clsReferencias.ConfirmarCajero, cajerosPermitidos);
            login.ShowDialog();
            arqueo.CajeroEntrega = login.cajero;

            return !string.IsNullOrEmpty(arqueo.CajeroEntrega);
        }

        private void InformeArqueo()
        {
            rptInforme cierre = new rptInforme(arqueo);
            cierre.ShowDialog();

        }

        //private void BtnOkDocument_Click(object sender, RoutedEventArgs e)
        //{
        //    CalcularTotalesDocumento();
        //}

        private void LstRecibos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstRecibos.SelectedItem != null)
            {
                CargarPagos();
            }
        }

        private void CargarPagos()
        {
            Recibo1 selected = (Recibo1)lstRecibos.SelectedItem;

            tblFormasPago.ItemsSource = new ReciboViewModel().ReciboFormaPago(new ReciboSon() { IdRecibo = selected.IdRecibo, Serie = selected.Serie });
            tblFormasPago.IsEnabled = !selected.regAnulado;
        }

        private void BtnConfirmPay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReciboPago pago = ((ReciboPago)tblFormasPago.CurrentItem);
                //     controller.ConfirmarPago(pago);
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }

        private void BtnEditPay_Click(object sender, RoutedEventArgs e)
        {
            ReciboPagoSon selected = (ReciboPagoSon)tblFormasPago.CurrentItem;
            EditarPago editarPago = new EditarPago((ReciboPagoSon)selected.Clone());
            editarPago.ShowDialog();

            if (cambios)
            {
                cambios = false;

                CargarPagos();
            }
        }
    }

}
