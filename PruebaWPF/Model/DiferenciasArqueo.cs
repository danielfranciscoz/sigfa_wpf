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
    
    public partial class DiferenciasArqueo
    {
        public int IdDiferenciasArqueo { get; set; }
        public int IdArqueo { get; set; }
        public int IdFormaPago { get; set; }
        public int IdMoneda { get; set; }
        public decimal Monto { get; set; }
    
        public virtual Arqueo Arqueo { get; set; }
        public virtual FormaPago FormaPago { get; set; }
        public virtual Moneda Moneda { get; set; }
    }
}
