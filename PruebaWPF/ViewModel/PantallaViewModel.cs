using PruebaWPF.Interface;
using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.ViewModel
{
    class PantallaViewModel : IGestiones<Pantalla>
    {
        private SIFOPEntities db = new SIFOPEntities();

        public bool Authorize(string PermisoName)
        {
            throw new NotImplementedException();
        }

        public bool Authorize_Recinto(string PermisoName, int IdRecinto)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Pantalla Obj)
        {
            throw new NotImplementedException();
        }

        public List<Pantalla> FindAll()
        {
            throw new NotImplementedException();
        }

        public Pantalla FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public Pantalla FindById(string UId)
        {
            return db.Pantalla.Where(w => w.Uid.Equals(UId)).FirstOrDefault();
        }

        public List<Pantalla> FindByText(string text)
        {
            throw new NotImplementedException();
        }

        public void Guardar(Pantalla Obj)
        {
            throw new NotImplementedException();
        }

        public void Modificar(Pantalla Obj)
        {
            throw new NotImplementedException();
        }
    }
}
