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
        SIFOPEntities db = new SIFOPEntities();
        public void Eliminar(Arqueo Obj)
        {
            throw new NotImplementedException();
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
