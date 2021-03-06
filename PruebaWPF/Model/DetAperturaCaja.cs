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
    
    public partial class DetAperturaCaja
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DetAperturaCaja()
        {
            this.VoucherBanco = new HashSet<VoucherBanco>();
            this.Recibo = new HashSet<Recibo>();
        }
    
        public int IdDetAperturaCaja { get; set; }
        public int IdAperturaCaja { get; set; }
        public int IdCaja { get; set; }
        public string UsuarioCierre { get; set; }
        public Nullable<System.DateTime> FechaCierre { get; set; }
    
        public virtual Arqueo Arqueo { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual AperturaCaja AperturaCaja { get; set; }
        public virtual Caja Caja { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VoucherBanco> VoucherBanco { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Recibo> Recibo { get; set; }
    }
}
