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
    
    public partial class TmpUsuarioPermiso
    {
        public int IdOrden { get; set; }
        public int IDMenu { get; set; }
        public int NoItem { get; set; }
        public int NoFuncion { get; set; }
        public bool LCACtrolTotal { get; set; }
        public bool LCAAcceso { get; set; }
        public bool LCACrear { get; set; }
        public bool LCAModificar { get; set; }
        public bool LCAEliminar { get; set; }
        public bool LCASinPermiso { get; set; }
        public byte RegAnulado { get; set; }
        public byte Tipo { get; set; }
    }
}