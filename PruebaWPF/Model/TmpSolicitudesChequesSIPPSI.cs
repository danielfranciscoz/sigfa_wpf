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
    using System.Collections.Generic;
    
    public partial class TmpSolicitudesChequesSIPPSI
    {
        public long IDSecuencia { get; set; }
        public long IdSolicitudCheque { get; set; }
        public Nullable<int> IdActividadDetalle { get; set; }
        public string DescripcionActividad { get; set; }
        public string ConceptoSolCK { get; set; }
        public Nullable<int> TipoBeneficiario { get; set; }
        public string CodigoBeneficiario { get; set; }
        public string Beneficiario { get; set; }
        public Nullable<int> IdEstadoSolicitudCheque { get; set; }
        public string Estado { get; set; }
        public Nullable<int> IdAreaSolicitante { get; set; }
        public string NombreAreaSolicitante { get; set; }
        public Nullable<int> IdFuenteFinanciamiento { get; set; }
        public string DescripcionFuente { get; set; }
        public Nullable<decimal> MontoTotal { get; set; }
        public Nullable<byte> RegAnuladoAreaGasto { get; set; }
        public Nullable<byte> RegAnuladoSolCkDetalle { get; set; }
        public Nullable<int> IdAreaAsumeGasto { get; set; }
        public string NombreAreaAsumeGasto { get; set; }
        public string CodigoOG { get; set; }
        public string ObjetoGasto { get; set; }
        public Nullable<decimal> MontoAreaAsumeGasto { get; set; }
        public string CodigoContable { get; set; }
        public string NumContrato { get; set; }
        public string NumCheque { get; set; }
        public Nullable<System.DateTime> FechaAprobado { get; set; }
        public string Anio { get; set; }
    }
}