﻿using PruebaWPF.Helper;
using PruebaWPF.Referencias;
using System.Collections.Generic;
using System.Linq;

namespace PruebaWPF.Model
{
    class PartialClases
    {
    }

    public partial class Recibo1
    {
        public string NoOrdenPago => IdOrdenPago == null ? (regAnulado ? (ReciboAnulado.IdOrdenPago == null ? "" : ReciboAnulado.OrdenPago.NoOrdenPago) : "") : OrdenPago.NoOrdenPago;
    }

    public partial class DiferenciasArqueo
    {
        public string FormaPagoName => FormaPago.FormaPago1;
        public bool isDoc => FormaPago.isDoc;
        public string SimboloMoneda => Moneda.Simbolo;
        public string Resultado => Monto == 0 ? "" : (Monto < 0 ? "Faltante" : "Sobrante");

    }

    public partial class Arqueo
    {
        public string EstadoArqueo => isFinalizado ? clsReferencias.Finalizado : clsReferencias.EnProceso;
        public double SaldoInicial => DetAperturaCaja.AperturaCaja.SaldoInicial;

        public string anulados => string.Join(",", DetAperturaCaja.Recibo1.Where(w => w.regAnulado == true).Select(s => string.Format("{0}-{1}", s.IdRecibo, s.Serie)));
        public string Recibos => DetAperturaCaja.Recibo1.Count > 0 ? ObtenerRecibos() : "";

        private string ObtenerRecibos()
        {
            List<Recibo1> recibos = DetAperturaCaja.Recibo1.ToList();
            Recibo1 minimo = recibos.FirstOrDefault();
            Recibo1 maximo = recibos.LastOrDefault();

            return string.Format("Desde {0}-{1} Hasta {2}-{3}", minimo.IdRecibo, minimo.Serie, maximo.IdRecibo, maximo.Serie);
        }
    }

    public partial class DetAperturaCaja
    {
        public string Recibos => Recibo1.Count > 0 ? ObtenerRecibos() : "";

        private string ObtenerRecibos()
        {
            List<Recibo1> recibos = Recibo1.ToList();
            Recibo1 minimo = recibos.FirstOrDefault();
            Recibo1 maximo = recibos.LastOrDefault();

            return string.Format("Desde {0}-{1} Hasta {2}-{3}", minimo.IdRecibo, minimo.Serie, maximo.IdRecibo, maximo.Serie);
        }
    }

    public partial class MovimientoIngreso
    {
        public string Recinto { get; set; }
    }
}
