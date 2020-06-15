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
    
    public partial class Moneda
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Moneda()
        {
            this.ArqueoEfectivo = new HashSet<ArqueoEfectivo>();
            this.ArqueoNoEfectivo = new HashSet<ArqueoNoEfectivo>();
            this.DiferenciasArqueo = new HashSet<DiferenciasArqueo>();
            this.DenominacionMoneda = new HashSet<DenominacionMoneda>();
            this.ArancelPrecio = new HashSet<ArancelPrecio>();
            this.VoucherBanco = new HashSet<VoucherBanco>();
            this.MovimientoIngreso = new HashSet<MovimientoIngreso>();
            this.ReciboDiferencias = new HashSet<ReciboDiferencias>();
            this.ReciboPago = new HashSet<ReciboPago>();
            this.VariacionCambiaria = new HashSet<VariacionCambiaria>();
        }
    
        public int IdMoneda { get; set; }
        public string Moneda1 { get; set; }
        public string Simbolo { get; set; }
        public string CodigoISO { get; set; }
        public bool WebService { get; set; }
        public string UsuarioCreacion { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public bool regAnulado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArqueoEfectivo> ArqueoEfectivo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArqueoNoEfectivo> ArqueoNoEfectivo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DiferenciasArqueo> DiferenciasArqueo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DenominacionMoneda> DenominacionMoneda { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArancelPrecio> ArancelPrecio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VoucherBanco> VoucherBanco { get; set; }
        public virtual Usuario Usuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MovimientoIngreso> MovimientoIngreso { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboDiferencias> ReciboDiferencias { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboPago> ReciboPago { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VariacionCambiaria> VariacionCambiaria { get; set; }
    }
}
