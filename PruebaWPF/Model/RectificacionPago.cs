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
    
    public partial class RectificacionPago
    {
        public int IdRectificacionPago { get; set; }
        public int IdReciboPago { get; set; }
        public string Observacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public System.DateTime FechaCreacion { get; set; }
    
        public virtual ReciboPago ReciboPago { get; set; }
        public virtual ReciboPago ReciboPago1 { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
