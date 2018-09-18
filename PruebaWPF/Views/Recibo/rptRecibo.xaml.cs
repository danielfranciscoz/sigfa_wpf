using Confortex.Clases;
using Microsoft.Reporting.WinForms;
using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        public rptRecibo()
        {
            InitializeComponent();
        }

        public rptRecibo(ReciboSon recibo)
        {
            this.recibo = recibo;
            InitializeComponent();
            this.Title = "Recibo Oficial de Caja Número " + recibo.IdRecibo + "-" + recibo.Serie;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                VerReporte();
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
                Close();
            }
        }

        private void VerReporte()
        {
            //   SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            ReciboViewModel modeloRecibo = new ReciboViewModel();

            List<ReciboSon> recibos = modeloRecibo.FindRecibo(this.recibo.IdRecibo, this.recibo.Serie);


            var recibo = recibos.FirstOrDefault();

            List<InfoRecibo> info = new List<InfoRecibo>();
            info.Add(recibo.InfoRecibo);

            List<clsBarCode> barcode = new List<clsBarCode>();
            barcode.Add(new clsBarCode() { texto = "CBR-" + recibo.IdRecibo + "-" + recibo.Serie });

            List<fn_ConsultarInfoExterna_Result> cuenta = new SearchTipoDepositoViewModel().ObtenerTipoDeposito(string.IsNullOrEmpty(recibo.IdTipoDeposito.Value.ToString()) ? 0 : recibo.IdTipoDeposito.Value, recibo.Identificador, true, "", 1);

            List<DetReciboSon> detrecibo = modeloRecibo.DetallesRecibo(recibo);
            List<ReciboPagoSon> formaPago = modeloRecibo.ReciboFormaPago(recibo);

            List<VariacionCambiariaSon> variacionCambiarias = modeloRecibo.FindTipoCambio(recibo);

            ReportDataSource ReciboDataSource = new ReportDataSource("Recibo", recibos);
            ReportDataSource BarcodeDataSource = new ReportDataSource("Barcode", barcode);
            ReportDataSource InfoReciboDataSource = new ReportDataSource("InfoRecibo", info);
            ReportDataSource PorCuentaDataSource = new ReportDataSource("CuentaDe", cuenta);
            ReportDataSource DetReciboDataSource = new ReportDataSource("DetalleRecibo", detrecibo);
            ReportDataSource FormaPagoDataSource = new ReportDataSource("FormaPago", formaPago);
            ReportDataSource TipoCambioDataSource = new ReportDataSource("TipoCambio", variacionCambiarias);


            reporteDemo.Reset();
            reporteDemo.LocalReport.ReportEmbeddedResource = "PruebaWPF.Reportes.Recibo.Recibo.rdlc";

            reporteDemo.LocalReport.DataSources.Add(ReciboDataSource);
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
