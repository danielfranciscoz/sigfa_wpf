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
    
    public partial class ReciboDet
    {
        public int IdReciboDet { get; set; }
        public int IdRecibo { get; set; }
        public string Serie { get; set; }
        public int IdPrecioArancel { get; set; }
        public string Concepto { get; set; }
        public decimal Monto { get; set; }
        public decimal Descuento { get; set; }
        public string UsuarioCreacion { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public bool regAnulado { get; set; }
    
        public virtual Usuario Usuario { get; set; }
        public virtual ArancelPrecio ArancelPrecio { get; set; }
        public virtual Recibo1 Recibo1 { get; set; }
    }
}
