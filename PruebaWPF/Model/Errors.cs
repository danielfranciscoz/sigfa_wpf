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
    
    public partial class Errors
    {
        public int IdError { get; set; }
        public string Message { get; set; }
        public string InnerMessage { get; set; }
        public string EntityValidationErrors { get; set; }
        public string Source { get; set; }
        public string Metodo { get; set; }
        public string StackTrace { get; set; }
        public string Computadora { get; set; }
        public string Sistema { get; set; }
        public string Usuario { get; set; }
        public System.DateTime FechaCreacion { get; set; }
    
        public virtual Usuario Usuario1 { get; set; }
    }
}
