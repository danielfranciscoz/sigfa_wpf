using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Interface;
using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.ViewModel
{
    class AperturaCajaViewModel : IGestiones<AperturaCaja>
    {
        private SIFOPEntities db = new SIFOPEntities();
        private Pantalla pantalla;

        public AperturaCajaViewModel(Pantalla pantalla)
        {
            this.pantalla = pantalla;
        }

        //TODO Toda la gestión de apertura y cierre de caja
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
            return db.DetAperturaCaja.OrderByDescending(o => o.IdDetAperturaCaja).Take(clsConfiguration.Actual().TopRow).ToList()
                .Select(s => new DetAperturaCajaSon()
                {
                    AperturaCaja = s.AperturaCaja,
                    UsuarioCierre = s.UsuarioCierre,
                    FechaCierre = s.FechaCierre,
                    Caja = s.Caja,
                    IdAperturaCaja = s.IdAperturaCaja,
                    IdDetAperturaCaja = s.IdDetAperturaCaja,
                    Recinto = clsSessionHelper.recintosMemory.Where(w => w.IdRecinto == s.AperturaCaja.IdRecinto).FirstOrDefault().Siglas
                }).Where(b => new SecurityViewModel().RecintosPermiso(pantalla).Any(a => b.AperturaCaja.IdRecinto == a.IdRecinto)).ToList();
        }

        public AperturaCaja InicializarAperturaCaja()
        {
            AperturaCaja a = new AperturaCaja();
            a.FechaApertura = System.DateTime.Now;
            a.SaldoInicial = double.Parse(db.Configuracion.Where(w => w.Llave == clsConfiguration.Llaves.Saldo_Inicial_Cajas.ToString()).FirstOrDefault().Valor);
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

                return db.DetAperturaCaja.OrderByDescending(o => o.IdDetAperturaCaja).ToList().Select(s => new DetAperturaCajaSon()
                {
                    AperturaCaja = s.AperturaCaja,
                    UsuarioCierre = s.UsuarioCierre,
                    FechaCierre = s.FechaCierre,
                    Caja = s.Caja,
                    IdAperturaCaja = s.IdAperturaCaja,
                    IdDetAperturaCaja = s.IdDetAperturaCaja,
                    Recinto = clsSessionHelper.recintosMemory.Where(w => w.IdRecinto == s.AperturaCaja.IdRecinto).FirstOrDefault().Siglas
                })

                .Where(b => new SecurityViewModel().RecintosPermiso(pantalla).Any(a => b.AperturaCaja.IdRecinto == a.IdRecinto) &&
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

                    foreach (Caja c in cajas)
                    {
                        det.Add(new DetAperturaCaja() { IdAperturaCaja = Obj.IdApertura, IdCaja = c.IdCaja, UsuarioCierre = null, FechaCierre = null });
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
            return new SecurityViewModel().RecintosPermiso(pantalla, PermisoName);
        }

        public bool Autorice_Recinto(string PermisoName, int IdRecinto)
        {
            if (new SecurityViewModel().Autorize(pantalla, PermisoName, IdRecinto))
            {
                return true;
            }
            else
            {
                throw new AuthorizationException(PermisoName, IdRecinto);
            }
        }

        public bool Autorice(string PermisoName)
        {
            if (new SecurityViewModel().Autorize(pantalla, PermisoName))
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
    }
}
