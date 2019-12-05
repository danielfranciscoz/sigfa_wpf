using Microsoft.Reporting.WinForms;
using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace PruebaWPF.Views.Shared
{
    /// <summary>
    /// Lógica de interacción para rptInforme.xaml
    /// </summary>
    public partial class rptInforme : Window
    {
        private int reporte_id;
        private DetAperturaCajaSon detApertura;
        private Model.Arqueo arqueo;
        ReportDataSource[] datasSource;
        public rptInforme()
        {
            InitializeComponent();
        }
        public rptInforme(DetAperturaCajaSon detApertura)
        {
            reporte_id = (int)clsReferencias.Informes.cierre_caja;
            this.detApertura = detApertura;
            InitializeComponent();
            Title = "Informe de cierre de caja";
        }

        public rptInforme(Model.Arqueo arqueo)
        {
            reporte_id = (int)clsReferencias.Informes.arqueo_caja;
            this.arqueo = arqueo;
            InitializeComponent();
            Title = "Informe de arqueo de caja";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarInforme();
        }

        private void CargarInforme()
        {
            switch (reporte_id)
            {
                case (int)clsReferencias.Informes.cierre_caja:
                    InformeCierreCaja();
                    break;

                case (int)clsReferencias.Informes.arqueo_caja:
                    InformeArqueoCaja();
                    break;

                default: break;
            }
        }

        private void InformeArqueoCaja()
        {
            ArqueoViewModel controller = new ArqueoViewModel();
            List<Model.Arqueo> arqueoFinalizado = new List<Model.Arqueo>();
            List<ArqueoEfectivoSon> efectivo = controller.FindConteoEfectivo(arqueo.IdArqueoDetApertura, false);
            List<fn_TotalesArqueo_Result>[] recibido = controller.SaldoTotalArqueo(arqueo.DetAperturaCaja);
            List<ArqueoNoEfectivoSon> DocumentosArqueados = controller.FindDocumentosArqueados(arqueo.IdArqueoDetApertura);


            List<VariacionCambiariaSon> variacionCambiarias = controller.FindTipoCambios(arqueo.IdArqueoDetApertura);
            List<ArqueoNoEfectivoSon> documentosNoEnlazados = controller.FindDocumentosNoEnlazados(arqueo.IdArqueoDetApertura);
            List<DiferenciasArqueo> diferenciasArqueo = controller.DiferenciasArqueo(arqueo.IdArqueoDetApertura);

            List<Recibo1> recibos = new List<Recibo1>();
            List<Recibo1> recibosAnulados = new List<Recibo1>();


            foreach (var item in controller.FindRecibosContabilizados(arqueo))
            {
                if (item.regAnulado)
                {
                    recibosAnulados.Add(item);
                }
                else
                {
                    recibos.Add(item);
                }
            }
            arqueoFinalizado.Add(controller.FindById(arqueo.IdArqueoDetApertura));

            datasSource = new ReportDataSource[9];

            datasSource[0] = new ReportDataSource("ArqueoEfectivo", efectivo);
            datasSource[1] = new ReportDataSource("Recibos", recibos);
            datasSource[2] = new ReportDataSource("RecibosAnulados", recibosAnulados);
            datasSource[3] = new ReportDataSource("Arqueo", arqueoFinalizado);
            datasSource[4] = new ReportDataSource("EfectivoRecibido", recibido[0]);
            datasSource[5] = new ReportDataSource("ArqueoNoEfectivo", DocumentosArqueados);
            datasSource[6] = new ReportDataSource("TipoCambio", variacionCambiarias);
            datasSource[7] = new ReportDataSource("DocumentosNoEnlazados", documentosNoEnlazados);
            datasSource[8] = new ReportDataSource("DiferenciasArqueo", diferenciasArqueo);



            clsUtilidades.InformeDataSource(informe, datasSource);

            ParametrosComunes(informe, "PruebaWPF.Reportes.Arqueo.Arqueo.rdlc");
            informe.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            informe.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
            informe.ZoomPercent = 100;
            informe.LocalReport.Refresh();
        }

        private void InformeCierreCaja()
        {
            ReciboViewModel controller = new ReciboViewModel();

            List<ReciboSon> recibos = controller.FindAllApertura(detApertura.IdDetAperturaCaja);

            List<Model.Caja> caja = new List<Model.Caja>();
            caja.Add(detApertura.Caja);

            List<Model.AperturaCaja> apertura = new List<Model.AperturaCaja>();
            apertura.Add(detApertura.AperturaCaja);

            List<DetAperturaCajaSon> detalleApertura = new List<DetAperturaCajaSon>();
            detalleApertura.Add(detApertura);

            List<vista_RecibosPago> vista = new List<vista_RecibosPago>();
            List<ReciboAnulado> anulados = new List<ReciboAnulado>();

            foreach (ReciboSon item in recibos)
            {
                if (item.regAnulado)
                {
                    anulados.Add(new ReciboAnulado() { IdRecibo = item.IdRecibo, Serie = item.Serie, Motivo = item.ReciboAnulado.Motivo, UsuarioAnulacion = item.ReciboAnulado.UsuarioAnulacion });
                }
                else
                {
                    foreach (ReciboPagoSon pay in controller.ReciboFormaPago(item))
                    {
                        vista_RecibosPago vr = new vista_RecibosPago();
                        vr.IdRecibo = item.IdRecibo;
                        vr.Serie = item.Serie;
                        vr.porCuenta = item.Recibimos;
                        vr.FormaPago = pay.FormaPago.FormaPago1;
                        vr.Moneda = pay.Moneda.Moneda1;
                        vr.Monto = Double.Parse(pay.Monto.ToString());
                        vista.Add(vr);
                    }
                }
            }


            datasSource = new ReportDataSource[5];

            datasSource[0] = new ReportDataSource("ReciboPagos", vista);
            datasSource[1] = new ReportDataSource("Caja", caja);
            datasSource[2] = new ReportDataSource("Apertura", apertura);
            datasSource[3] = new ReportDataSource("DetApertura", detalleApertura);
            datasSource[4] = new ReportDataSource("ReciboAnulado", anulados);

            clsUtilidades.InformeDataSource(informe, datasSource);

            ParametrosComunes(informe, "PruebaWPF.Reportes.AperturaCaja.InformeCierre.rdlc");

            informe.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            informe.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
            informe.ZoomPercent = 100;
            informe.LocalReport.Refresh();
        }

        private void ParametrosComunes(ReportViewer informe, string reportURL)
        {
            string systemName = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;
            string systemVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();


            informe.LocalReport.ReportEmbeddedResource = reportURL;
            informe.LocalReport.SetParameters(new ReportParameter("UserParameter", clsSessionHelper.usuario.Login));
            informe.LocalReport.SetParameters(new ReportParameter("SystemNameParameter", string.Format("{0} \nV.{1}", systemName, systemVersion)));
        }



    }
}
