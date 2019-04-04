using PruebaWPF.Helper;
using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.ViewModel
{
    class SecurityViewModel
    {
        private SIFOPEntities db = new SIFOPEntities();

        public SecurityViewModel()
        {
        }

        /// <summary>
        /// Retorna los recintos a los cuales tiene permiso un usuario en dependencia de la acción solicitada.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="PermisoName"></param>
        /// <returns></returns>
        public List<vw_RecintosRH> RecintosPermiso(Pantalla p, string PermisoName)
        {

            var permisos = db.Permiso.Where(w => w.IdPantalla == p.IdPantalla && w.PermisoName.Nombre.Equals(PermisoName)).Select(s => s.IdRecinto);
            var Ids = clsSessionHelper.perfiles.Select(s => s.IdRecinto).Intersect(permisos);

            return clsSessionHelper.recintosMemory.Where(w => Ids.Any(a => w.IdRecinto == a)).ToList();
        }

        /// <summary>
        /// Retorna los recintos para los cuales el usuario tiene permitido acceder a la información.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<vw_RecintosRH> RecintosPermiso(Pantalla p)
        {

            var permisos = db.Permiso.Where(w => w.IdPantalla == p.IdPantalla && w.IdPermisoName == 1).Select(s => s.IdRecinto);
            var Ids = clsSessionHelper.perfiles.Select(s => s.IdRecinto).Intersect(permisos);

            return clsSessionHelper.recintosMemory.Where(w => Ids.Any(a => w.IdRecinto == a)).ToList();
        }

        /// <summary>
        /// Verifica si un usuario puede realizar la acción soliticada, basandose en los permisos asociados a sus perfiles y recintos de perfil
        /// </summary>
        /// <param name="p"></param>
        /// <param name="PermisoName"></param>
        /// <param name="IdRecinto"></param>
        /// <returns>True si se encuentra autorizado para realizar la acción, False en caso contrario</returns>
        public bool Autorize(Pantalla p, string PermisoName, int IdRecinto)
        {

            var permisos = db.Permiso.Where(w => w.IdPantalla == p.IdPantalla && w.PermisoName.Nombre.Equals(PermisoName) && w.IdRecinto == IdRecinto).Select(s => s.IdPerfil);
            var Ids = clsSessionHelper.perfiles.Select(s => s.IdPerfil).Intersect(permisos);
       
            return Ids.ToList().Count > 0 ? true : false;
        }

        /// <summary>
        /// Verifica si un usuario puede realizar la acción soliticada, basandose en los permisos asociados a sus perfiles
        /// Este método esta diseñado para las pantallas que sirven para gestionar información que no es filtrada por recinto, por ejemplo la VariacionCambiaria
        /// </summary>
        /// <param name="p"></param>
        /// <param name="PermisoName"></param>
        /// <param name="IdRecinto"></param>
        /// <returns>True si se encuentra autorizado para realizar la acción, False en caso contrario</returns>
        public bool Autorize(Pantalla p, string PermisoName)
        {

            var permisos = db.Permiso.Where(w => w.IdPantalla == p.IdPantalla && w.PermisoName.Nombre.Equals(PermisoName)).Select(s => s.IdPerfil);
            var Ids = clsSessionHelper.perfiles.Select(s => s.IdPerfil).Intersect(permisos);

            return Ids.ToList().Count > 0 ? true : false;
        }
    }
}
