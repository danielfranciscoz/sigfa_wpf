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
    
    public partial class Asiento
    {
        public int IdAsiento { get; set; }
        public int IdRecibo { get; set; }
        public string Serie { get; set; }
        public string IdArea { get; set; }
        public short IdCuentaContable { get; set; }
        public bool Naturaleza { get; set; }
        public decimal Monto { get; set; }
        public bool regAnulado { get; set; }
    
        public virtual CuentaContable CuentaContable { get; set; }
        public virtual Recibo1 Recibo1 { get; set; }
    }
}
