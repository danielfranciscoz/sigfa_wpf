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
    class ArqueoViewModel : IGestiones<Arqueo>
    {

        private SecurityViewModel seguridad;
        private Pantalla pantalla;

        public ArqueoViewModel(Pantalla pantalla)
        {
            seguridad = new SecurityViewModel();
            this.pantalla = pantalla;
        }

        SIFOPEntities db = new SIFOPEntities();
        public void Eliminar(Arqueo Obj)
        {
            throw new NotImplementedException();
        }

        public DetAperturaCaja DetectarApertura()
        {
            DetAperturaCaja apertura = new DetAperturaCaja();
            Caja c = db.Caja.Where(w => w.MAC == clsSessionHelper.MACMemory && w.regAnulado == false).First();

            if (c == null)
            {
                throw new Exception("No hemos podido detectar la caja a la cual pertenece este equipo, los arqueos solo se pueden realizar en los equipos destinados como cajas de la Universidad.");
            }

            List<DetAperturaCaja> aperturas = db.DetAperturaCaja.Where(w => w.IdCaja == c.IdCaja && w.FechaCierre != null).ToList();

            if (!aperturas.Any())
            {
                throw new Exception("Esta caja no posee cierres, por lo tanto no puede ser arqueada");
            }

            List<Arqueo> enProceso = aperturas.Where(w => w.Arqueo != null).Select(w => w.Arqueo).ToList();

            if (enProceso.Any())
            {
                if (enProceso.Where(w => w.isFinalizado == false).Any()) // El arqueo sin finalizar debe ser retornado hasta que se finalice
                {
                    return enProceso.Where(w => w.isFinalizado == false).First().DetAperturaCaja;
                }
            }

            List<DetAperturaCaja> noArqueadas = aperturas.Where(w => w.Arqueo == null).ToList();

            if (!noArqueadas.Any())
            {
                throw new Exception("Esta caja ya se encuentra arqueada");
            }

            return noArqueadas.First();



        }

        public List<Arqueo> FindAll()
        {
            return db.Arqueo.ToList();
        }

        public Arqueo FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public List<Arqueo> FindByText(string text)
        {
            throw new NotImplementedException();
        }

        public void Guardar(Arqueo Obj)
        {
            throw new NotImplementedException();
        }

        public void Modificar(Arqueo Obj)
        {
            throw new NotImplementedException();
        }

        public bool Autorice(string PermisoName)
        {
            throw new NotImplementedException();
        }

        public bool Autorice_Recinto(string PermisoName, int IdRecinto)
        {
            throw new NotImplementedException();
        }

    }
}
