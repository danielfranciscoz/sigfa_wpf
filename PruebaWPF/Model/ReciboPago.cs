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
    
    public partial class ReciboPago
    {
        public int IdReciboPago { get; set; }
        public int IdRecibo { get; set; }
        public string Serie { get; set; }
        public int IdFormaPago { get; set; }
        public decimal Monto { get; set; }
        public int IdMoneda { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public bool regAnulado { get; set; }
    
        public virtual Moneda Moneda { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual FormaPago FormaPago { get; set; }
        public virtual ReciboPagoBono ReciboPagoBono { get; set; }
        public virtual ReciboPagoCheque ReciboPagoCheque { get; set; }
        public virtual Recibo1 Recibo1 { get; set; }
        public virtual ReciboPagoTarjeta ReciboPagoTarjeta { get; set; }
    }
}
