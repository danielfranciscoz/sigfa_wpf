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
    
    public partial class ReciboAnulado
    {
        public int IdRecibo { get; set; }
        public string Serie { get; set; }
        public string Motivo { get; set; }
        public Nullable<int> IdOrdenPago { get; set; }
        public string UsuarioAnulacion { get; set; }
        public System.DateTime FechaAnulacion { get; set; }
    
        public virtual Usuario Usuario { get; set; }
        public virtual Recibo Recibo { get; set; }
        public virtual OrdenPago OrdenPago { get; set; }
    }
}
