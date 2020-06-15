using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PruebaWPF.Model
{
    class PartialClases
    {
    }

    public partial class Recibo
    {
        public string NoOrdenPago => IdOrdenPago == null ? (regAnulado ? (ReciboAnulado.IdOrdenPago == null ? "" : ReciboAnulado.OrdenPago.NoOrdenPago) : "") : OrdenPago.NoOrdenPago;
        public string IdAreaUnion => IdOrdenPago == null ? ReciboDatos.IdArea : OrdenPago.IdArea;
        
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

        public string anulados => string.Join(",", DetAperturaCaja.Recibo.Where(w => w.regAnulado == true).Select(s => string.Format("{0}-{1}", s.IdRecibo, s.Serie)));
        public string Recibos => DetAperturaCaja.Recibo.Count > 0 ? ObtenerRecibos() : "";

        private string ObtenerRecibos()
        {
            List<Recibo> recibos = DetAperturaCaja.Recibo.ToList();
            Recibo minimo = recibos.FirstOrDefault();
            Recibo maximo = recibos.LastOrDefault();

            return string.Format("Desde {0}-{1} Hasta {2}-{3}", minimo.IdRecibo, minimo.Serie, maximo.IdRecibo, maximo.Serie);
        }
    }

    public partial class DetAperturaCaja
    {
        public string Recibos => Recibo.Count > 0 ? ObtenerRecibos() : "";

        private string ObtenerRecibos()
        {
            List<Recibo> recibos = Recibo.ToList();
            Recibo minimo = recibos.FirstOrDefault();
            Recibo maximo = recibos.LastOrDefault();

            return string.Format("Desde {0}-{1} Hasta {2}-{3}", minimo.IdRecibo, minimo.Serie, maximo.IdRecibo, maximo.Serie);
        }
    }

    public partial class CuentaContable
    {
        public string CuentaCodigo => string.Format("{0} {1}", CuentaContable1, Descripcion);
    }

    public partial class MovimientoIngreso
    {
        public string Recinto { get; set; }
    }

    public partial class DetalleMovimientoIngreso
    {
        public bool canDelete { get; set; } = true;
        public decimal? Debe => Naturaleza == false ? FactorPorcentual : (decimal?)null;
        public decimal? Haber => Naturaleza ? FactorPorcentual : (decimal?)null;
    }

    public partial class Asiento
    {
        public decimal? Debe => Naturaleza == false ? Monto : (decimal?)null;
        public decimal? Haber => Naturaleza ? Monto : (decimal?)null;
        public string Area { get; set; }
    }

    public partial class ReciboPago
    {
        public string r => Serie + " " + IdRecibo + " " + IdReciboPago;
        public string OrdenPago => Recibo?.IdOrdenPago != null ? Recibo.OrdenPago.NoOrdenPago : "";
        public string FechaROC => Recibo?.Fecha.ToString();
        public string porCuenta => Recibo?.Recibimos;
        public string FormaPago1 => FormaPago?.FormaPago1;
        public string Moneda1 => Moneda?.Moneda1;
    }

    public partial class OrdenPago
    {

        public string Identificador_Externo => ObtenerIdentificador();

        private string ObtenerIdentificador()
        {
            if (new ReciboViewModel().isAgenteExterno(IdTipoDeposito))
            {
                return new AgenteExternoViewModel().FindById(int.Parse(Identificador)).Identificacion;
            }
            else
            {
                return Identificador;
            }

        }
    }
}
