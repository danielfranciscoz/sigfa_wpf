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
    
    public partial class TmpInformeFinanciero
    {
        public short IdCuentaContable { get; set; }
        public Nullable<byte> IdPeriodoInicial { get; set; }
        public Nullable<byte> IdPeriodoFinal { get; set; }
        public Nullable<byte> IdPrograma { get; set; }
        public Nullable<byte> IdProyecto { get; set; }
        public Nullable<byte> IdActividad { get; set; }
        public Nullable<int> IdOrden { get; set; }
        public Nullable<int> IdOrdenGrupo { get; set; }
        public string CuentaSuperior { get; set; }
        public Nullable<int> CuentaGrupo { get; set; }
        public string CuentaContable { get; set; }
        public string Serie { get; set; }
        public string Descripcion { get; set; }
        public Nullable<bool> BalanceResultado { get; set; }
        public Nullable<bool> Acumulativa { get; set; }
        public Nullable<decimal> DebeInicial { get; set; }
        public Nullable<decimal> HaberInicial { get; set; }
        public Nullable<decimal> DebeMovMes { get; set; }
        public Nullable<decimal> HaberMovMes { get; set; }
        public string Login { get; set; }
    }
}
