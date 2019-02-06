using Microsoft.Reporting.WinForms;
using PruebaWPF.Helper;
using PruebaWPF.Model;
using PruebaWPF.ViewModel;
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

namespace PruebaWPF.Views.Shared
{
    /// <summary>
    /// Lógica de interacción para rptInforme.xaml
    /// </summary>
    public partial class rptInforme : Window
    {
        private DetAperturaCajaSon detApertura;

        public rptInforme()
        {
            InitializeComponent();
        }
        public rptInforme(DetAperturaCajaSon detApertura)
        {
            this.detApertura = detApertura;
            InitializeComponent();
            Title = "Informe de cierre de caja";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InformeCierreCaja();
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
            
            foreach (ReciboSon item in recibos.Where(w=>w.regAnulado==false))
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
            List<ReciboAnulado> anulados;

            if (recibos.Where(w => w.regAnulado == true).Count()>0)
            {
            anulados = recibos.Where(w => w.regAnulado == true).Select(s=>new ReciboAnulado() { IdRecibo=s.IdRecibo,Serie=s.Serie,Motivo=s.ReciboAnulado.Motivo,UsuarioAnulacion=s.ReciboAnulado.UsuarioAnulacion}).ToList();
            }
            else
            {
                anulados = new List<ReciboAnulado>();
            }

            ReportDataSource ReciboDataSource = new ReportDataSource("ReciboPagos", vista);
            ReportDataSource CajaDataSource = new ReportDataSource("Caja", caja);
            ReportDataSource AperturaDataSource = new ReportDataSource("Apertura", apertura);
            ReportDataSource DetAperturaDataSource = new ReportDataSource("DetApertura", detalleApertura);
            ReportDataSource AnuladosDataSource = new ReportDataSource("ReciboAnulado", anulados);

            informe.Reset();
            informe.LocalReport.ReportEmbeddedResource = "PruebaWPF.Reportes.AperturaCaja.InformeCierre.rdlc";
            informe.LocalReport.SetParameters(new ReportParameter("UserParameter", clsSessionHelper.usuario.Login));

            AddDataSource(new ReportDataSource[] { ReciboDataSource,CajaDataSource,AperturaDataSource,DetAperturaDataSource,AnuladosDataSource});


            informe.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            informe.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
            informe.ZoomPercent = 100;
            informe.LocalReport.Refresh();
        }

        private void AddDataSource(ReportDataSource[] datasources)
        {
            for (int i = 0; i < datasources.Length; i++)
            {
                informe.LocalReport.DataSources.Add(datasources[i]);
            }
        }


    }
}
