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
    class AccountViewModel
    {
        private SIFOPEntities db = new SIFOPEntities();

        public AccountViewModel() { }

        public List<Pantalla> ObtenerMenu()
        {
            List<Perfil> perfiles = clsSessionHelper.perfiles.Select(s=>s.Perfil).ToList();
            List<Permiso> permisos = db.Permiso.ToList().Where(w => perfiles.Any(a => a.IdPerfil == w.IdPerfil)).ToList();

            List<Pantalla> hijos = permisos.Where(w => w.Pantalla.Uid != null).Select(s => s.Pantalla).Distinct().ToList();
            List<Pantalla> padres = FindPadres(hijos);

            return padres;
        }

        private List<Pantalla> FindPadres(List<Pantalla> hijos)
        {
            List<Pantalla> padre = db.Pantalla.ToList().Where(w => hijos.Any(a => a.IdPadre == w.IdPantalla)).ToList();

            if (padre.Where(w => w.IdPadre != null).Count() > 0)
            {
                padre = FindPadres(padre).ToList();
            }
            return hijos.Union(padre).ToList();
        }

        public String ObtenerTipoCambio()
        {
            return new VariacionCambiariaViewModel().GetTipoCambioBD();
        }

        public List<AccesoDirectoPerfil> ObtenerAccesoDirectoPerfil()
        {
            //Primero obtengo todo en una lista porque la lista de perfiles se encuentra cargada en memoria y se genera un error de InvalidCastException al intentar realizar el where
            return db.AccesoDirectoPerfil.ToList().Where(w => clsSessionHelper.perfiles.Any(a => w.IdPerfil == a.IdPerfil)).ToList();
        }

        public List<AccesoDirectoUsuario> ObtenerAccesoDirectoUsuario()
        {
            return db.AccesoDirectoUsuario.Where(w => w.IdUsuario == clsSessionHelper.usuario.Login).ToList();
        }

    }
}
