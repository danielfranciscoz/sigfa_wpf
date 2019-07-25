using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Interface;
using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PruebaWPF.ViewModel
{
    class OrdenPagoViewModel : IGestiones<OrdenPagoSon>
    {
        private SIFOPEntities db = new SIFOPEntities();
        private Pantalla pantalla;
        List<vw_RecintosRH> r;
        private SecurityViewModel seguridad;

        public OrdenPagoViewModel(Pantalla pantalla)
        {
            seguridad = new SecurityViewModel();
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
            //Recuerda igualar las columnas de este select en el FindBytEXT
            using (SIFOPEntities t = new SIFOPEntities())
            {
            return t.OrdenPago.Where(w => string.IsNullOrEmpty(w.CodRecibo) && w.regAnulado == false).OrderByDescending(o => o.IdOrdenPago).Take(clsConfiguration.Actual().TopRow).ToList().Select(a => new OrdenPagoSon
            {
                IdOrdenPago = a.IdOrdenPago,
                NoOrdenPago = a.NoOrdenPago,
                IdRecinto = a.IdRecinto,
                TextoIdentificador = a.TextoIdentificador,
                UsuarioRemitente = a.UsuarioRemitente,
                Sistema = a.Sistema,
                FechaEnvio = a.FechaEnvio,
                regAnulado = a.regAnulado,
                CodRecibo = a.CodRecibo,
                Identificador = a.Identificador,
                IdTipoDeposito = a.IdTipoDeposito,
                TipoDeposito = a.TipoDeposito,
                IdArea = a.IdArea,
                DetOrdenPagoArancel = a.DetOrdenPagoArancel,
                CantidadPagos = a.DetOrdenPagoArancel.Where(w => w.IdOrdenPago == a.IdOrdenPago && w.regAnulado == false).Count(),
                Area = clsSessionHelper.areasMemory.Where(w => w.codigo == a.IdArea).FirstOrDefault().descripcion.ToUpper(),
                Recinto = clsSessionHelper.recintosMemory.Where(w => w.IdRecinto == a.IdRecinto).Select(s => s.Siglas).FirstOrDefault().ToString()
            }
            ).Where(b => r.Any(a => b.IdRecinto == a.IdRecinto)).ToList();

            }

        }

        public OrdenPagoSon FindById(int Id)
        {

            return (OrdenPagoSon)db.OrdenPago.Find(Id);
        }

        public List<OrdenPagoSon> FindByText(string text)
        {
            if (!text.Equals(""))
            {
                string[] busqueda = text.Trim().Split(' ');

                using (SIFOPEntities t = new SIFOPEntities())
                {                    
                    return t.OrdenPago.ToList().Select(a => new OrdenPagoSon
                    {
                        IdOrdenPago = a.IdOrdenPago,
                        NoOrdenPago = a.NoOrdenPago,
                        IdRecinto = a.IdRecinto,
                        TextoIdentificador = a.TextoIdentificador,
                        UsuarioRemitente = a.UsuarioRemitente,
                        Sistema = a.Sistema,
                        FechaEnvio = a.FechaEnvio,
                        regAnulado = a.regAnulado,
                        CodRecibo = a.CodRecibo,
                        Identificador = a.Identificador,
                        IdTipoDeposito = a.IdTipoDeposito,
                        TipoDeposito = a.TipoDeposito,
                        IdArea = a.IdArea,
                        DetOrdenPagoArancel = a.DetOrdenPagoArancel,
                        CantidadPagos = t.DetOrdenPagoArancel.Where(w => w.IdOrdenPago == a.IdOrdenPago && w.regAnulado == false).Count(),
                        Area = clsSessionHelper.areasMemory.Where(w => w.codigo == a.IdArea).FirstOrDefault().descripcion.ToUpper(),
                        Recinto = clsSessionHelper.recintosMemory.Find(w => w.IdRecinto == a.IdRecinto).Siglas
                    }).Where(
                       w => busqueda.All(a => w.TextoIdentificador.Contains(a))
                    && (w.regAnulado == false && string.IsNullOrEmpty(w.CodRecibo))).ToList().Where(b => r.Any(a => b.IdRecinto == a.IdRecinto)).ToList();

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
                throw new AuthorizationException(PermisoName, IdRecinto);
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
        public string Area { get; set; }
        public string Recinto { get; set; }

        public string PorCuenta { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
