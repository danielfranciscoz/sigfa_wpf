using Microsoft.Reporting.WinForms;
using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using System;
using System.Collections.Generic;
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
        private List<ReciboPago> pagos;
        private ColumnasFiltro filtros;

        public rptInforme()
        {
            InitializeComponent();
        }
        public rptInforme(DetAperturaCajaSon detApertura)
        {
            reporte_id = (int)clsReferencias.Informes.cierre_caja;
            this.detApertura = detApertura;
            InitializeComponent();
            Title = "Reporte de cierre de caja";
        }

        public rptInforme(Model.Arqueo arqueo)
        {
            reporte_id = (int)clsReferencias.Informes.arqueo_caja;
            this.arqueo = arqueo;
            InitializeComponent();
            Title = "Reporte de arqueo de caja";
        }

        public rptInforme(List<ReciboPago> pagos, ColumnasFiltro filtros)
        {
            reporte_id = (int)clsReferencias.Informes.informe_general_ingresos;
            this.pagos = pagos;
            this.filtros = filtros;
            InitializeComponent();
            Title = "Reporte general de ingresos";
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
                case (int)clsReferencias.Informes.informe_general_ingresos:
                    InformeGeneralIngresos(pagos, filtros);
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
            List<DocumentosEfectivo> DocumentosArqueados = controller.FindDocumentosEfectivo(arqueo.DetAperturaCaja);

            
            List<VariacionCambiariaSon> variacionCambiarias = controller.FindTipoCambios(arqueo.DetAperturaCaja.AperturaCaja);
            List<Rectificaciones> rectificaciones = controller.FindRectificaciones(arqueo.IdArqueoDetApertura);
            List<DiferenciasArqueo> diferenciasArqueo = controller.DiferenciasArqueo(arqueo.IdArqueoDetApertura);

            List<Model.Recibo> recibos = new List<Model.Recibo>();
            List<Model.Recibo> recibosAnulados = new List<Model.Recibo>();


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
            datasSource[5] = new ReportDataSource("DocumentosEfectivo", DocumentosArqueados);
            datasSource[6] = new ReportDataSource("TipoCambio", variacionCambiarias);
            datasSource[7] = new ReportDataSource("Rectificaciones", rectificaciones);
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
            List<Rectificaciones> rectificaciones = controller.FindRectificaciones(detApertura.IdDetAperturaCaja);

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
                        vista_RecibosPago vr = new vista_RecibosPago
                        {
                            IdRecibo = item.IdRecibo,
                            Serie = item.Serie,
                            porCuenta = item.Recibimos,
                            FormaPago = pay.FormaPago.FormaPago1,
                            Moneda = pay.Moneda.Moneda1,
                            Monto = Double.Parse(pay.Monto.ToString())
                        };
                        vista.Add(vr);
                    }
                }
            }


            datasSource = new ReportDataSource[6];

            datasSource[0] = new ReportDataSource("ReciboPagos", vista);
            datasSource[1] = new ReportDataSource("Caja", caja);
            datasSource[2] = new ReportDataSource("Apertura", apertura);
            datasSource[3] = new ReportDataSource("DetApertura", detalleApertura);
            datasSource[4] = new ReportDataSource("ReciboAnulado", anulados);
            datasSource[5] = new ReportDataSource("Rectificaciones", rectificaciones);

            clsUtilidades.InformeDataSource(informe, datasSource);

            ParametrosComunes(informe, "PruebaWPF.Reportes.AperturaCaja.InformeCierre.rdlc");

            informe.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            informe.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
            informe.ZoomPercent = 100;
            informe.LocalReport.Refresh();
        }

        private void InformeGeneralIngresos(List<ReciboPago> pagos, ColumnasFiltro columnasFiltro)
        {
            ReciboViewModel controller = new ReciboViewModel();

            List<ColumnasFiltro> columnas = new List<ColumnasFiltro>();
            columnasFiltro.Fecha = GenerarFecha(columnasFiltro.startdate, columnasFiltro.enddate);
            columnasFiltro.Recinto = columnasFiltro.Recinto != null ? columnasFiltro.Recinto : "TODOS LOS RECINTOS";
            columnasFiltro.Area = columnasFiltro.Area != null ? columnasFiltro.Area : "TODAS LAS ÁREAS";
            columnasFiltro.Caja = columnasFiltro.Caja != null ? columnasFiltro.Caja : "TODAS LAS CAJAS";
            columnas.Add(columnasFiltro);

            datasSource = new ReportDataSource[2];

            datasSource[0] = new ReportDataSource("ReciboPagos", pagos);
            datasSource[1] = new ReportDataSource("Filtros", columnas);

            clsUtilidades.InformeDataSource(informe, datasSource);

            ParametrosComunes(informe, "PruebaWPF.Reportes.Informes.InformeGeneralIngresos.rdlc");

            informe.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            informe.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
            informe.ZoomPercent = 100;
            informe.LocalReport.Refresh();
        }

        private string GenerarFecha(DateTime? startdate, DateTime? enddate)
        {
            string fecha = "";
            if (startdate.HasValue && enddate.HasValue)
            {
                fecha = string.Format("Desde el {0} hasta el {1}", startdate.Value.ToString(@"dd \de MMMM \del yyyy"), enddate.Value.ToString(@"dd \de MMMM \del yyyy"));
            }
            else if (startdate.HasValue)
            {
                fecha = string.Format("Desde el {0} hasta la fecha de impresión de este informe", startdate.Value.ToString(@"dd \de MMMM \del yyyy"));
            }
            else if (enddate.HasValue)
            {
                fecha = string.Format("Desde el inicio hasta el {0}", enddate.Value.ToString(@"dd \de MMMM \del yyyy"));
            }
            else
            {
                fecha = string.Format("Desde el inicio hasta la fecha de impresión de este informe");
            }

            return fecha;
        }

        private void ParametrosComunes(ReportViewer informe, string reportURL)
        {

            informe.LocalReport.ReportEmbeddedResource = reportURL;
            informe.LocalReport.SetParameters(new ReportParameter("UserParameter", clsSessionHelper.usuario.Login));
            informe.LocalReport.SetParameters(new ReportParameter("SystemNameParameter", string.Format("{0} \nV.{1}", clsSessionHelper.SystemName, clsSessionHelper.SystemVersion)));
        }



    }
}
