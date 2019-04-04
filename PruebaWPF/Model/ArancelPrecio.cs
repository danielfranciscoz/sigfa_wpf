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
    
    public partial class ArancelPrecio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ArancelPrecio()
        {
            this.DetOrdenPagoArancel = new HashSet<DetOrdenPagoArancel>();
            this.ReciboDet = new HashSet<ReciboDet>();
            this.Exoneracion = new HashSet<Exoneracion>();
        }
    
        public int IdArancelPrecio { get; set; }
        public int IdArancel { get; set; }
        public decimal Precio { get; set; }
        public int IdMoneda { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public bool regAnulado { get; set; }
    
        public virtual Moneda Moneda { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Arancel Arancel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetOrdenPagoArancel> DetOrdenPagoArancel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboDet> ReciboDet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Exoneracion> Exoneracion { get; set; }
    }
}
