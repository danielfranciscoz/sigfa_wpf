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
    
    public partial class fnx_ConciliaDocumentos_PorCuenta_Result
    {
        public int Secuencia { get; set; }
        public Nullable<byte> IdOrigen { get; set; }
        public string Origen { get; set; }
        public Nullable<int> IdCuentaContable { get; set; }
        public string CuentaContable { get; set; }
        public string Descripcion { get; set; }
        public Nullable<int> IdActividad { get; set; }
        public string NombreActividad { get; set; }
        public string Mes { get; set; }
        public string IdYear { get; set; }
        public Nullable<int> IdSolicitudCheque { get; set; }
        public Nullable<byte> TipoDocumento { get; set; }
        public string Codigo_p { get; set; }
        public Nullable<int> Interno_p { get; set; }
        public string Codigo_c { get; set; }
        public Nullable<int> Interno_c { get; set; }
        public Nullable<decimal> Cantidad { get; set; }
        public Nullable<int> IdFuente { get; set; }
        public string Fuente { get; set; }
        public Nullable<bool> Conciliada { get; set; }
    }
}
