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
    
    public partial class AccesoDirectoUsuario
    {
        public int IdAccesoDirectoUsuario { get; set; }
        public int IdPantalla { get; set; }
        public string IdUsuario { get; set; }
        public string BackgroundCard { get; set; }
        public System.DateTime FechaCreacion { get; set; }
    
        public virtual Usuario Usuario { get; set; }
        public virtual Pantalla Pantalla { get; set; }
    }
}