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
    
    public partial class Proyecto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Proyecto()
        {
            this.Actividad = new HashSet<Actividad>();
        }
    
        public byte IdProyecto { get; set; }
        public byte IdPrograma { get; set; }
        public string IdInterfaz { get; set; }
        public string Descripcion { get; set; }
        public string LoginCreacion { get; set; }
        public byte RegAnulado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Actividad> Actividad { get; set; }
        public virtual Programa Programa { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
