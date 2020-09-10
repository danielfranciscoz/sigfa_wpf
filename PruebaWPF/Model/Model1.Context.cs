﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class SIFOPEntities : DbContext
    {
        public SIFOPEntities()
            : base("name=SIFOPEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Arqueo> Arqueo { get; set; }
        public virtual DbSet<ArqueoDocumento> ArqueoDocumento { get; set; }
        public virtual DbSet<ArqueoEfectivo> ArqueoEfectivo { get; set; }
        public virtual DbSet<ArqueoNoEfectivo> ArqueoNoEfectivo { get; set; }
        public virtual DbSet<ArqueoRecibo> ArqueoRecibo { get; set; }
        public virtual DbSet<DiferenciasArqueo> DiferenciasArqueo { get; set; }
        public virtual DbSet<RectificacionPago> RectificacionPago { get; set; }
        public virtual DbSet<Errors> Errors { get; set; }
        public virtual DbSet<AgenteExternoCat> AgenteExternoCat { get; set; }
        public virtual DbSet<AreaEquivalencia> AreaEquivalencia { get; set; }
        public virtual DbSet<IdentificacionAgenteExterno> IdentificacionAgenteExterno { get; set; }
        public virtual DbSet<DenominacionMoneda> DenominacionMoneda { get; set; }
        public virtual DbSet<Moneda> Moneda { get; set; }
        public virtual DbSet<Asiento> Asiento { get; set; }
        public virtual DbSet<DetalleMovimientoIngreso> DetalleMovimientoIngreso { get; set; }
        public virtual DbSet<MovimientoIngreso> MovimientoIngreso { get; set; }
        public virtual DbSet<Banco> Banco { get; set; }
        public virtual DbSet<CiaTarjetaCredito> CiaTarjetaCredito { get; set; }
        public virtual DbSet<CuentaContable> CuentaContable { get; set; }
        public virtual DbSet<FuenteFinanciamiento> FuenteFinanciamiento { get; set; }
        public virtual DbSet<Perfil> Perfil { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<UsuarioPerfil> UsuarioPerfil { get; set; }
        public virtual DbSet<VariacionCambiaria> VariacionCambiaria { get; set; }
        public virtual DbSet<Arancel> Arancel { get; set; }
        public virtual DbSet<ArancelArea> ArancelArea { get; set; }
        public virtual DbSet<ArancelPrecio> ArancelPrecio { get; set; }
        public virtual DbSet<ArancelTipoDeposito> ArancelTipoDeposito { get; set; }
        public virtual DbSet<AreaPagoDelegado> AreaPagoDelegado { get; set; }
        public virtual DbSet<DetOrdenPagoArancel> DetOrdenPagoArancel { get; set; }
        public virtual DbSet<Exoneracion> Exoneracion { get; set; }
        public virtual DbSet<ExoneracionTotalArea> ExoneracionTotalArea { get; set; }
        public virtual DbSet<TipoArancel> TipoArancel { get; set; }
        public virtual DbSet<UsuarioArancel> UsuarioArancel { get; set; }
        public virtual DbSet<AperturaCaja> AperturaCaja { get; set; }
        public virtual DbSet<Caja> Caja { get; set; }
        public virtual DbSet<DetAperturaCaja> DetAperturaCaja { get; set; }
        public virtual DbSet<FormaPago> FormaPago { get; set; }
        public virtual DbSet<InfoRecibo> InfoRecibo { get; set; }
        public virtual DbSet<Recibo> Recibo { get; set; }
        public virtual DbSet<ReciboAnulado> ReciboAnulado { get; set; }
        public virtual DbSet<ReciboDatos> ReciboDatos { get; set; }
        public virtual DbSet<ReciboDet> ReciboDet { get; set; }
        public virtual DbSet<ReciboDiferencias> ReciboDiferencias { get; set; }
        public virtual DbSet<ReciboPago> ReciboPago { get; set; }
        public virtual DbSet<ReciboPagoBono> ReciboPagoBono { get; set; }
        public virtual DbSet<ReciboPagoDeposito> ReciboPagoDeposito { get; set; }
        public virtual DbSet<ReciboPagoTarjeta> ReciboPagoTarjeta { get; set; }
        public virtual DbSet<ReciboSIRA> ReciboSIRA { get; set; }
        public virtual DbSet<SerieRecibo> SerieRecibo { get; set; }
        public virtual DbSet<TipoDeposito> TipoDeposito { get; set; }
        public virtual DbSet<VoucherBanco> VoucherBanco { get; set; }
        public virtual DbSet<AccesoDirectoPerfil> AccesoDirectoPerfil { get; set; }
        public virtual DbSet<AccesoDirectoUsuario> AccesoDirectoUsuario { get; set; }
        public virtual DbSet<Configuracion> Configuracion { get; set; }
        public virtual DbSet<Pantalla> Pantalla { get; set; }
        public virtual DbSet<Permiso> Permiso { get; set; }
        public virtual DbSet<PermisoName> PermisoName { get; set; }
        public virtual DbSet<RecintoEquivalencia> RecintoEquivalencia { get; set; }
        public virtual DbSet<FormulaConversion> FormulaConversion { get; set; }
        public virtual DbSet<Parametro> Parametro { get; set; }
        public virtual DbSet<vw_Aranceles> vw_Aranceles { get; set; }
        public virtual DbSet<vw_AreaEquivalencia> vw_AreaEquivalencia { get; set; }
        public virtual DbSet<vw_Areas> vw_Areas { get; set; }
        public virtual DbSet<vw_FuentesSIPPSI> vw_FuentesSIPPSI { get; set; }
        public virtual DbSet<vw_Prematricula> vw_Prematricula { get; set; }
        public virtual DbSet<vw_ProveedoresSISCOM> vw_ProveedoresSISCOM { get; set; }
        public virtual DbSet<vw_RecintosRH> vw_RecintosRH { get; set; }
        public virtual DbSet<vwCarrerasSIRA> vwCarrerasSIRA { get; set; }
        public virtual DbSet<vwEmpleadosRH> vwEmpleadosRH { get; set; }
        public virtual DbSet<vwEstudiantesSIRA> vwEstudiantesSIRA { get; set; }
        public virtual DbSet<vwSedesSIRA> vwSedesSIRA { get; set; }
        public virtual DbSet<TipoCuenta> TipoCuenta { get; set; }
        public virtual DbSet<Programa> Programa { get; set; }
        public virtual DbSet<vw_ObtenerPeriodosEspecificos> vw_ObtenerPeriodosEspecificos { get; set; }
        public virtual DbSet<OrdenPago> OrdenPago { get; set; }
        public virtual DbSet<ReciboPagoCheque> ReciboPagoCheque { get; set; }
    
        [DbFunction("SIFOPEntities", "fn_TotalesArqueo")]
        public virtual IQueryable<fn_TotalesArqueo_Result> fn_TotalesArqueo(Nullable<int> idArqueo, Nullable<bool> isDoc)
        {
            var idArqueoParameter = idArqueo.HasValue ?
                new ObjectParameter("IdArqueo", idArqueo) :
                new ObjectParameter("IdArqueo", typeof(int));
    
            var isDocParameter = isDoc.HasValue ?
                new ObjectParameter("isDoc", isDoc) :
                new ObjectParameter("isDoc", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_TotalesArqueo_Result>("[SIFOPEntities].[fn_TotalesArqueo](@IdArqueo, @isDoc)", idArqueoParameter, isDocParameter);
        }
    
        [DbFunction("SIFOPEntities", "fn_ConsultarInfoExterna")]
        public virtual IQueryable<fn_ConsultarInfoExterna_Result> fn_ConsultarInfoExterna(Nullable<int> tipoDeposito, string criterio, Nullable<bool> criterioInterno, string texto, Nullable<int> top, Nullable<bool> useLike, Nullable<bool> isReingreso)
        {
            var tipoDepositoParameter = tipoDeposito.HasValue ?
                new ObjectParameter("TipoDeposito", tipoDeposito) :
                new ObjectParameter("TipoDeposito", typeof(int));
    
            var criterioParameter = criterio != null ?
                new ObjectParameter("Criterio", criterio) :
                new ObjectParameter("Criterio", typeof(string));
    
            var criterioInternoParameter = criterioInterno.HasValue ?
                new ObjectParameter("CriterioInterno", criterioInterno) :
                new ObjectParameter("CriterioInterno", typeof(bool));
    
            var textoParameter = texto != null ?
                new ObjectParameter("Texto", texto) :
                new ObjectParameter("Texto", typeof(string));
    
            var topParameter = top.HasValue ?
                new ObjectParameter("Top", top) :
                new ObjectParameter("Top", typeof(int));
    
            var useLikeParameter = useLike.HasValue ?
                new ObjectParameter("UseLike", useLike) :
                new ObjectParameter("UseLike", typeof(bool));
    
            var isReingresoParameter = isReingreso.HasValue ?
                new ObjectParameter("isReingreso", isReingreso) :
                new ObjectParameter("isReingreso", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_ConsultarInfoExterna_Result>("[SIFOPEntities].[fn_ConsultarInfoExterna](@TipoDeposito, @Criterio, @CriterioInterno, @Texto, @Top, @UseLike, @isReingreso)", tipoDepositoParameter, criterioParameter, criterioInternoParameter, textoParameter, topParameter, useLikeParameter, isReingresoParameter);
        }
    
        [DbFunction("SIFOPEntities", "fn_ObtenerTasaCambio")]
        public virtual IQueryable<fn_ObtenerTasaCambio_Result> fn_ObtenerTasaCambio(Nullable<System.DateTime> fecha, Nullable<int> idMoneda)
        {
            var fechaParameter = fecha.HasValue ?
                new ObjectParameter("Fecha", fecha) :
                new ObjectParameter("Fecha", typeof(System.DateTime));
    
            var idMonedaParameter = idMoneda.HasValue ?
                new ObjectParameter("IdMoneda", idMoneda) :
                new ObjectParameter("IdMoneda", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_ObtenerTasaCambio_Result>("[SIFOPEntities].[fn_ObtenerTasaCambio](@Fecha, @IdMoneda)", fechaParameter, idMonedaParameter);
        }
    
        [DbFunction("SIFOPEntities", "fn_ObtenerMenu")]
        public virtual IQueryable<fn_ObtenerMenu_Result> fn_ObtenerMenu(string user, Nullable<bool> isWeb)
        {
            var userParameter = user != null ?
                new ObjectParameter("user", user) :
                new ObjectParameter("user", typeof(string));
    
            var isWebParameter = isWeb.HasValue ?
                new ObjectParameter("isWeb", isWeb) :
                new ObjectParameter("isWeb", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_ObtenerMenu_Result>("[SIFOPEntities].[fn_ObtenerMenu](@user, @isWeb)", userParameter, isWebParameter);
        }
    
        [DbFunction("SIFOPEntities", "LoginInfo")]
        public virtual IQueryable<LoginInfo_Result> LoginInfo(string usuario)
        {
            var usuarioParameter = usuario != null ?
                new ObjectParameter("usuario", usuario) :
                new ObjectParameter("usuario", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<LoginInfo_Result>("[SIFOPEntities].[LoginInfo](@usuario)", usuarioParameter);
        }
    
        public virtual int sp_ArancelesSIRA(string carnet, Nullable<bool> isMatricula)
        {
            var carnetParameter = carnet != null ?
                new ObjectParameter("carnet", carnet) :
                new ObjectParameter("carnet", typeof(string));
    
            var isMatriculaParameter = isMatricula.HasValue ?
                new ObjectParameter("isMatricula", isMatricula) :
                new ObjectParameter("isMatricula", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_ArancelesSIRA", carnetParameter, isMatriculaParameter);
        }
    
        public virtual int sp_InsertarPagoSIRA(string carnet, string detalleRecibo, Nullable<bool> isMatricula)
        {
            var carnetParameter = carnet != null ?
                new ObjectParameter("carnet", carnet) :
                new ObjectParameter("carnet", typeof(string));
    
            var detalleReciboParameter = detalleRecibo != null ?
                new ObjectParameter("detalleRecibo", detalleRecibo) :
                new ObjectParameter("detalleRecibo", typeof(string));
    
            var isMatriculaParameter = isMatricula.HasValue ?
                new ObjectParameter("isMatricula", isMatricula) :
                new ObjectParameter("isMatricula", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_InsertarPagoSIRA", carnetParameter, detalleReciboParameter, isMatriculaParameter);
        }
    
        public virtual int sp_RevertirPagoSIRA(string carnet, Nullable<int> idRecibo, string serie, string motivo, Nullable<bool> isMatricula)
        {
            var carnetParameter = carnet != null ?
                new ObjectParameter("carnet", carnet) :
                new ObjectParameter("carnet", typeof(string));
    
            var idReciboParameter = idRecibo.HasValue ?
                new ObjectParameter("IdRecibo", idRecibo) :
                new ObjectParameter("IdRecibo", typeof(int));
    
            var serieParameter = serie != null ?
                new ObjectParameter("Serie", serie) :
                new ObjectParameter("Serie", typeof(string));
    
            var motivoParameter = motivo != null ?
                new ObjectParameter("Motivo", motivo) :
                new ObjectParameter("Motivo", typeof(string));
    
            var isMatriculaParameter = isMatricula.HasValue ?
                new ObjectParameter("isMatricula", isMatricula) :
                new ObjectParameter("isMatricula", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_RevertirPagoSIRA", carnetParameter, idReciboParameter, serieParameter, motivoParameter, isMatriculaParameter);
        }
    
        public virtual ObjectResult<Nullable<double>> sp_ConvertirDivisas(Nullable<int> idMonedaConvertir, Nullable<int> idMonedaFinal, Nullable<double> monto, Nullable<System.DateTime> fecha)
        {
            var idMonedaConvertirParameter = idMonedaConvertir.HasValue ?
                new ObjectParameter("IdMonedaConvertir", idMonedaConvertir) :
                new ObjectParameter("IdMonedaConvertir", typeof(int));
    
            var idMonedaFinalParameter = idMonedaFinal.HasValue ?
                new ObjectParameter("IdMonedaFinal", idMonedaFinal) :
                new ObjectParameter("IdMonedaFinal", typeof(int));
    
            var montoParameter = monto.HasValue ?
                new ObjectParameter("Monto", monto) :
                new ObjectParameter("Monto", typeof(double));
    
            var fechaParameter = fecha.HasValue ?
                new ObjectParameter("Fecha", fecha) :
                new ObjectParameter("Fecha", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<double>>("sp_ConvertirDivisas", idMonedaConvertirParameter, idMonedaFinalParameter, montoParameter, fechaParameter);
        }
    
        public virtual int fn_ObtenerInfoUsuario(string noInterno)
        {
            var noInternoParameter = noInterno != null ?
                new ObjectParameter("NoInterno", noInterno) :
                new ObjectParameter("NoInterno", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("fn_ObtenerInfoUsuario", noInternoParameter);
        }
    }
}
