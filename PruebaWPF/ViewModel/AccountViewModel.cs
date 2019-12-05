
using PruebaWPF.Helper;
using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PruebaWPF.ViewModel
{
    class AccountViewModel
    {
        private SIFOPEntities db;
        private SecurityViewModel sec;
        public AccountViewModel()
        {
            db = new SIFOPEntities();
            sec = new SecurityViewModel(db);
        }

        public List<Pantalla> ObtenerMenu()
        {
            IQueryable<UsuarioPerfil> querable = sec.perfiles();
            IQueryable<Permiso> permisos = db.Permiso.Where(w => querable.Any(a => a.IdPerfil == w.IdPerfil) && w.Pantalla.isMenu && w.IdPermisoName == 1 && w.Pantalla.isWeb == false && w.Pantalla.regAnulado == false);

            IQueryable<Pantalla> hijos = permisos.Where(w => w.Pantalla.Uid != null).Select(s => s.Pantalla).Distinct().OrderBy(o => o.Orden);
            IQueryable<Pantalla> padres = FindPadres(hijos);

            return padres.ToList();
        }

        public List<AccesoDirectoUsuarioSon> AccesosDirectoUsuario()
        {
            IQueryable<UsuarioPerfil> querable = sec.perfiles();

            IQueryable<Permiso> permisos = db.Permiso.Where(w => querable.Any(a => a.IdPerfil == w.IdPerfil) && w.IdPermisoName == 1 && w.Pantalla.isWeb == false && w.Pantalla.regAnulado == false);
            IQueryable<Pantalla> hijos = permisos.Where(w => w.Pantalla.Uid != null).Select(s => s.Pantalla).Distinct();

            IQueryable<AccesoDirectoUsuario> user = db.AccesoDirectoUsuario.Where(w => w.IdUsuario == clsSessionHelper.usuario.Login);
            IQueryable<AccesoDirectoUsuarioSon> accesos =

                hijos.Select(s => new AccesoDirectoUsuarioSon
                {
                    IdAccesoDirectoUsuario = user.Where(a => a.IdPantalla == s.IdPantalla).Select(s1 => s1.IdAccesoDirectoUsuario).FirstOrDefault(),
                    hasAD = user.Any(a => a.IdPantalla == s.IdPantalla),
                    Pantalla = s,
                    BackgroundCard = user.Where(a => a.IdPantalla == s.IdPantalla).Select(s1 => s1.BackgroundCard).FirstOrDefault()
                });

            return accesos.ToList();
        }

        private IQueryable<Pantalla> FindPadres(IQueryable<Pantalla> hijos)
        {
            IQueryable<Pantalla> padre = db.Pantalla.Where(w => w.regAnulado == false).OrderBy(o => o.Orden).Where(w => hijos.Any(a => a.IdPadre == w.IdPantalla));

            if (padre.Any(w => w.IdPadre != null))
            {
                padre = FindPadres(padre);
            }
            return hijos.Union(padre);
        }

        public String ObtenerTipoCambio()
        {
            return new VariacionCambiariaViewModel().GetTipoCambioBD();
        }

        public List<Pantalla> ObtenerAccesoDirectoPerfil()
        {
            IQueryable<UsuarioPerfil> querable = sec.perfiles();
            //Primero obtengo todo en una lista porque la lista de perfiles se encuentra cargada en memoria y se genera un error de InvalidCastException al intentar realizar el where
            return db.AccesoDirectoPerfil.Where(w => querable.Any(a => w.IdPerfil == a.IdPerfil) && w.Pantalla.regAnulado == false).Select(s => s.Pantalla).Distinct().ToList();
        }

        public List<AccesoDirectoUsuario> ObtenerAccesoDirectoUsuario()
        {
            IQueryable<UsuarioPerfil> querable = sec.perfiles();
            IQueryable<Pantalla> pantallasAcceso = db.Permiso.Where(w => querable.Any(a => a.IdPerfil == w.IdPerfil) && w.Pantalla.isMenu && w.IdPermisoName == 1 && w.Pantalla.isWeb == false && w.Pantalla.regAnulado == false).Select(s => s.Pantalla).Distinct();
            return db.AccesoDirectoUsuario.Where(w => w.IdUsuario == clsSessionHelper.usuario.Login && pantallasAcceso.Any(e => e.IdPantalla == w.IdPantalla)).ToList();
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

        internal List<UsuarioPerfil> FindPerfiles()
        {
            return new SecurityViewModel(db).perfilesUser();
        }
    }

    class AccesoDirectoUsuarioSon : AccesoDirectoUsuario
    {
        public bool hasAD { get; set; }
    }
}
