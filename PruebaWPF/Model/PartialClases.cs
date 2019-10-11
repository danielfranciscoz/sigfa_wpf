using PruebaWPF.Referencias;

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
    }
}
