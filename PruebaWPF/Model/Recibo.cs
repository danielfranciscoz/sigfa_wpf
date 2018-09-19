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
    
    public partial class Recibo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Recibo()
        {
            this.ReciboBillete = new HashSet<ReciboBillete>();
            this.ReciboBono = new HashSet<ReciboBono>();
            this.ReciboCheque = new HashSet<ReciboCheque>();
            this.ReciboDetalle = new HashSet<ReciboDetalle>();
            this.ReciboFlujoEfectivo = new HashSet<ReciboFlujoEfectivo>();
            this.ReciboPagare = new HashSet<ReciboPagare>();
            this.ReciboTarjeta = new HashSet<ReciboTarjeta>();
        }
    
        public int IdRecibo { get; set; }
        public string SerieRecibo { get; set; }
        public byte IdPrograma { get; set; }
        public byte IdRecinto { get; set; }
        public byte IdPeriodoEspecifico { get; set; }
        public int IdImpresion { get; set; }
        public System.DateTime Fecha { get; set; }
        public byte TipoMoneda { get; set; }
        public decimal TasaCambio { get; set; }
        public byte IdActividad { get; set; }
        public Nullable<int> IdPrematricula { get; set; }
        public string ALCodigo { get; set; }
        public string EstudianteCarne { get; set; }
        public Nullable<int> IdEmpleado { get; set; }
        public Nullable<int> IdProveedor { get; set; }
        public Nullable<int> IdAgenteExterno { get; set; }
        public string Recibimos { get; set; }
        public Nullable<short> IdCuentaContable { get; set; }
        public string Concepto { get; set; }
        public bool Efectivo { get; set; }
        public bool Cheque { get; set; }
        public bool TarjetaCredito { get; set; }
        public bool Pagare { get; set; }
        public bool Bono { get; set; }
        public decimal Monto { get; set; }
        public byte IdFuenteFinanciamiento { get; set; }
        public Nullable<short> IdConcepto { get; set; }
        public string LoginCreacion { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public bool RegAnulado { get; set; }
    
        public virtual Actividad Actividad { get; set; }
        public virtual Concepto Concepto1 { get; set; }
        public virtual FuenteFinanciamiento FuenteFinanciamiento { get; set; }
        public virtual PeriodoEspecifico PeriodoEspecifico { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboBillete> ReciboBillete { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboBono> ReciboBono { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboCheque> ReciboCheque { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboDetalle> ReciboDetalle { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboFlujoEfectivo> ReciboFlujoEfectivo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboPagare> ReciboPagare { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboTarjeta> ReciboTarjeta { get; set; }
    }
}