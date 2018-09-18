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
    
    public partial class w_LibroMayorAcumulado
    {
        public int IdComprobante { get; set; }
        public int IdTipoComprobante { get; set; }
        public string TipoComprobante { get; set; }
        public string Codigo { get; set; }
        public string Dia { get; set; }
        public string Mes { get; set; }
        public string Year { get; set; }
        public decimal TDebe { get; set; }
        public decimal THaber { get; set; }
        public string IdInterno { get; set; }
        public Nullable<int> IdImpresion { get; set; }
        public string NoCheque { get; set; }
        public Nullable<int> IdSolicitudCheque { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Concepto { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public Nullable<short> IdCuentaContable { get; set; }
        public string CuentaSuperior { get; set; }
        public string CuentaContable { get; set; }
        public string Descripcion { get; set; }
        public bool Tipo { get; set; }
        public string Superior { get; set; }
        public string Programa { get; set; }
        public System.DateTime FechaFin { get; set; }
        public Nullable<int> IdOrden { get; set; }
        public decimal DebeInicial { get; set; }
        public decimal HaberInicial { get; set; }
        public string NombreActividad { get; set; }
        public Nullable<byte> IdActividad { get; set; }
        public byte Estado { get; set; }
        public byte IdPeriodoEspecifico { get; set; }
        public string Beneficiario { get; set; }
        public byte IdEstado { get; set; }
        public string EstadoComprobante { get; set; }
        public string Usuario { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public string DiaCr { get; set; }
        public string MesCr { get; set; }
        public string YearCr { get; set; }
        public Nullable<byte> IdTipoCuenta { get; set; }
        public byte IdPrograma { get; set; }
        public string LineaEstado { get; set; }
        public string Fuente { get; set; }
        public Nullable<byte> IdFuenteFinanciamiento { get; set; }
    }
}
