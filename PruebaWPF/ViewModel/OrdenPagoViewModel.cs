using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Interface;
using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PruebaWPF.ViewModel
{
    class OrdenPagoViewModel : IGestiones<OrdenPagoSon>
    {
        private SIFOPEntities db = new SIFOPEntities();
        private Pantalla pantalla;
        IQueryable<vw_RecintosRH> r;
        private SecurityViewModel seguridad;

        public OrdenPagoViewModel(Pantalla pantalla)
        {
            seguridad = new SecurityViewModel(db);
            r = seguridad.RecintosPermiso(pantalla);

            this.pantalla = pantalla;
        }

        public Pantalla Pantalla(string UId)
        {
            return new PantallaViewModel().FindById(UId);
        }

        public void PagarOrden(int IdOrdenPago)
        {
            throw new NotImplementedException();
            //using (SIFOPEntities m = new SIFOPEntities())
            //{
            //    m.sp_PagarOrdenPIENSA(IdOrdenPago, clsSessionHelper.usuario.Login);
            //}
        }

        public void Eliminar(OrdenPagoSon ordenpago)
        {
            throw new NotImplementedException();
        }

        public List<OrdenPagoSon> FindAll()
        {
            throw new NotImplementedException();
        }

        public List<OrdenPagoSon> FindAll(bool allRecintos)
        {
            return FindAllOrders(allRecintos).ToList();
        }
        private IQueryable<OrdenPagoSon> FindAllOrders(bool allRecintos)
        {

            if (clsSessionHelper.vigenciaOP == 0)
            {
                clsSessionHelper.vigenciaOP = int.Parse(new SharedViewModel().Configuracion(clsConfiguration.Llaves.dias_OP.ToString()).Valor);
            }

            //Recuerda igualar las columnas de este select en el FindBytEXT
            var consulta = db.OrdenPago.Where(w => string.IsNullOrEmpty(w.CodRecibo) && w.regAnulado == false && DbFunctions.DiffDays(w.FechaEnvio, DateTime.Now) <= clsSessionHelper.vigenciaOP).OrderByDescending(o => o.IdOrdenPago).Take(clsConfiguration.Actual().TopRow)
                    .Join(
                    db.vw_Areas,
                    r => r.IdArea,
                    area => area.codigo,
                    (r, area) => new { areaInner = area, r }
                )
                .Join(
                    db.vw_RecintosRH,
                    r => r.r.IdRecinto,
                    recinto => recinto.IdRecinto,
                    (r, recinto) => new { r, recinto }
                )
                    .Select(a => new OrdenPagoSon
                    {
                        IdOrdenPago = a.r.r.IdOrdenPago,
                        NoOrdenPago = a.r.r.NoOrdenPago,
                        IdRecinto = a.r.r.IdRecinto,
                        TextoIdentificador = a.r.r.TextoIdentificador,
                        UsuarioRemitente = a.r.r.UsuarioRemitente,
                        Sistema = a.r.r.Sistema,
                        FechaEnvio = a.r.r.FechaEnvio,
                        regAnulado = a.r.r.regAnulado,
                        CodRecibo = a.r.r.CodRecibo,
                        Identificador = a.r.r.Identificador,
                        IdTipoDeposito = a.r.r.IdTipoDeposito,
                        TipoDeposito = a.r.r.TipoDeposito,
                        IdArea = a.r.r.IdArea,
                        DetOrdenPagoArancel = a.r.r.DetOrdenPagoArancel,
                        CantidadPagos = a.r.r.DetOrdenPagoArancel.Count(),
                        Recinto = a.recinto.Siglas,
                        Area = a.r.areaInner.descripcion.ToUpper()
                    }
            );

            if (allRecintos)
            {
                return consulta;
            }
            else
            {
                return consulta.Where(w => r.Any(a => w.IdRecinto == a.IdRecinto));
            }

        }

        public OrdenPagoSon FindById(int Id)
        {
            return (OrdenPagoSon)db.OrdenPago.Find(Id);
        }

        public List<OrdenPagoSon> FindByText(string text)
        {
            throw new NotImplementedException();
        }

        public List<OrdenPagoSon> FindByText(bool allRecintos, string text)
        {
            if (!text.Equals(""))
            {
                string[] busqueda = text.ToLower().Trim().Split(' ');

                    return FindAllOrders(allRecintos).ToList().Where(
                       w => busqueda.All(a => w.TextoIdentificador.ToLower().Contains(a)) || w.Identificador_Externo.ToLower().Contains(text))
                    .ToList();
                
            }
            else
            {
                return FindAll(allRecintos);
            }
        }

        public List<OrdenPagoSon> FindByTextIgnoreRecinto(string text)
        {
            if (!text.Equals(""))
            {
                string[] busqueda = text.Trim().Split(' ');

                using (SIFOPEntities t = new SIFOPEntities())
                {
                    return FindAll().Where(
                       w => busqueda.All(a => w.TextoIdentificador.Contains(a)))
                    .ToList();
                }
            }
            else
            {
                return FindAll();
            }


        }

        public void Guardar(OrdenPagoSon Obj)
        {
            throw new NotImplementedException();
        }

        public void Modificar(OrdenPagoSon Obj)
        {
            throw new NotImplementedException();
        }

        public bool Authorize_Recinto(string PermisoName, int IdRecinto)
        {
            if (seguridad.Authorize(pantalla, PermisoName, IdRecinto))
            {
                return true;
            }
            else
            {
                throw new AuthorizationException(PermisoName, IdRecinto, db);
            }
        }

        public bool Authorize(string PermisoName)
        {
            if (seguridad.Authorize(pantalla, PermisoName))
            {
                return true;
            }
            else
            {
                throw new AuthorizationException(PermisoName);
            }
        }
    }
    public class OrdenPagoSon : OrdenPago, ICloneable
    {


        public int CantidadPagos { get; set; }
        public int Vence => clsSessionHelper.vigenciaOP - ((TimeSpan)(DateTime.Now-FechaEnvio)).Days;
        public string Area { get; set; }
        public string Recinto { get; set; }

        public string PorCuenta { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
