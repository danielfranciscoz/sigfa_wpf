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
    
    public partial class TmpDisponibilidad
    {
        public int Secuencia { get; set; }
        public byte IdDocumento { get; set; }
        public string Documento { get; set; }
        public int IdComprobante { get; set; }
        public string Serie { get; set; }
        public Nullable<int> IdCuentaContable { get; set; }
        public Nullable<int> IdArea { get; set; }
        public string CuentaContable { get; set; }
        public string Descripcion { get; set; }
        public Nullable<bool> Tipo { get; set; }
        public Nullable<int> Signo { get; set; }
        public Nullable<decimal> MN_Debe { get; set; }
        public Nullable<decimal> MN_Haber { get; set; }
        public Nullable<decimal> ME_Debe { get; set; }
        public Nullable<decimal> ME_Haber { get; set; }
        public Nullable<byte> IdFuente { get; set; }
        public string Fuente { get; set; }
        public Nullable<byte> IdMes { get; set; }
        public string Mes { get; set; }
        public Nullable<int> Periodo { get; set; }
        public Nullable<byte> IdTipoDocumento { get; set; }
        public string strDocumento { get; set; }
        public string LoginCreacion { get; set; }
    }
}
