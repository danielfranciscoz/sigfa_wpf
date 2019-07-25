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
    
    public partial class OrdenPago
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrdenPago()
        {
            this.DetOrdenPagoArancel = new HashSet<DetOrdenPagoArancel>();
            this.Recibo1 = new HashSet<Recibo1>();
            this.ReciboAnulado = new HashSet<ReciboAnulado>();
        }
    
        public int IdOrdenPago { get; set; }
        public string NoOrdenPago { get; set; }
        public string IdArea { get; set; }
        public int IdRecinto { get; set; }
        public int IdTipoDeposito { get; set; }
        public string Identificador { get; set; }
        public string TextoIdentificador { get; set; }
        public string UsuarioRemitente { get; set; }
        public string Sistema { get; set; }
        public System.DateTime FechaEnvio { get; set; }
        public string CodRecibo { get; set; }
        public bool regAnulado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetOrdenPagoArancel> DetOrdenPagoArancel { get; set; }
        public virtual TipoDeposito TipoDeposito { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Recibo1> Recibo1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboAnulado> ReciboAnulado { get; set; }
    }
}
