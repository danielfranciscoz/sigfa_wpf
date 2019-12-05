using PruebaWPF.Helper;
using PruebaWPF.Model;
using System.Collections.Generic;
using System.Linq;

namespace PruebaWPF.ViewModel
{
    class SecurityViewModel
    {
        private SIFOPEntities db;

     
        public SecurityViewModel(SIFOPEntities db) {
            this.db = db;
        }

        public IQueryable<UsuarioPerfil> perfiles() {
            return db.UsuarioPerfil.Where(w => (w.Login == clsSessionHelper.usuario.Login || w.Usuario.LoginEmail == clsSessionHelper.usuario.Login) && w.Perfil.isWeb == false && w.RegAnulado == false && w.Usuario.RegAnulado == false && w.Perfil.RegAnulado == false);
        }

        public List<UsuarioPerfil> perfilesUser()
        {
            return perfiles().ToList();
        }

        /// <summary>
        /// Retorna los recintos a los cuales tiene permiso un usuario en dependencia de la acción solicitada.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="PermisoName"></param>
        /// <returns></returns>
        public List<vw_RecintosRH> RecintosPermiso(Pantalla p, string PermisoName)
        {
            IQueryable<UsuarioPerfil> querable = perfiles();

            IQueryable<Permiso> permisos = db.Permiso.Where(w =>
                w.IdPantalla == p.IdPantalla &&
                w.PermisoName.Nombre.Equals(PermisoName) &&
                querable.Any(a => a.IdPerfil == w.IdPerfil && a.IdRecinto == w.IdRecinto)
               );


            return db.vw_RecintosRH.Where(w => permisos.Any(a => w.IdRecinto == a.IdRecinto)).ToList();
        }

        /// <summary>
        /// Retorna los recintos para los cuales el usuario tiene permitido acceder a la información.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public IQueryable<vw_RecintosRH> RecintosPermiso(Pantalla p)
        {
            IQueryable<UsuarioPerfil> querable = perfiles();

            IQueryable<Permiso> permisos = db.Permiso.Where(w =>
                w.IdPantalla == p.IdPantalla &&
                w.IdPermisoName == 1 &&
                querable.Any(a => a.IdPerfil == w.IdPerfil && a.IdRecinto == w.IdRecinto)
            );

            //var Ids = clsSessionHelper.perfiles.Select(s => s.IdRecinto).Intersect(permisos);

            return db.vw_RecintosRH.Where(w => permisos.Any(a => w.IdRecinto == a.IdRecinto));
        }

        /// <summary>
        /// Verifica si un usuario puede realizar la acción soliticada, basandose en los permisos asociados a sus perfiles y recintos de perfil
        /// </summary>
        /// <param name="p"></param>
        /// <param name="PermisoName"></param>
        /// <param name="IdRecinto"></param>
        /// <returns>True si se encuentra autorizado para realizar la acción, False en caso contrario</returns>
        public bool Authorize(Pantalla p, string PermisoName, int IdRecinto)
        {
            IQueryable<UsuarioPerfil> querable = perfiles();

            IQueryable<Permiso> permisos = db.Permiso.Where(w =>
                    w.IdPantalla == p.IdPantalla &&
                    w.PermisoName.Nombre.Equals(PermisoName) &&
                    w.IdRecinto == IdRecinto &&
                    querable.Any(a => a.IdPerfil == w.IdPerfil && a.IdRecinto == w.IdRecinto)
                );

            return permisos.Any() ? true : false;
        }

        /// <summary>
        /// Verifica si un usuario puede realizar la acción soliticada, basandose en los permisos asociados a sus perfiles
        /// Este método esta diseñado para las pantallas que sirven para gestionar información que no es filtrada por recinto, por ejemplo la VariacionCambiaria
        /// </summary>
        /// <param name="p"></param>
        /// <param name="PermisoName"></param>
        /// <param name="IdRecinto"></param>
        /// <returns>True si se encuentra autorizado para realizar la acción, False en caso contrario</returns>
        public bool Authorize(Pantalla p, string PermisoName)
        {
            IQueryable<UsuarioPerfil> querable = perfiles();

            IQueryable<Permiso> permisos = db.Permiso.Where(w =>
                w.IdPantalla == p.IdPantalla &&
                w.PermisoName.Nombre.Equals(PermisoName) &&
                querable.Any(a => a.IdPerfil == w.IdPerfil && a.IdRecinto == w.IdRecinto)
            );

            return permisos.Any() ? true : false;
        }
    }
}
