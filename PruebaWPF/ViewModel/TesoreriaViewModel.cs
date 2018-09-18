using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Interface;
using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.ViewModel
{
    class TesoreriaViewModel : ISecurity
    {
        private SIFOPEntities db = new SIFOPEntities();
        private Pantalla pantalla;

        public TesoreriaViewModel(Pantalla pantalla)
        {
            this.pantalla = pantalla;
        }

        public TesoreriaViewModel()
        {

        }

        public List<CajaSon> FindAllCajas()
        {

            return db.Caja.Select(s => new CajaSon()
            {
                IdCaja = s.IdCaja,
                Nombre = s.Nombre,
                MAC = s.MAC,
                UsuarioCreacion = s.UsuarioCreacion,
                IdRecinto = s.IdRecinto,
                IdSerie = s.IdSerie,
                regAnulado = s.regAnulado,
                FechaCreacion = s.FechaCreacion,
                Recinto = db.vw_RecintosRH.Where(w => w.IdRecinto == s.IdRecinto).Select(a => a.Siglas).FirstOrDefault().ToString(),
                cantidadRecibos = db.Recibo1.Where(w => w.IdCaja== s.IdCaja).Count()
            }).Where(w => w.regAnulado == false).ToList().Where(b => new SecurityViewModel().RecintosPermiso(pantalla).Any(a => b.IdRecinto == a.IdRecinto)).ToList();
        }


        public List<SerieRecibo> FindAllSeries()
        {
            return db.SerieRecibo.ToList();
        }

        public List<SerieRecibo> FindAddSeries(string IdSerie)
        {
            return db.SerieRecibo.Where(w => w.regAnulado == false && (!db.Caja.Any(c => c.IdSerie == w.IdSerie && c.regAnulado == false)) || w.IdSerie == IdSerie).ToList();
        }

        public string FindMacActual()
        {

            byte[] bytes = NetworkInterface.GetAllNetworkInterfaces().ToList().FirstOrDefault().GetPhysicalAddress().GetAddressBytes();

            string MAC = "";
            for (int i = 0; i < bytes.Length; i++)
            {

                MAC = MAC + "" + bytes[i].ToString("X2");

                if (i != bytes.Length - 1)
                {
                    MAC = MAC + "-";

                }
            }

            return MAC;
        }

        public void SaveUpdateCaja(Caja c)
        {
            var cajas = db.Caja.Where(w => (w.MAC == c.MAC || (w.Nombre == c.Nombre && w.IdRecinto == c.IdRecinto)) && w.regAnulado == false).ToList();

            Caja caja;
            if (c.IdCaja == 0)
            {
                if (cajas.Count() == 0)
                {
                    caja = new Caja();

                    AtributosCaja(caja, c);

                    caja.UsuarioCreacion = clsSessionHelper.usuario.Login;
                    caja.FechaCreacion = System.DateTime.Now;

                    db.Caja.Add(caja);
                }
                else
                {
                    ExcepcionCaja();
                }
            }
            else
            {
                if (cajas.Where(w => w.IdCaja != c.IdCaja).Count() == 0)
                {
                    caja = db.Caja.Find(c.IdCaja);

                    AtributosCaja(caja, c);

                    db.Entry(caja).State = System.Data.Entity.EntityState.Modified;

                }
                else
                {
                    ExcepcionCaja();
                }
            }
            db.SaveChanges();

        }

        public void SaveSerie(SerieRecibo serie)
        {
            if (db.SerieRecibo.Where(w => w.IdSerie == serie.IdSerie).Count() == 0)
            {
                serie.UsuarioCreacion = clsSessionHelper.usuario.Login;
                serie.FechaCreacion = System.DateTime.Now;
                db.SerieRecibo.Add(serie);
                db.SaveChanges();
            }
            else
            {
                ExcepcionSerie();
            }
        }

        public void EliminarSerie(SerieRecibo c)
        {
            SerieRecibo serie = db.SerieRecibo.Find(c.IdSerie);
            serie.regAnulado = true;
            db.Entry(serie).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        private void ExcepcionCaja()
        {
            throw new Exception("No es posible agregar una caja con el mismo nombre o la misma direccón MAC, por favor verifique la información. Es posible que el MAC haya sido utilizado para otro recinto de la Universidad.");
        }


        private void ExcepcionSerie()
        {
            throw new Exception("No es posible registrar una seria que ya existe o haya existido en el sistema.");
        }

        private void AtributosCaja(Caja caja, Caja c)
        {
            caja.Nombre = c.Nombre;
            caja.MAC = c.MAC;
            caja.IdRecinto = c.IdRecinto;
            caja.IdSerie = c.IdSerie;
        }

        public void EliminarCaja(CajaSon c)
        {
            Caja caja = db.Caja.Find(c.IdCaja);
            caja.regAnulado = true;
            db.Entry(caja).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
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
    public class CajaSon : Caja, ICloneable
    {
        public string Recinto { get; set; }
        public int cantidadRecibos { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
