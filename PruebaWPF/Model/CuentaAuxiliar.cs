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
    
    public partial class CuentaAuxiliar
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CuentaAuxiliar()
        {
            this.EPLineaComprobanteDiario = new HashSet<EPLineaComprobanteDiario>();
            this.EPLineaComprobantePago = new HashSet<EPLineaComprobantePago>();
            this.LineaComprobanteDiario = new HashSet<LineaComprobanteDiario>();
            this.LineaComprobantePago = new HashSet<LineaComprobantePago>();
        }
    
        public int IdAuxiliar { get; set; }
        public short IdCuentaContable { get; set; }
        public string CuentaContable { get; set; }
        public string Descripcion { get; set; }
        public string LoginCreacion { get; set; }
        public bool RegAnulado { get; set; }
    
        public virtual CuentaContable CuentaContable1 { get; set; }
        public virtual Usuario Usuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EPLineaComprobanteDiario> EPLineaComprobanteDiario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EPLineaComprobantePago> EPLineaComprobantePago { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LineaComprobanteDiario> LineaComprobanteDiario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LineaComprobantePago> LineaComprobantePago { get; set; }
    }
}
