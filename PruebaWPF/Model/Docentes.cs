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
    
    public partial class Docentes
    {
        public string DocenteCodigo { get; set; }
        public int PersonaCodigo { get; set; }
        public short DocenteTipoCodigo { get; set; }
        public string FacultadCodigo { get; set; }
        public string DocenteNumRuc { get; set; }
        public string DocenteObservaciones { get; set; }
        public short VersionNum { get; set; }
        public int UsuarioCreacion { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public int UsuarioUltModif { get; set; }
        public System.DateTime FechaUltModif { get; set; }
        public bool RegAnulado { get; set; }
        public string RegAnuladoObserv { get; set; }
    
        public virtual Personas Personas { get; set; }
    }
}