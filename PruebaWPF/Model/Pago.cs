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
    
    public partial class Pago
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pago()
        {
            this.AnulacionPago = new HashSet<AnulacionPago>();
            this.DetallePago = new HashSet<DetallePago>();
        }
    
        public int IdPago { get; set; }
        public string Identificador { get; set; }
        public string TipoDeposito { get; set; }
        public long NoTrx { get; set; }
        public byte IdBanco { get; set; }
        public string Sucursal { get; set; }
        public string Cajero { get; set; }
        public string PorCuenta { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public Nullable<System.DateTime> FechaAnulacion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AnulacionPago> AnulacionPago { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetallePago> DetallePago { get; set; }
        public virtual Banco Banco { get; set; }
    }
}