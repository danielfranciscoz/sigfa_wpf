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
            List<Perfil> perfiles = clsSessionHelper.perfiles.Select(s => s.Perfil).ToList();
            List<Permiso> permisos = db.Permiso.ToList().Where(w => perfiles.Any(a => a.IdPerfil == w.IdPerfil) && w.Pantalla.isMenu && w.IdPermisoName == 1 && w.Pantalla.isWeb == false && w.Pantalla.regAnulado == false).ToList();

            List<Pantalla> hijos = permisos.Where(w => w.Pantalla.Uid != null).Select(s => s.Pantalla).Distinct().OrderBy(o=>o.Orden).ToList();
            List<Pantalla> padres = FindPadres(hijos);

            return padres;
        }

        public List<AccesoDirectoUsuarioSon> AccesosDirectoUsuario()
        {
            List<Perfil> perfiles = clsSessionHelper.perfiles.Select(s => s.Perfil).ToList();
            List<Permiso> permisos = db.Permiso.ToList().Where(w => perfiles.Any(a => a.IdPerfil == w.IdPerfil) && w.IdPermisoName == 1 && w.Pantalla.isWeb == false && w.Pantalla.regAnulado == false).ToList();
            List<Pantalla> hijos = permisos.Where(w => w.Pantalla.Uid != null).Select(s => s.Pantalla).Distinct().ToList();

            List<AccesoDirectoUsuario> user = db.AccesoDirectoUsuario.Where(w => w.IdUsuario == clsSessionHelper.usuario.Login).ToList();
            List<AccesoDirectoUsuarioSon> accesos =

                hijos.Select(s => new AccesoDirectoUsuarioSon
                {
                    IdAccesoDirectoUsuario = user.Where(a => a.IdPantalla == s.IdPantalla).Select(s1 => s1.IdAccesoDirectoUsuario).FirstOrDefault(),
                    hasAD = user.Any(a => a.IdPantalla == s.IdPantalla),
                    Pantalla = s,
                    BackgroundCard = user.Where(a => a.IdPantalla == s.IdPantalla).Select(s1 => s1.BackgroundCard).FirstOrDefault()
                }).ToList();

            return accesos;
        }

        private List<Pantalla> FindPadres(List<Pantalla> hijos)
        {
            List<Pantalla> padre = db.Pantalla.Where(w => w.regAnulado == false).OrderBy(o => o.Orden).ToList().Where(w => hijos.Any(a => a.IdPadre == w.IdPantalla)).ToList();

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

        public List<Pantalla> ObtenerAccesoDirectoPerfil()
        {
            //Primero obtengo todo en una lista porque la lista de perfiles se encuentra cargada en memoria y se genera un error de InvalidCastException al intentar realizar el where
            return db.AccesoDirectoPerfil.ToList().Where(w => clsSessionHelper.perfiles.Any(a => w.IdPerfil == a.IdPerfil) && w.Pantalla.regAnulado == false).Select(s => s.Pantalla).Distinct().ToList();
        }

        public List<AccesoDirectoUsuario> ObtenerAccesoDirectoUsuario()
        {
            List<Pantalla> pantallasAcceso = db.Permiso.ToList().Where(w => clsSessionHelper.perfiles.Any(a => a.IdPerfil == w.IdPerfil) && w.Pantalla.isMenu && w.IdPermisoName == 1 && w.Pantalla.isWeb == false && w.Pantalla.regAnulado == false).Select(s => s.Pantalla).Distinct().ToList();
            return db.AccesoDirectoUsuario.ToList().Where(w => w.IdUsuario == clsSessionHelper.usuario.Login && pantallasAcceso.Exists(e => e.IdPantalla == w.IdPantalla)).ToList();
        }

        public string SaveAccesosDirectos(List<AccesoDirectoUsuarioSon> accesos)
        {
            int agregados = 0, editados = 0, eliminados = 0;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    AccesoDirectoUsuario au;
                    foreach (AccesoDirectoUsuarioSon item in accesos)
                    {
                        if (item.IdAccesoDirectoUsuario == 0)
                        {
                            if (item.hasAD)
                            {
                                au = new AccesoDirectoUsuario();
                                au.IdPantalla = item.Pantalla.IdPantalla;
                                au.IdUsuario = clsSessionHelper.usuario.Login;
                                au.BackgroundCard = item.BackgroundCard;
                                au.FechaCreacion = System.DateTime.Now;

                                db.AccesoDirectoUsuario.Add(au);
                                agregados++;
                            }
                        }
                        else
                        {
                            au = db.AccesoDirectoUsuario.Find(item.IdAccesoDirectoUsuario);

                            if (!item.hasAD)
                            {
                                db.Entry(au).State = System.Data.Entity.EntityState.Deleted;
                                eliminados++;
                            }
                            else
                            {
                                if (au.BackgroundCard != item.BackgroundCard)
                                {
                                    au.BackgroundCard = item.BackgroundCard;
                                    db.Entry(au).State = System.Data.Entity.EntityState.Modified;
                                    editados++;
                                }
                            }
                        }
                    }

                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            return string.Format("Elementos agregados {0} \nElementos Modificados {1} \nElementos removidos {2}", agregados, editados, eliminados);
        }


    }

    class AccesoDirectoUsuarioSon : AccesoDirectoUsuario
    {
        public bool hasAD { get; set; }
    }
}
