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
    
    public partial class ReciboPagoBono
    {
        public int IdReciboPago { get; set; }
        public string Numero { get; set; }
        public string Emisor { get; set; }
    
        public virtual ReciboPago ReciboPago { get; set; }
    }
}
