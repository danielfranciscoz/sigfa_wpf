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
    
    public partial class ReciboPagoTarjeta
    {
        public int IdReciboPago { get; set; }
        public byte IdTarjeta { get; set; }
        public string Tarjeta { get; set; }
        public int Autorizacion { get; set; }
        public Nullable<int> IdVoucherBanco { get; set; }
    
        public virtual CiaTarjetaCredito CiaTarjetaCredito { get; set; }
        public virtual ReciboPago ReciboPago { get; set; }
        public virtual VoucherBanco VoucherBanco { get; set; }
    }
}
