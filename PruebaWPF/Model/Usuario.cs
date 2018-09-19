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
    
    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            this.Actividad = new HashSet<Actividad>();
            this.AutorizarRestricciones = new HashSet<AutorizarRestricciones>();
            this.BalanzaComprobacion = new HashSet<BalanzaComprobacion>();
            this.Banco = new HashSet<Banco>();
            this.Carrera = new HashSet<Carrera>();
            this.CarreraTipo = new HashSet<CarreraTipo>();
            this.CiaTarjetaCredito = new HashSet<CiaTarjetaCredito>();
            this.CuentaAuxiliar = new HashSet<CuentaAuxiliar>();
            this.CuentaContable = new HashSet<CuentaContable>();
            this.CuentaContable1 = new HashSet<CuentaContable>();
            this.Departamento = new HashSet<Departamento>();
            this.EjecucionPresupuestaria = new HashSet<EjecucionPresupuestaria>();
            this.EstructuraCuenta = new HashSet<EstructuraCuenta>();
            this.EstudianteCarrera = new HashSet<EstudianteCarrera>();
            this.Facultad = new HashSet<Facultad>();
            this.Municipio = new HashSet<Municipio>();
            this.PeriodoEspecifico = new HashSet<PeriodoEspecifico>();
            this.PeriodoGeneral = new HashSet<PeriodoGeneral>();
            this.Proveedor = new HashSet<Proveedor>();
            this.Proyecto = new HashSet<Proyecto>();
            this.ReciboBillete = new HashSet<ReciboBillete>();
            this.ReciboBono = new HashSet<ReciboBono>();
            this.ReciboCheque = new HashSet<ReciboCheque>();
            this.ReciboFlujoEfectivo = new HashSet<ReciboFlujoEfectivo>();
            this.ReciboPagare = new HashSet<ReciboPagare>();
            this.ReciboTarjeta = new HashSet<ReciboTarjeta>();
            this.Recinto = new HashSet<Recinto>();
            this.SolicitudCheque = new HashSet<SolicitudCheque>();
            this.TipoCuenta = new HashSet<TipoCuenta>();
            this.AccesoDirectoPerfil = new HashSet<AccesoDirectoPerfil>();
            this.AccesoDirectoUsuario = new HashSet<AccesoDirectoUsuario>();
            this.Arancel = new HashSet<Arancel>();
            this.ArancelArea = new HashSet<ArancelArea>();
            this.ArancelPrecio = new HashSet<ArancelPrecio>();
            this.ArancelTipoDeposito = new HashSet<ArancelTipoDeposito>();
            this.Caja = new HashSet<Caja>();
            this.Configuracion = new HashSet<Configuracion>();
            this.DetOrdenPagoArancel = new HashSet<DetOrdenPagoArancel>();
            this.FormaPago = new HashSet<FormaPago>();
            this.InfoRecibo = new HashSet<InfoRecibo>();
            this.Permiso = new HashSet<Permiso>();
            this.Recibo1 = new HashSet<Recibo1>();
            this.ReciboDet = new HashSet<ReciboDet>();
            this.ReciboPago = new HashSet<ReciboPago>();
            this.TipoDeposito = new HashSet<TipoDeposito>();
            this.UsuarioAutorizar = new HashSet<UsuarioAutorizar>();
            this.UsuarioPerfil = new HashSet<UsuarioPerfil>();
            this.UsuarioPrograma = new HashSet<UsuarioPrograma>();
            this.VariacionCambiaria = new HashSet<VariacionCambiaria>();
            this.ReciboAnulado = new HashSet<ReciboAnulado>();
        }
    
        public string Login { get; set; }
        public Nullable<int> noInterno { get; set; }
        public string LoginEmail { get; set; }
        public int IdOrden { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Auditar { get; set; }
        public bool Habilitado { get; set; }
        public bool Activo { get; set; }
        public bool PeriodoVigente { get; set; }
        public bool PeriodoNoVigente { get; set; }
        public bool PeriodoCerrado { get; set; }
        public bool DatoAdministrativo { get; set; }
        public bool DatoOperativo { get; set; }
        public bool DatoConfiguracion { get; set; }
        public bool TodoPrograma { get; set; }
        public bool CambiarPassword { get; set; }
        public Nullable<byte> IdProyecto { get; set; }
        public Nullable<byte> IdActividad { get; set; }
        public string LoginCreacion { get; set; }
        public bool RegAnulado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Actividad> Actividad { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AutorizarRestricciones> AutorizarRestricciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BalanzaComprobacion> BalanzaComprobacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Banco> Banco { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Carrera> Carrera { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CarreraTipo> CarreraTipo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CiaTarjetaCredito> CiaTarjetaCredito { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CuentaAuxiliar> CuentaAuxiliar { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CuentaContable> CuentaContable { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CuentaContable> CuentaContable1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Departamento> Departamento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EjecucionPresupuestaria> EjecucionPresupuestaria { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EstructuraCuenta> EstructuraCuenta { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EstudianteCarrera> EstudianteCarrera { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Facultad> Facultad { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Municipio> Municipio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PeriodoEspecifico> PeriodoEspecifico { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PeriodoGeneral> PeriodoGeneral { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Proveedor> Proveedor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Proyecto> Proyecto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboBillete> ReciboBillete { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboBono> ReciboBono { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboCheque> ReciboCheque { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboFlujoEfectivo> ReciboFlujoEfectivo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboPagare> ReciboPagare { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboTarjeta> ReciboTarjeta { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Recinto> Recinto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitudCheque> SolicitudCheque { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TipoCuenta> TipoCuenta { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccesoDirectoPerfil> AccesoDirectoPerfil { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccesoDirectoUsuario> AccesoDirectoUsuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Arancel> Arancel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArancelArea> ArancelArea { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArancelPrecio> ArancelPrecio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArancelTipoDeposito> ArancelTipoDeposito { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Caja> Caja { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Configuracion> Configuracion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetOrdenPagoArancel> DetOrdenPagoArancel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormaPago> FormaPago { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InfoRecibo> InfoRecibo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Permiso> Permiso { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Recibo1> Recibo1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboDet> ReciboDet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboPago> ReciboPago { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TipoDeposito> TipoDeposito { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuarioAutorizar> UsuarioAutorizar { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuarioPerfil> UsuarioPerfil { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuarioPrograma> UsuarioPrograma { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VariacionCambiaria> VariacionCambiaria { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReciboAnulado> ReciboAnulado { get; set; }
    }
}