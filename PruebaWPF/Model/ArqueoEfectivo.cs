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
    
    public partial class ArqueoEfectivo
    {
        public int IdArqueoEfectivo { get; set; }
        public int IdArqueo { get; set; }
        public int IdMoneda { get; set; }
        public double Denominacion { get; set; }
        public byte Cantidad { get; set; }
    
        public virtual Moneda Moneda { get; set; }
        public virtual Arqueo Arqueo { get; set; }
    }
}
