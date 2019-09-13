﻿using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.ViewModel;
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
        private List<FormaPago> formasPagoDocumentos;
        private ObservableCollection<ArqueoNoEfectivoSon> documentos;
        private System.Windows.Forms.BindingSource ArqueoEfectivoBindingSource = new System.Windows.Forms.BindingSource();
        private System.Windows.Forms.BindingSource DocumentosEfectivoBindingSource = new System.Windows.Forms.BindingSource();
        private List<Moneda> monedas;
        private ObservableCollection<Recibo1> recibos;
        MaterialDesignExtensions.Controllers.StepperController tabmaterial;
        private DateTime? fecha = null;
        clsValidateInput validate = new clsValidateInput();

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
            CamposNormales();
        }

        private void CamposNormales()
        {
            clsValidateInput.Validate(txtMonto, clsValidateInput.DecimalNumber);
            validate.AsignarBorderNormal(new Control[] { cboRecibo, cboDocumento, txtNoDocumento, cboMoneda, txtMonto });
        }

        private bool ValidarCampos()
        {
            return clsValidateInput.ValidateALL(new Control[] { cboRecibo, cboDocumento, txtNoDocumento, cboMoneda, txtMonto });
        }
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
                var tc = new ReciboViewModel().FindTipoCambio(new ReciboSon() { IdRecibo = agregado.IdRecibo, Serie = agregado.Serie, Fecha = agregado.Fecha, IdOrdenPago = agregado.IdOrdenPago, ReciboPago = agregado.ReciboPago }, null).Select(s => string.Format(" {0} -> C$ {1}", s.SimboloMoneda, s.Valor)); ;


                txtTC.Text = string.Format("Fecha de Apertura= {0}, TC={1}", fecha.Value.ToString("dd/MM/yyyy"), string.Join(",", tc).ToString());
            }

        }

        private void btnArqueo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (arqueo.UsuarioArqueador.Equals(""))
                {
                    controller.Guardar(arqueo);

                }
                CargarRecibosContabilizados();
                tabmaterial.Continue();
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
                AsignarFecha(recibos.FirstOrDefault());
            }


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

                CargarRecibosArqueados();
                if (documentos == null) //Lo que esta dentro de este if solo me interesa que se cargue una vez porque no cambiará
                {
                    CargarInfoIdentificador();
                    CargarDocumentosPago();
                    CargarMonedas();
                    CargarDocumentosArqueados();
                }

                tabmaterial.Continue();
            }
            catch (Exception ex)
            {
                lblErrorEfectivo.Text = new clsException(ex).ErrorMessage();
                panelErrorEfectivo.Visibility = Visibility.Visible;
            }
        }

        private void CargarDocumentosPago()
        {

            cboDocumento.ItemsSource = formasPagoDocumentos;
        }

        private void CargarInfoIdentificador()
        {
            formasPagoDocumentos = controller.FindFormasPagoDocumentos(apertura.IdDetAperturaCaja);
            string informacion = null;
            foreach (FormaPago f in formasPagoDocumentos)
            {
                if (!string.IsNullOrEmpty(f.Identificador))
                {
                    if (informacion == null)
                    {
                        informacion = string.Format("{0}: {1},", f.FormaPago1, f.Identificador);
                    }
                    else
                    {
                        informacion = informacion + string.Format(" {0}: {1},", f.FormaPago1, f.Identificador);
                    }
                }
            }
            if (informacion != null)
            {
                if (informacion.Length > 0)
                {
                    txtInfoIdentificador.Text = informacion.Substring(0, informacion.Length - 1);
                }
                else
                {
                    txtInfoIdentificador.Text = "No hay información disponible";
                }
            }
        }

        private void CargarEfectivoContabilizado()
        {
            if (efectivo == null)
            {
                efectivo = new BindingList<ArqueoEfectivoSon>(controller.FindConteoEfectivo(apertura));
                ArqueoEfectivoBindingSource.DataSource = efectivo;

                tblEfectivo.ItemsSource = ArqueoEfectivoBindingSource;
                efectivo.ListChanged += new ListChangedEventHandler(Ef_CollectionChanged);
                CalcularTotales();
            }
        }

        private void CargarRecibosArqueados()
        {
            cboRecibo.ItemsSource = recibos.Select(s => new ArqueoNoEfectivoSon { IdRecibo = s.IdRecibo, Serie = s.Serie }).Distinct();
            panelConteoDocumentos.Text = "" + controller.FindTotalDocumentos(apertura.IdDetAperturaCaja);
        }

        private void CargarMonedas()
        {
            monedas = controller.FindMonedas();
            cboMoneda.ItemsSource = monedas;
        }

        private void CargarDocumentosArqueados()
        {
            documentos = new ObservableCollection<ArqueoNoEfectivoSon>(controller.FindDocumentosArqueados(apertura.IdDetAperturaCaja));
            documentos.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Documentos_CollectionChanged);
            tblDocumentosEfectivo.ItemsSource = documentos;
            if (documentos != null)
            {
                CalcularTotalesDocumento();
            }
        }

        private void Documentos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        private void Ef_CollectionChanged(object sender, ListChangedEventArgs e)
        {
            tblEfectivo.CommitEdit();
            CalcularTotales();
            tblEfectivo.Items.Refresh();
        }

        private void Doc_CollectionChanged(object sender, ListChangedEventArgs e)
        {

            tblEfectivo.CommitEdit();
            CalcularTotalesDocumento();
            tblEfectivo.Items.Refresh();
        }

        private void CalcularTotalesDocumento()
        {
            var total = documentos.GroupBy(g => new { g.FormaPago.FormaPago1, g.Moneda.Moneda1, g.Moneda.Simbolo }).Select(s1 => new { s1.Key.FormaPago1, TotalMoneda = s1.Key.Moneda1, TotalSimbolo = s1.Key.Simbolo, TotalEfectivo = s1.Sum(s => s.MontoFisico) });

            lstDocumentos.ItemsSource = total;
        }

        private void CalcularTotales()
        {
            var total = efectivo.GroupBy(g => new { g.Moneda.Moneda1, g.Moneda.Simbolo }).Select(s1 => new { TotalMoneda = s1.Key.Moneda1, TotalSimbolo = s1.Key.Simbolo, TotalEfectivo = s1.Sum(s => s.Total) });

            lstTotales.ItemsSource = total;
        }

        private void btnConteoNoEfectivo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                controller.GuardarNoEfectivo(documentos.ToList(), arqueo);
                documentos = null;
                CargarDocumentosArqueados();
                TotalSaldoArqueo();
                tabmaterial.Continue();
            }
            catch (Exception ex)
            {
                lblErrorDocumentos.Text = new clsException(ex).ErrorMessage();
                panelErrorDocumentos.Visibility = Visibility.Visible;
            }
        }

        private void CargarResumenTotales()
        {
            throw new NotImplementedException();
        }

        private void BtnDeletePay_Click(object sender, RoutedEventArgs e)
        {
            documentos.Remove((ArqueoNoEfectivoSon)tblDocumentosEfectivo.CurrentItem);
            CalcularTotalesDocumento();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ValidaAgregaDocumento();
        }

        private void ValidaAgregaDocumento()
        {
            if (ValidarCampos())
            {
                Dictionary<TextBox, int> c = new Dictionary<TextBox, int>();
                c.Add(txtMonto, clsValidateInput.DecimalNumber);

                if (ValidarNumericos(c))
                {
                    AgregarDocumento();
                }
            }
        }

        private bool ValidarRepetido(ArqueoNoEfectivoSon noefectivo)
        {
            return documentos.Any(a => a.Recibo == noefectivo.Recibo && a.IdFormaPago == noefectivo.IdFormaPago && a.NoDocumento == noefectivo.NoDocumento && a.MonedaFisica == noefectivo.MonedaFisica);
        }

        private void AgregarDocumento()
        {
            ArqueoNoEfectivoSon noefectivo = new ArqueoNoEfectivoSon();
            var recibo = (ArqueoNoEfectivoSon)cboRecibo.SelectedItem;

            noefectivo.IdRecibo = recibo.IdRecibo;
            noefectivo.Serie = recibo.Serie;

            FormaPago formapago = (FormaPago)cboDocumento.SelectedItem;
            noefectivo.IdFormaPago = formapago.IdFormaPago;
            noefectivo.FormaPago = formapago;

            noefectivo.NoDocumento = txtNoDocumento.Text;

            Moneda moneda = (Moneda)cboMoneda.SelectedItem;
            noefectivo.MonedaFisica = moneda.IdMoneda;
            noefectivo.Moneda = moneda;

            noefectivo.MontoFisico = decimal.Parse(txtMonto.Text);

            if (!ValidarRepetido(noefectivo))
            {
                noefectivo.IdReciboPago = controller.FindReciboPago(noefectivo);

                documentos.Add(noefectivo);

                CalcularTotalesDocumento();
                LimpiarCamposDocumento();
            }
            else
            {
                lblErrorDocumentos.Text = "El documento ya ha sido agregado";
                panelErrorDocumentos.Visibility = Visibility.Visible;
            }
        }

        private void LimpiarCamposDocumento()
        {
            clsValidateInput.CleanALL(new Control[] { cboRecibo, cboDocumento, txtNoDocumento, cboMoneda, txtMonto });

            if (panelErrorDocumentos.Visibility == Visibility.Visible)
            {
                panelErrorDocumentos.Visibility = Visibility.Hidden;
            }
        }

        private void TxtMonto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ValidaAgregaDocumento();
            }
        }

        private void TotalSaldoArqueo()
        {
            var saldos = controller.SaldoTotalArqueo(apertura);

            lstEfectivoRecibido.Items.Clear();
            lstEfectivoRecibidoEquivalente.Items.Clear();
            lstEfectivoArqueo.Items.Clear();
            lstEfectivoArqueoEquivalente.Items.Clear();
            lstDiferencias.Items.Clear();
            lstDiferenciasEquivalente.Items.Clear();
            SaldosEfectivo(saldos[0]);

            lstPendienteDocumento.Items.Clear();
            //lstDocumentoMatch.Items.Clear();
            DocumentosNoEnlazados(saldos[1]);


        }

        private void DocumentosNoEnlazados(List<fn_TotalesArqueo_Result> list)
        {
         
            List<ArqueoNoEfectivoSon> documentosMatch = documentos.Where(w=>w.IdReciboPago == null).Union(controller.FindDocumentosNoEnlazados(apertura)).ToList();
            tblDocumentosMatch.ItemsSource = documentosMatch;

            foreach (var item in list)
            {
                if (item.Diferencia != 0)
                {
                    lstPendienteDocumento.Items.Add(string.Format("{0}, {1} {2} {3}", item.Diferencia > 0 ? "Sobrante" : "Faltante", item.FormaPago, item.MonedaDiferencia, item.Diferencia));
                }
            }
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

            for (int i = 0; i < SumETotalArqueo.Count; i++)
            {
                double diferencia = SumETotalArqueo[i].Monto - SumETotal[i].Monto;
                lstDiferenciasEquivalente.Items.Add(diferencia < 0 ? "Faltante " : (diferencia == 0 ? "" : "Sobrante ") + SumETotal[i].Moneda + " " + string.Format("{0:N}", diferencia));
            }
        }
    }

}

//19XFB2F7XFE081917