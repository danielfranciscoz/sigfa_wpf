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
    
    public partial class Beneficiario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Beneficiario()
        {
            this.SolicitudCheque = new HashSet<SolicitudCheque>();
        }
    
        public int IdBeneficiario { get; set; }
        public Nullable<int> IdEmpleado { get; set; }
        public Nullable<int> IdProveedor { get; set; }
        public Nullable<int> PersonaCodigo { get; set; }
        public string LoginCreacion { get; set; }
        public Nullable<bool> RegAnulado { get; set; }
    
        public virtual Empleado Empleado { get; set; }
        public virtual Personas Personas { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitudCheque> SolicitudCheque { get; set; }
    }
}
