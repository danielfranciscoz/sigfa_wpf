using PruebaWPF.Clases;
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
            return db.AperturaCaja.Where(w=>w.Fecha.Year == System.DateTime.Now.Year).ToList().Where(b => new SecurityViewModel().RecintosPermiso(pantalla).Any(a => b.IdRecinto == a.IdRecinto)).ToList();
        }

        public AperturaCaja FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public List<AperturaCaja> FindByText(string text)
        {
            throw new NotImplementedException();
        }

        public void Guardar(AperturaCaja Obj)
        {
            throw new NotImplementedException();
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
}
