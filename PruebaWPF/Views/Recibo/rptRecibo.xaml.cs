﻿using Microsoft.Reporting.WinForms;
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

        public rptRecibo(InfoRecibo info)
        {
            this.info = info;
            InitializeComponent();
            this.Title = "Previsualización de Encabezado y Pie de Recibo";
            this.isFirtTime = true;
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
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
                Close();
            }
        }

        private void VerPreview()
        {
            List<ReciboSon> recibos = new List<ReciboSon>();
            recibos.Add(new ReciboSon() { UsuarioCreacion = clsSessionHelper.usuario.Login, Serie = "VISTA PREVIA", Recibimos = "XXXXXXXX" });

            List<Model.OrdenPago> ordenPago = new List<Model.OrdenPago>();
            ordenPago.Add(new Model.OrdenPago());

            List<InfoRecibo> infos = new List<InfoRecibo>();
            infos.Add(info);

            List<clsBarCode> barcode = new List<clsBarCode>();
            barcode.Add(new clsBarCode() { texto = "0000-A" });

            List<fn_ConsultarInfoExterna_Result> cuenta = new List<fn_ConsultarInfoExterna_Result>();
            cuenta.Add(new fn_ConsultarInfoExterna_Result() { Id = "ESTO NO ES UN RECIBO OFICIAL DE CAJA" });

            List<DetReciboSon> detrecibo = new List<DetReciboSon>();
            detrecibo.Add(new DetReciboSon() { Concepto = "", Monto = 0, ArancelPrecio = new ArancelPrecio() { ArancelArea = new ArancelArea() { Arancel = new Arancel() { Nombre = "Arancel pagado" } }, Moneda = new Moneda() } });

            List<ReciboPagoSon> formaPago = new List<ReciboPagoSon>();
            //formaPago.Add(new ReciboPagoSon() { Monto = 0, ObjInfoAdicional = new object [2]{"","" }, Moneda = new Moneda(), FormaPago = new FormaPago() { FormaPago1 = "Forma de pago 1" } });
            formaPago.Add(new ReciboPagoSon() { Monto = 0, Moneda = new Moneda(), FormaPago = new FormaPago() { FormaPago1 = "Forma de pago 1" } });

            List<VariacionCambiariaSon> variacionCambiarias = new List<VariacionCambiariaSon>();
            variacionCambiarias.Add(new VariacionCambiariaSon() { Valor = 0, Moneda = new Moneda() });

            CargarVisor(recibos, ordenPago, barcode, infos, cuenta, detrecibo, formaPago, variacionCambiarias, true);
        }

        public void VerRecibo()
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
            barcode.Add(new clsBarCode() { texto = recibo.IdRecibo + "-" + recibo.Serie });

            List<fn_ConsultarInfoExterna_Result> cuenta = new List<fn_ConsultarInfoExterna_Result>();

            fn_ConsultarInfoExterna_Result info = new fn_ConsultarInfoExterna_Result();

            info.Nombre = recibo.TextoIdentificador;
            //if (modeloRecibo.isAgenteExterno(recibo.IdTipoDeposito))
            //{
            //    AgenteExternoCat agente = new AgenteExternoViewModel().FindById(int.Parse(recibo.Identificador));
            //    if (agente != null)
            //    {
            //        info.Id = agente.Identificacion;
            //        info.IdentificatorType = agente.IdentificacionAgenteExterno.Identificacion;
            //    }
            //    else
            //    {
            //        info.Id = "XXX-XXX";
            //        info.IdentificatorType = "XXXXXX";
            //    }
            //}
            //else
            //{
            //    info.Id = recibo.Identificador;
            //    info.IdentificatorType = recibo.TipoDeposito.TextHint;

            //}

            info.Id = recibo.Identificador;
            info.IdentificatorType = recibo.TipoDeposito.TextHint;
            info.Info1 = recibo.ReciboDatos != null ? (recibo.ReciboDatos.Obervacion ?? "Ninguno") : (recibo.OrdenPago.Observacion ?? "Ninguno");

            cuenta.Add(info);
            //  new SearchTipoDepositoViewModel().ObtenerTipoDeposito(string.IsNullOrEmpty(recibo.IdTipoDeposito.Value.ToString()) ? 0 : recibo.IdTipoDeposito.Value, recibo.Identificador, true, "", 1,null);

            List<DetReciboSon> detrecibo = modeloRecibo.DetallesRecibo(recibo);
            List<ReciboPagoSon> formaPago = modeloRecibo.ReciboFormaPagoConsolidado(recibo);

            List<VariacionCambiariaSon> variacionCambiarias = modeloRecibo.FindTipoCambio(recibo, detrecibo);

            CargarVisor(recibos, ordenPago, barcode, infos, cuenta, detrecibo, formaPago, variacionCambiarias, false);
        }

        private void CargarVisor(List<ReciboSon> recibos, List<Model.OrdenPago> ordenPago, List<clsBarCode> barcode, List<InfoRecibo> infos, List<fn_ConsultarInfoExterna_Result> cuenta, List<DetReciboSon> detrecibo, List<ReciboPagoSon> formaPago, List<VariacionCambiariaSon> variacionCambiarias, bool isPreview)
        {
            ReportDataSource[] datasSource = new ReportDataSource[8];

            datasSource[0] = new ReportDataSource("Recibo", recibos);
            datasSource[1] = new ReportDataSource("OrdenPago", ordenPago);
            datasSource[2] = new ReportDataSource("Barcode", barcode);
            datasSource[3] = new ReportDataSource("InfoRecibo", infos);
            datasSource[4] = new ReportDataSource("CuentaDe", cuenta);
            datasSource[5] = new ReportDataSource("DetalleRecibo", detrecibo);
            datasSource[6] = new ReportDataSource("FormaPago", formaPago);
            datasSource[7] = new ReportDataSource("TipoCambio", variacionCambiarias);


            clsUtilidades.InformeDataSource(informe, datasSource);
            informe.LocalReport.ReportEmbeddedResource = "PruebaWPF.Reportes.Recibo.Recibo.rdlc";
            informe.LocalReport.SetParameters(new ReportParameter("isFirstTime", isFirtTime.ToString()));

            if (isFirtTime && !isPreview) //Cuando es primera vez, se realiza la verificacion para ver si se enviará correo
            {
                if (ordenPago.Any())
                {
                    Model.OrdenPago o = ordenPago.FirstOrDefault();
                    if (o != null && clsSessionHelper.entorno == clsReferencias.Release)
                    {
                        string emailTo = o.EmailNotificacion;
                        if (!string.IsNullOrEmpty(emailTo))
                        {
                            Email e = new Email();
                            e.SendRecibo(informe, string.Format("{0}-{1}", recibo.IdRecibo, recibo.Serie), cuenta.First().Nombre, emailTo, o.Usuario.LoginEmail);
                        }
                    }
                }

                AutoPrint autoprintme = new AutoPrint(informe.LocalReport);
                autoprintme.Print();
            }
            else
            {
                informe.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                informe.LocalReport.Refresh();
            }





        }

    }

}
