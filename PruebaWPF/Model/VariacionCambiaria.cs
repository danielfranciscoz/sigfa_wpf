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
    
    public partial class VariacionCambiaria
    {
        public int IdVariacionCambiaria { get; set; }
        public int IdMoneda { get; set; }
        public System.DateTime Fecha { get; set; }
        public decimal Valor { get; set; }
        public string LoginCreacion { get; set; }
        public bool RegAnulado { get; set; }
    
        public virtual Moneda Moneda { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}