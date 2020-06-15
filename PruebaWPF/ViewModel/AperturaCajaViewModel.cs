using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Interface;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PruebaWPF.ViewModel
{
    class AperturaCajaViewModel : IGestiones<AperturaCaja>
    {
        private SIFOPEntities db = new SIFOPEntities();
        private Pantalla pantalla;
        IQueryable<vw_RecintosRH> r;
        private SecurityViewModel seguridad;

        public AperturaCajaViewModel(Pantalla pantalla)
        {
            seguridad = new SecurityViewModel(db);
            r = seguridad.RecintosPermiso(pantalla);

            this.pantalla = pantalla;
        }

        public void Eliminar(AperturaCaja Obj)
        {
            throw new NotImplementedException();
        }

        public List<AperturaCaja> FindAll()
        {
            throw new NotImplementedException();
        }

        public List<DetAperturaCajaSon> FindAllAperturas()
        {


            return db.DetAperturaCaja
                .Join(
                    db.vw_RecintosRH,
                    a => a.AperturaCaja.IdRecinto,
                    recinto => recinto.IdRecinto,
                    (a, recinto) => new { a, recinto }
                )
                .Select(s => new DetAperturaCajaSon()
                {
                    AperturaCaja = s.a.AperturaCaja,
                    UsuarioCierre = s.a.UsuarioCierre,
                    FechaCierre = s.a.FechaCierre,
                    Caja = s.a.Caja,
                    IdAperturaCaja = s.a.IdAperturaCaja,
                    IdDetAperturaCaja = s.a.IdDetAperturaCaja,
                    Recibo = s.a.Recibo,
                    Recinto = s.recinto.Siglas,
                    Arqueo = s.a.Arqueo
                })
                .Take(clsConfiguration.Actual().TopRow)
                .Where(w => r.Any(a => w.AperturaCaja.IdRecinto == a.IdRecinto))
                .OrderByDescending(o => o.IdDetAperturaCaja).ToList();

        }

        public AperturaCaja InicializarAperturaCaja()
        {
            AperturaCaja a = new AperturaCaja();
            a.FechaApertura = System.DateTime.Now;
            a.SaldoInicial = double.Parse(db.Configuracion.Find(clsConfiguration.Llaves.Saldo_Inicial_Cajas.ToString()).Valor);
            a.UsuarioCreacion = clsSessionHelper.usuario.Login;
            return a;
        }

        public List<Caja> FindCajas(int idRecinto)
        {
            List<DetAperturaCaja> aperturadas = db.DetAperturaCaja.ToList().Where(w => w.AperturaCaja.FechaApertura.Date == System.DateTime.Now.Date && w.FechaCierre == null).ToList();

            return db.Caja.Where(w => w.IdRecinto == idRecinto && w.regAnulado == false).ToList().Where(w => aperturadas.All(a => w.IdCaja != a.IdCaja)).ToList();
        }

        public AperturaCaja FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public List<AperturaCaja> FindByText(string text)
        {
            throw new NotImplementedException();
        }

        public List<DetAperturaCajaSon> FindAperturasByText(string text)
        {

            if (!text.Equals(""))
            {
                return FindAllAperturas()
                .Where(b => 
                    (b.AperturaCaja.FechaApertura.Year.ToString() + "/" + b.AperturaCaja.FechaApertura.ToString("MM")).Contains(text)).ToList();
            }
            else
            {
                return FindAllAperturas();
            }
        }

        public void Guardar(AperturaCaja Obj)
        {
            throw new NotImplementedException();
        }

        public void Guardar(AperturaCaja Obj, List<Caja> cajas)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.AperturaCaja.Add(Obj);
                    List<DetAperturaCaja> det = new List<DetAperturaCaja>();

                    int maxID = db.DetAperturaCaja.Max(m =>(int?) m.IdDetAperturaCaja)??0;

                    foreach (Caja c in cajas)
                    {
                        maxID++;
                        det.Add(new DetAperturaCaja() { IdDetAperturaCaja = maxID, IdAperturaCaja = Obj.IdApertura, IdCaja = c.IdCaja, UsuarioCierre = null, FechaCierre = null });
                    }

                    db.DetAperturaCaja.AddRange(det);

                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        public void CerrarCaja(DetAperturaCaja caja)
        {
            DetAperturaCaja apertura = db.DetAperturaCaja.Find(caja.IdDetAperturaCaja);
            apertura.FechaCierre = System.DateTime.Now;
            apertura.UsuarioCierre = clsSessionHelper.usuario.Login;

            db.Entry(apertura).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();
        }
        public void Modificar(AperturaCaja Obj)
        {
            throw new NotImplementedException();
        }

        public List<vw_RecintosRH> Recintos(string PermisoName)
        {
            return seguridad.RecintosPermiso(pantalla, PermisoName);
        }

        public bool Authorize_Recinto(string PermisoName, int IdRecinto)
        {
            if (seguridad.Authorize(pantalla, PermisoName, IdRecinto))
            {
                return true;
            }
            else
            {
                throw new AuthorizationException(PermisoName, IdRecinto,db);
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

    public class DetAperturaCajaSon : DetAperturaCaja
    {
        public string Recinto { get; set; }
        public string EstadoArqueo => Arqueo == null ? clsReferencias.Pendiente : Arqueo.isFinalizado ? clsReferencias.Finalizado : clsReferencias.EnProceso;
    }
}
