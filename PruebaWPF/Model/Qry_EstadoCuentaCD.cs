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
    
    public partial class Qry_EstadoCuentaCD
    {
        public short IdCuentaContable { get; set; }
        public string CuentaSuperior { get; set; }
        public string CuentaContable { get; set; }
        public string Descripcion { get; set; }
        public Nullable<int> IdImpresion { get; set; }
        public Nullable<int> NoCheque { get; set; }
        public int IdInterno { get; set; }
        public System.DateTime Fecha { get; set; }
        public byte Estado { get; set; }
        public string Concepto { get; set; }
        public Nullable<decimal> Debe { get; set; }
        public Nullable<decimal> Haber { get; set; }
        public bool DebeHaber { get; set; }
        public Nullable<decimal> ParcialCordoba { get; set; }
        public Nullable<byte> IdFuenteFinanciamiento { get; set; }
        public string FuenteFinanciamiento { get; set; }
        public string LoginCreacion { get; set; }
    }
}
