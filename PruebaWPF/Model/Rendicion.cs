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
    
    public partial class Rendicion
    {
        public int IdRendicion { get; set; }
        public int NumCheque { get; set; }
        public string IdYear { get; set; }
        public Nullable<int> IdSolCk { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public string Observaciones { get; set; }
        public Nullable<bool> Estado { get; set; }
        public Nullable<bool> e_aritmenticos { get; set; }
        public Nullable<bool> e_manchones { get; set; }
        public Nullable<bool> e_soportes { get; set; }
        public Nullable<bool> e_conceptos { get; set; }
        public Nullable<bool> e_beneficiarios { get; set; }
        public Nullable<bool> e_autorizado { get; set; }
        public Nullable<bool> e_desglose { get; set; }
        public Nullable<bool> e_cartajustificacion { get; set; }
        public Nullable<bool> e_otrasinconsistencias { get; set; }
        public Nullable<System.DateTime> FechaRecepcion { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string LoginCreacion { get; set; }
        public Nullable<byte> RegAnulado { get; set; }
    }
}