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
    
    public partial class Qry_Transferencia
    {
        public int IdTransferencia { get; set; }
        public byte IdFuenteFinanciamiento { get; set; }
        public string FuenteFinanciamiento { get; set; }
        public int IdCuentaBanco { get; set; }
        public string CuentaContable { get; set; }
        public string strCuenta { get; set; }
        public Nullable<int> IdYear { get; set; }
        public Nullable<int> Mes { get; set; }
        public Nullable<int> Periodo { get; set; }
        public string NoReferencia { get; set; }
        public string Descripcion { get; set; }
        public Nullable<decimal> Monto { get; set; }
        public Nullable<decimal> Reservado { get; set; }
        public Nullable<decimal> Disponible { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public Nullable<byte> IdMoneda { get; set; }
        public Nullable<decimal> TasaCambio { get; set; }
        public Nullable<byte> Estado { get; set; }
        public string LoginCreacion { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string LoginModificacion { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
        public Nullable<byte> RegAnulado { get; set; }
    }
}