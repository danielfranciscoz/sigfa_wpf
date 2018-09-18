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
    
    public partial class Empleado
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Empleado()
        {
            this.Beneficiario = new HashSet<Beneficiario>();
            this.Proveedor = new HashSet<Proveedor>();
        }
    
        public int IdEmpleado { get; set; }
        public Nullable<int> IdEmpleado_RRHH { get; set; }
        public Nullable<int> PersonaCodigo { get; set; }
        public Nullable<short> IdCuentaContable { get; set; }
        public Nullable<short> IdCuentaContableAnticipo { get; set; }
        public string Ubicacion { get; set; }
        public string Cargo { get; set; }
        public string Extension { get; set; }
        public decimal Saldo { get; set; }
        public decimal SaldoAnticipo { get; set; }
        public string LoginCreacion { get; set; }
        public bool RegAnulado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Beneficiario> Beneficiario { get; set; }
        public virtual CuentaContable CuentaContable { get; set; }
        public virtual Personas Personas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Proveedor> Proveedor { get; set; }
    }
}
