//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PruebaWPF.Model
{
    using System;
    
    public partial class fnx_Cheque_Result
    {
        public int IdComprobantePago { get; set; }
        public System.DateTime Fecha { get; set; }
        public Nullable<System.DateTime> FechaImpresion { get; set; }
        public string Concepto { get; set; }
        public Nullable<byte> TipoMoneda { get; set; }
        public string Beneficiario { get; set; }
        public Nullable<decimal> cantidad { get; set; }
        public decimal TasaCambio { get; set; }
        public int IdComprobante { get; set; }
        public string IdInterfaz { get; set; }
        public string Descripcion { get; set; }
        public Nullable<decimal> ParcialCordoba { get; set; }
        public Nullable<decimal> Debe { get; set; }
        public Nullable<decimal> Haber { get; set; }
        public Nullable<int> IdOrden { get; set; }
        public bool Extension { get; set; }
        public string LoginImpresion { get; set; }
        public int IdImpresion { get; set; }
        public decimal TotalDebe { get; set; }
        public decimal TotalHaber { get; set; }
        public string CantidadLetra { get; set; }
    }
}
