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
    
    public partial class EPLineaComprobanteDiario
    {
        public int IdAsiento { get; set; }
        public int IdComprobanteDiario { get; set; }
        public Nullable<short> IdCuentaContable { get; set; }
        public Nullable<int> IdAuxiliar { get; set; }
        public Nullable<byte> IdActividad { get; set; }
        public int IdGrupo { get; set; }
        public short IdOrden { get; set; }
        public bool Agrupa { get; set; }
        public bool DebeHaber { get; set; }
        public Nullable<decimal> ParcialCordoba { get; set; }
        public Nullable<decimal> ParcialDolar { get; set; }
        public Nullable<decimal> Debe { get; set; }
        public Nullable<decimal> Haber { get; set; }
        public bool RegAnulado { get; set; }
    
        public virtual Actividad Actividad { get; set; }
        public virtual CuentaAuxiliar CuentaAuxiliar { get; set; }
        public virtual CuentaContable CuentaContable { get; set; }
        public virtual EPComprobanteDiario EPComprobanteDiario { get; set; }
    }
}
