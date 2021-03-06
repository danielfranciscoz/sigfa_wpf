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
    
    public partial class Arqueo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Arqueo()
        {
            this.ArqueoDocumento = new HashSet<ArqueoDocumento>();
            this.ArqueoEfectivo = new HashSet<ArqueoEfectivo>();
            this.ArqueoRecibo = new HashSet<ArqueoRecibo>();
            this.DiferenciasArqueo = new HashSet<DiferenciasArqueo>();
        }
    
        public int IdArqueoDetApertura { get; set; }
        public System.DateTime FechaInicioArqueo { get; set; }
        public Nullable<System.DateTime> FechaFinArqueo { get; set; }
        public string UsuarioArqueador { get; set; }
        public string Observaciones { get; set; }
        public string CajeroEntrega { get; set; }
        public bool isFinalizado { get; set; }
    
        public virtual DetAperturaCaja DetAperturaCaja { get; set; }
        public virtual Usuario Usuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArqueoDocumento> ArqueoDocumento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArqueoEfectivo> ArqueoEfectivo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArqueoRecibo> ArqueoRecibo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DiferenciasArqueo> DiferenciasArqueo { get; set; }
    }
}
