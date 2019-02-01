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
            return db.fn_ObtenerMenu(clsSessionHelper.usuario.Login,false).ToList().Select(a => new Pantalla { IdPantalla = a.IdPantalla.Value, IdPadre = a.IdPadre.Value, Titulo = a.Titulo, isMenu = a.isMenu.Value, URL = a.URL, Orden = a.Orden.Value, Tipo = a.Tipo, Icon = a.Icon, Uid = a.UId }).OrderBy(a => a.Orden).ToList();
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
