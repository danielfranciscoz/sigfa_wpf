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
    class AgenteExternoViewModel : IGestiones<AgenteExterno>
    {
        private SIFOPEntities db = new SIFOPEntities();
        private Pantalla pantalla;

        public AgenteExternoViewModel()
        {

        }
        public AgenteExternoViewModel(Pantalla pantalla)
        {
            this.pantalla = pantalla;
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

        public void Eliminar(AgenteExterno Obj)
        {
            AgenteExterno a = db.AgenteExterno.Find(Obj.IdAgenteExterno);
            a.RegAnulado = true;

            db.Entry(a).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public List<AgenteExterno> FindAll()
        {
            return db.AgenteExterno.Where(w => w.RegAnulado == false).Take(clsConfiguration.Actual().TopRow).OrderBy(a => a.Nombre).ToList();
        }

        public AgenteExterno FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public List<AgenteExterno> FindByText(string text)
        {
            if (!text.Equals(""))
            {
                string[] busqueda = text.Trim().Split(' ');

                //Este método retorna un error cuando se realiza la búsqueda demasiado rápido, todo es culpa de entity y los métodos asíncronos
                return db.AgenteExterno.Where(
                   w => busqueda.All(a => w.Nombre.Contains(a))
                && w.RegAnulado == false).OrderBy(a => a.Nombre).ToList();

            }
            else
            {
                return FindAll();
            }
        }

        public void Guardar(AgenteExterno Obj)
        {
            throw new NotImplementedException();
        }

        public void Modificar(AgenteExterno Obj)
        {
            throw new NotImplementedException();
        }
    }
}
