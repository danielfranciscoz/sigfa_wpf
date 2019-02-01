using Microsoft.Reporting.WinForms;
using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace PruebaWPF.Views.Recibo
{
    /// <summary>
    /// Lógica de interacción para rptRecibo.xaml
    /// </summary>
    public partial class rptRecibo : Window
    {
        private ReciboSon recibo;
        private InfoRecibo info;
        private bool isFirtTime;
        public rptRecibo()
        {
            InitializeComponent();
        }

        public rptRecibo(ReciboSon recibo, Boolean isFirstTime)
        {
            this.recibo = recibo;
            InitializeComponent();
            this.Title = "Recibo Oficial de Caja Número " + recibo.IdRecibo + "-" + recibo.Serie;
            this.isFirtTime = isFirstTime;
        }

        public rptRecibo(InfoRecibo info, Boolean isFirstTime)
        {
            this.info = info;
            InitializeComponent();
            this.Title = "Previsualización de Encabezado y Pie de Recibo";
            this.isFirtTime = isFirstTime;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (recibo != null)
                {
                    VerRecibo();
                }
                else
                {
                    VerPreview();
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
                Close();
            }
        }

        private void VerPreview()
        {
            List<ReciboSon> recibos = new List<ReciboSon>();
            recibos.Add(new ReciboSon() { UsuarioCreacion=clsSessionHelper.usuario.Login,Serie="VISTA PREVIA",Recibimos="XXXXXXXX"});

            List<Model.OrdenPago> ordenPago = new List<Model.OrdenPago>();
            ordenPago.Add(new Model.OrdenPago());

            List<InfoRecibo> infos = new List<InfoRecibo>();
            infos.Add(info);

            List<clsBarCode> barcode = new List<clsBarCode>();
            barcode.Add(new clsBarCode() { texto = "CBR-0000" });

            List<fn_ConsultarInfoExterna_Result> cuenta = new List<fn_ConsultarInfoExterna_Result>();
            cuenta.Add(new fn_ConsultarInfoExterna_Result() { Id="ESTO NO ES UN RECIBO OFICIAL DE CAJA"});

            List<DetReciboSon> detrecibo = new List<DetReciboSon>();
            detrecibo.Add(new DetReciboSon() { Concepto="",Descuento=0,Monto=0,ArancelPrecio= new ArancelPrecio() { Arancel=new Arancel(),Moneda=new Moneda()} });

            List<ReciboPagoSon> formaPago = new List<ReciboPagoSon>();
            formaPago.Add(new ReciboPagoSon() {Monto=0,InfoAdicional="",DetalleAdicional="",Moneda=new Moneda(),FormaPago=new FormaPago()});

            List<VariacionCambiariaSon> variacionCambiarias = new List<VariacionCambiariaSon>();
            variacionCambiarias.Add(new VariacionCambiariaSon() { Valor=0,Moneda=new Moneda()});

            CargarVisor(recibos, ordenPago, barcode, infos, cuenta, detrecibo, formaPago, variacionCambiarias);
        }
 
        private void VerRecibo()
        {
            //   SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            ReciboViewModel modeloRecibo = new ReciboViewModel();

            List<ReciboSon> recibos = modeloRecibo.FindRecibo(this.recibo.IdRecibo, this.recibo.Serie);


            var recibo = recibos.FirstOrDefault();

            List<Model.OrdenPago> ordenPago = new List<Model.OrdenPago>();
            ordenPago.Add(recibo.OrdenPago);

            List<InfoRecibo> infos = new List<InfoRecibo>();
            infos.Add(recibo.InfoRecibo);

            List<clsBarCode> barcode = new List<clsBarCode>();
            barcode.Add(new clsBarCode() { texto = "CBR-" + recibo.IdRecibo + "-" + recibo.Serie });

            List<fn_ConsultarInfoExterna_Result> cuenta = new SearchTipoDepositoViewModel().ObtenerTipoDeposito(string.IsNullOrEmpty(recibo.IdTipoDeposito.Value.ToString()) ? 0 : recibo.IdTipoDeposito.Value, recibo.Identificador, true, "", 1);

            List<DetReciboSon> detrecibo = modeloRecibo.DetallesRecibo(recibo);
            List<ReciboPagoSon> formaPago = modeloRecibo.ReciboFormaPago(recibo);

            List<VariacionCambiariaSon> variacionCambiarias = modeloRecibo.FindTipoCambio(recibo);

            CargarVisor(recibos, ordenPago, barcode, infos, cuenta, detrecibo, formaPago, variacionCambiarias);
        }

        private void CargarVisor(List<ReciboSon> recibos, List<Model.OrdenPago> ordenPago, List<clsBarCode> barcode, List<InfoRecibo> infos, List<fn_ConsultarInfoExterna_Result> cuenta, List<DetReciboSon> detrecibo, List<ReciboPagoSon> formaPago, List<VariacionCambiariaSon> variacionCambiarias)
        {
            ReportDataSource ReciboDataSource = new ReportDataSource("Recibo", recibos);
            ReportDataSource OrdenPagoDataSource = new ReportDataSource("OrdenPago", ordenPago);
            ReportDataSource BarcodeDataSource = new ReportDataSource("Barcode", barcode);
            ReportDataSource InfoReciboDataSource = new ReportDataSource("InfoRecibo", infos);
            ReportDataSource PorCuentaDataSource = new ReportDataSource("CuentaDe", cuenta);
            ReportDataSource DetReciboDataSource = new ReportDataSource("DetalleRecibo", detrecibo);
            ReportDataSource FormaPagoDataSource = new ReportDataSource("FormaPago", formaPago);
            ReportDataSource TipoCambioDataSource = new ReportDataSource("TipoCambio", variacionCambiarias);

            reporteDemo.Reset();
            reporteDemo.LocalReport.ReportEmbeddedResource = "PruebaWPF.Reportes.Recibo.Recibo.rdlc";
            reporteDemo.LocalReport.SetParameters(new ReportParameter("isFirstTime", "True"));
            reporteDemo.LocalReport.DataSources.Add(ReciboDataSource);
            reporteDemo.LocalReport.DataSources.Add(OrdenPagoDataSource);
            reporteDemo.LocalReport.DataSources.Add(InfoReciboDataSource);
            reporteDemo.LocalReport.DataSources.Add(BarcodeDataSource);
            reporteDemo.LocalReport.DataSources.Add(PorCuentaDataSource);
            reporteDemo.LocalReport.DataSources.Add(DetReciboDataSource);
            reporteDemo.LocalReport.DataSources.Add(FormaPagoDataSource);
            reporteDemo.LocalReport.DataSources.Add(TipoCambioDataSource);

            reporteDemo.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            reporteDemo.LocalReport.Refresh();
        }

    }
}
