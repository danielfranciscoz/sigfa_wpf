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
    
    public partial class Actividad
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Actividad()
        {
            this.EPLineaComprobanteDiario = new HashSet<EPLineaComprobanteDiario>();
            this.EPLineaComprobantePago = new HashSet<EPLineaComprobantePago>();
            this.EstructuraCuenta = new HashSet<EstructuraCuenta>();
            this.LineaComprobanteDiario = new HashSet<LineaComprobanteDiario>();
            this.LineaComprobantePago = new HashSet<LineaComprobantePago>();
            this.Recibo = new HashSet<Recibo>();
            this.SolicitudCheque = new HashSet<SolicitudCheque>();
        }
    
        public byte IdActividad { get; set; }
        public byte IdProyecto { get; set; }
        public string IdInterfaz { get; set; }
        public string Descripcion { get; set; }
        public Nullable<int> ActividadEquivalente { get; set; }
        public Nullable<bool> Ingreso { get; set; }
        public string LoginCreacion { get; set; }
        public byte RegAnulado { get; set; }
    
        public virtual Proyecto Proyecto { get; set; }
        public virtual Usuario Usuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EPLineaComprobanteDiario> EPLineaComprobanteDiario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EPLineaComprobantePago> EPLineaComprobantePago { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EstructuraCuenta> EstructuraCuenta { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LineaComprobanteDiario> LineaComprobanteDiario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LineaComprobantePago> LineaComprobantePago { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Recibo> Recibo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitudCheque> SolicitudCheque { get; set; }
    }
}
