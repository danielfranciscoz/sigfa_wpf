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
    
    public partial class fnx_GeneraSolicitudesCheques_Result
    {
        public int Secuencia { get; set; }
        public Nullable<int> IdSolicitudFI { get; set; }
        public Nullable<int> IdYear { get; set; }
        public Nullable<int> IdActividad { get; set; }
        public string Actividad { get; set; }
        public Nullable<int> IdBeneficiario { get; set; }
        public string Beneficiario { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public Nullable<byte> IdMoneda { get; set; }
        public string Moneda { get; set; }
        public Nullable<decimal> TasaCambio { get; set; }
        public Nullable<decimal> Monto { get; set; }
        public string Concepto { get; set; }
        public Nullable<byte> IdEstado { get; set; }
        public string Estado { get; set; }
        public Nullable<byte> IdFuenteFinanciamiento { get; set; }
        public Nullable<int> IdSolicitudSI { get; set; }
        public Nullable<bool> Conciliada { get; set; }
        public string LoginCreacion { get; set; }
    }
}