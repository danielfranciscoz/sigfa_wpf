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
    
    public partial class ArancelArea
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ArancelArea()
        {
            this.ArancelPrecio = new HashSet<ArancelPrecio>();
            this.AreaPagoDelegado = new HashSet<AreaPagoDelegado>();
            this.UsuarioArancel = new HashSet<UsuarioArancel>();
        }
    
        public int IdArancelArea { get; set; }
        public int IdArancel { get; set; }
        public string IdArea { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public bool regAnulado { get; set; }
        public string IdAreaDestino { get; set; }
    
        public virtual Usuario Usuario { get; set; }
        public virtual Arancel Arancel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArancelPrecio> ArancelPrecio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AreaPagoDelegado> AreaPagoDelegado { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuarioArancel> UsuarioArancel { get; set; }
    }
}
