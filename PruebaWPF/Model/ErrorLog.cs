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
    
    public partial class ErrorLog
    {
        public int ErrorLogID { get; set; }
        public System.DateTime ErrorTime { get; set; }
        public string UserName { get; set; }
        public int ErrorNumber { get; set; }
        public string Carnet { get; set; }
        public Nullable<int> Recibo { get; set; }
        public string Serie { get; set; }
        public Nullable<int> ErrorLine { get; set; }
        public string ErrorMessage { get; set; }
    }
}