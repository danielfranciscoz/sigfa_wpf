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
    
    public partial class FuenteFinanciamiento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FuenteFinanciamiento()
        {
            this.Recibo = new HashSet<Recibo>();
        }
    
        public byte IdFuenteFinanciamiento { get; set; }
        public string Nombre { get; set; }
        public string Siglas { get; set; }
        public bool Tiene_Ingreso { get; set; }
        public bool Tiene_Egreso { get; set; }
        public Nullable<byte> IdFuente_SIPPSI { get; set; }
        public string LoginCreacion { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public bool RegAnulado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Recibo> Recibo { get; set; }
    }
}
