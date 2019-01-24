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
    
    public partial class PeriodoEspecifico
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PeriodoEspecifico()
        {
            this.BalanzaComprobacion = new HashSet<BalanzaComprobacion>();
            this.ComprobanteDiario = new HashSet<ComprobanteDiario>();
            this.ComprobantePago = new HashSet<ComprobantePago>();
            this.ConciliacionBanco = new HashSet<ConciliacionBanco>();
            this.EjecucionPresupuestaria = new HashSet<EjecucionPresupuestaria>();
            this.EPComprobanteDiario = new HashSet<EPComprobanteDiario>();
            this.Presupuesto = new HashSet<Presupuesto>();
            this.Recibo = new HashSet<Recibo>();
            this.SolicitudCheque = new HashSet<SolicitudCheque>();
            this.Recibo1 = new HashSet<Recibo1>();
        }
    
        public byte IdPeriodoEspecifico { get; set; }
        public byte IdPeriodoGeneral { get; set; }
        public System.DateTime FechaInicio { get; set; }
        public System.DateTime FechaFin { get; set; }
        public string Observacion { get; set; }
        public byte Estado { get; set; }
        public string LoginCreacion { get; set; }
        public bool RegAnulado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BalanzaComprobacion> BalanzaComprobacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComprobanteDiario> ComprobanteDiario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComprobantePago> ComprobantePago { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConciliacionBanco> ConciliacionBanco { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EjecucionPresupuestaria> EjecucionPresupuestaria { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EPComprobanteDiario> EPComprobanteDiario { get; set; }
        public virtual PeriodoGeneral PeriodoGeneral { get; set; }
        public virtual Usuario Usuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Presupuesto> Presupuesto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Recibo> Recibo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitudCheque> SolicitudCheque { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Recibo1> Recibo1 { get; set; }
    }
}
