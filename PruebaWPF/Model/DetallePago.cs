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
    
    public partial class DetallePago
    {
        public int IdDetallePago { get; set; }
        public int IdPago { get; set; }
        public int IdArancelPrecio { get; set; }
        public int IdMoneda { get; set; }
        public decimal Precio { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public Nullable<System.DateTime> FechaAnulacion { get; set; }
    
        public virtual Moneda Moneda { get; set; }
        public virtual ArancelPrecio ArancelPrecio { get; set; }
        public virtual Pago Pago { get; set; }
    }
}