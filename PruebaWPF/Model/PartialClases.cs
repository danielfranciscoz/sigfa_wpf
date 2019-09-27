namespace PruebaWPF.Model
{
    class PartialClases
    {
    }

    public partial class Recibo1
    {
        public string NoOrdenPago => IdOrdenPago == null ? (regAnulado?(ReciboAnulado.IdOrdenPago==null?"":ReciboAnulado.OrdenPago.NoOrdenPago):"") : OrdenPago.NoOrdenPago;
    }
}
