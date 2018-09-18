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
    
    public partial class SolicitudCheque
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SolicitudCheque()
        {
            this.AutorizarRestricciones = new HashSet<AutorizarRestricciones>();
            this.DocumentoEnlace = new HashSet<DocumentoEnlace>();
            this.SolicitudEstado = new HashSet<SolicitudEstado>();
        }
    
        public int IdSolicitudCheque { get; set; }
        public string IdYear { get; set; }
        public Nullable<byte> IdActividad { get; set; }
        public int IdBeneficiario { get; set; }
        public Nullable<byte> IdPeriodoEspecifico { get; set; }
        public System.DateTime Fecha { get; set; }
        public Nullable<byte> TipoMoneda { get; set; }
        public decimal TasaCambio { get; set; }
        public decimal Cantidad { get; set; }
        public string Concepto { get; set; }
        public bool Presupuesto { get; set; }
        public byte Estado { get; set; }
        public Nullable<byte> IdFuenteFinanciamiento { get; set; }
        public Nullable<short> IdCuentaContable { get; set; }
        public int Actividad { get; set; }
        public string LoginCreacion { get; set; }
        public Nullable<int> IdSolicitudCheque_SIPPSI { get; set; }
        public bool RegAnulado { get; set; }
    
        public virtual Actividad Actividad1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AutorizarRestricciones> AutorizarRestricciones { get; set; }
        public virtual Beneficiario Beneficiario { get; set; }
        public virtual CuentaContable CuentaContable { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentoEnlace> DocumentoEnlace { get; set; }
        public virtual FuenteFinanciamiento FuenteFinanciamiento { get; set; }
        public virtual PeriodoEspecifico PeriodoEspecifico { get; set; }
        public virtual Usuario Usuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitudEstado> SolicitudEstado { get; set; }
        public virtual SolicitudRegresada SolicitudRegresada { get; set; }
    }
}
