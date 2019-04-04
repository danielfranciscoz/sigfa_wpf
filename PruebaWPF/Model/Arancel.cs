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
    
    public partial class Arancel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Arancel()
        {
            this.ArancelArea = new HashSet<ArancelArea>();
            this.ArancelPrecio = new HashSet<ArancelPrecio>();
            this.ArancelTipoDeposito = new HashSet<ArancelTipoDeposito>();
        }
    
        public int IdArancel { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdTipoArancel { get; set; }
        public short IdCuentaContable { get; set; }
        public bool isPrecioVariable { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public bool regAnulado { get; set; }
    
        public virtual CuentaContable CuentaContable { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual TipoArancel TipoArancel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArancelArea> ArancelArea { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArancelPrecio> ArancelPrecio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArancelTipoDeposito> ArancelTipoDeposito { get; set; }
    }
}
