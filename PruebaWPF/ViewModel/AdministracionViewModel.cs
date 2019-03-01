﻿using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Interface;
using PruebaWPF.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.ViewModel
{
    class AdministracionViewModel : ISecurity
    {
        private SIFOPEntities db = new SIFOPEntities();
        private Pantalla pantalla;

        public AdministracionViewModel(Pantalla pantalla)
        {
            this.pantalla = pantalla;
        }

        public AdministracionViewModel() { }

        #region Administracion de Usuarios
        public List<Usuario> FindAllUsers()
        {
            return db.Usuario.ToList();
        }

        public List<Usuario> FindUsersByName(String text)
        {
            if (!text.Equals(""))
            {
                string[] busqueda = text.Trim().Split(' ');

                List<Usuario> usuarios = db.Usuario.ToList().Where(w =>
                               w.Login.ToLower().Contains(text.ToLower()) ||
                               busqueda.All(a => w.Nombre.ToLower().Contains(a.ToLower()))
                    ).ToList();

                return usuarios;
            }
            else
            {
                return FindAllUsers();
            }
        }

        public void DeletePerfilUsuario(UsuarioPerfil u)
        {
            UsuarioPerfil del = db.UsuarioPerfil.Find(u.Login, u.IdPerfil, u.IdRecinto);
            db.Entry(del).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
        }

        public void ActivaDesactivaUsuario(Usuario u)
        {
            Usuario mod = db.Usuario.Find(u.Login);
            mod.RegAnulado = u.RegAnulado;

            db.Entry(mod).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public vwEmpleadosRH ObtenerTrabajador(string cod_reloj)
        {
            Decimal codigo = decimal.Parse(cod_reloj);
            vwEmpleadosRH trab = db.vwEmpleadosRH.Where(w => w.NoEmpleado == codigo).FirstOrDefault();
            if (trab != null)
            {
                if (db.Usuario.Any(a => a.noInterno == trab.Cod_Interno))
                {
                    throw new Exception("El código de trabajador ingresado ya se encuentra asociado a un usuario");
                }
            }
            else
            {
                throw new Exception("El valor ingresado no es un código de trabajador válido");

            }
            return trab;
        }

        public InfoUsuario ObtenerInfoUsuario(Usuario u)
        {

            InfoUsuario info = new InfoUsuario();

            List<SqlParameter> p = new List<SqlParameter>();
            if (u.noInterno != null)
            {
                p.Add(new SqlParameter("@NoInterno", u.noInterno));

                info = db.Database.SqlQuery<InfoUsuario>("seg.fn_ObtenerInfoUsuario @NoInterno", p.ToArray()).First();
                info.usuario = u;
            }
            else
            {
                info.nombres = u.Nombre;
                info.usuario = u;
            }
            return info;
        }

        public List<UsuarioPerfilSon> ObtenerPerfilesUsuario(Usuario u)
        {

            return db.Usuario.Find(u.Login).UsuarioPerfil.Where(w => w.RegAnulado == false)
                 .Select(s => new UsuarioPerfilSon()
                 {
                     IdPerfil = s.IdPerfil,
                     Perfil = s.Perfil,
                     LoginCreacion = s.LoginCreacion,
                     Login = s.Login,
                     IdRecinto = s.IdRecinto,
                     Recinto = clsSessionHelper.recintosMemory.Where(w => w.IdRecinto == s.IdRecinto).FirstOrDefault().Siglas,
                 }).ToList();
        }

        public Usuario SaveUser(Usuario user, List<UsuarioPerfilSon> perfiles)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    user.LoginCreacion = clsSessionHelper.usuario.Login;
                    db.Usuario.Add(user);

                    UsuarioPerfil perfil;

                    foreach (UsuarioPerfilSon item in perfiles)
                    {
                        perfil = new UsuarioPerfil();
                        perfil.Login = user.Login;
                        perfil.IdPerfil = item.IdPerfil;
                        perfil.IdRecinto = item.IdRecinto;
                        perfil.LoginCreacion = user.LoginCreacion;

                        db.UsuarioPerfil.Add(perfil);
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
            return user;
        }

        public Usuario UpdateUser(Usuario user)
        {
            Usuario original = db.Usuario.Find(user.Login);
            original.LoginEmail = user.LoginEmail;
            original.noInterno = user.noInterno;
            original.Nombre = user.Nombre;

            db.Entry(original).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();

            return user;
        }

        public void UpdateRoles(string login, List<UsuarioPerfilSon> perfiles)
        {
            UsuarioPerfil perfil;

            foreach (UsuarioPerfilSon item in perfiles)
            {
                perfil = new UsuarioPerfil();
                perfil.Login = login;
                perfil.IdPerfil = item.IdPerfil;
                perfil.IdRecinto = item.IdRecinto;
                perfil.LoginCreacion = clsSessionHelper.usuario.Login;

                db.UsuarioPerfil.Add(perfil);
            }

            db.SaveChanges();

        }

        public bool ValidarUsuario(Usuario user)
        {
            var validacion = db.Usuario.Where(a => a.Login == user.Login || a.LoginEmail == user.LoginEmail).ToList();

            if (validacion.Count > 0)
            {
                if (string.IsNullOrEmpty(user.LoginCreacion))
                {
                    if (validacion.Any(a => a.Login == user.Login))
                    {
                        throw new Exception("La autenticación LDAP ingresada ya se encuentra asociada a un usuario del sistema");
                    }
                    else
                    {
                        throw new Exception("El correo institucional ingresado ya se encuentra asociado a un usuario del sistema");
                    }

                }
                else
                {
                    if (validacion.Where(a => a.LoginEmail == user.LoginEmail && a.Login != user.Login).Count() > 0)
                    {
                        throw new Exception("El correo institucional ingresado ya se encuentra asociado a un usuario del sistema");
                    }

                }
            }


            return true;

        }
        #endregion


        #region Perfiles
        public List<Perfil> FindAllRolesUser(Usuario user, int IdRecinto = 0)
        {
            if (string.IsNullOrEmpty(user.Login))
            {
                return FindAllPerfiles();
            }
            else
            {
                IEnumerable<Perfil> p = user.UsuarioPerfil.Where(w => w.RegAnulado == false && w.IdRecinto == IdRecinto).Select(s => s.Perfil).ToArray();

                List<Perfil> b = db.Perfil.Where(w => w.RegAnulado == false)
                    .OrderBy(o => o.Perfil1).ToArray()
                    .Except(p, new PerfilComparer()).ToList();

                return b;
            }
        }

        public List<Perfil> FindAllPerfiles()
        {
            return db.Perfil.Where(w => w.RegAnulado == false).OrderBy(o => o.Perfil1).ToList();
        }

        public List<Pantalla> FindPantallas(Perfil p)
        {
            List<Pantalla> hijos = db.Permiso.Where(w => w.IdPerfil == p.IdPerfil && w.Pantalla.Uid != null && w.IdPermisoName == 1).Select(s => s.Pantalla).Distinct().ToList();
            List<Pantalla> padres = FindPadres(hijos);
            return padres;
        }

        public List<Pantalla> FindAccesosDirectos(Perfil p)
        {
            List<Permiso> pantallasAcceso = db.Permiso.Where(w => w.IdPerfil == p.IdPerfil && w.Pantalla.Uid != null && w.IdPermisoName == 1).ToList();

            List<Pantalla> hijos = db.AccesoDirectoPerfil.ToList().Where(w => w.IdPerfil == p.IdPerfil && w.Pantalla.regAnulado == false && pantallasAcceso.Any(a => a.IdPantalla == w.IdPantalla)).Select(s => s.Pantalla).ToList();
            return hijos;
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

        public List<PermisoSon> FindPermisos(Pantalla pantalla, Perfil perfil)
        {

            return db.Permiso.Where(w => w.IdPerfil == perfil.IdPerfil && w.IdPantalla == pantalla.IdPantalla).OrderBy(o => o.IdRecinto).ToList()
                .Select(s => new PermisoSon()
                {
                    IdPermiso = s.IdPermiso,
                    IdPermisoName = s.IdPermisoName,
                    PermisoName = s.PermisoName,
                    IdPerfil = s.IdPerfil,
                    IdPantalla = s.IdPantalla,
                    FechaCreacion = s.FechaCreacion,
                    UsuarioCreacion = s.UsuarioCreacion,
                    Recinto = clsSessionHelper.recintosMemory.Where(w => w.IdRecinto == s.IdRecinto).FirstOrDefault().Siglas
                }).ToList();
        }

        public List<Perfil> FindPerfilesByName(String text)
        {
            if (!text.Equals(""))
            {
                List<Perfil> perfiles =
                    db.Perfil.Where(w =>
                                w.Perfil1.ToLower().Contains(text.ToLower())
                    ).ToList();

                return perfiles;
            }
            else
            {
                return FindAllPerfiles();
            }
        }

        public List<PantallaToAccess> FindPantallaToAccess(bool isWeb, Perfil p = null)
        {
            if (p == null)
            {
                return db.Pantalla.Where(w => w.Uid != null && w.isWeb == isWeb).Select(s => new PantallaToAccess()
                {
                    IdPantalla = s.IdPantalla,
                    Titulo = s.Titulo

                }).ToList();
            }
            else
            {
                List<Pantalla> actualAcceso = db.Permiso.Where(w => w.Pantalla.Uid != null && w.IdPermisoName == 1 && w.Pantalla.regAnulado == false).Select(s => s.Pantalla).ToList();

                return db.Pantalla.ToList().Where(w => actualAcceso.Any(a => a.IdPantalla != w.IdPantalla) && w.isWeb == isWeb).Select(s => new PantallaToAccess()
                {
                    IdPantalla = s.IdPantalla,
                    Titulo = s.Titulo
                }).ToList();

            }

        }
        public Perfil SaveUpdatePerfil(Perfil perfil, List<vw_RecintosRH> recintos, List<PantallaToAccess> pantallas)
        {

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    AccesoDirectoPerfil adp;
                    Permiso permiso;
                    string usuario = clsSessionHelper.usuario.Login;

                    if (perfil.IdPerfil == 0)
                    {
                        int id = db.Perfil.Max(m => m.IdPerfil) + 1;

                        perfil.IdPerfil = byte.Parse(id.ToString());
                        perfil.LoginCreacion = usuario;
                        db.Perfil.Add(perfil);

                        foreach (PantallaToAccess item in pantallas)
                        {
                            if (item.canAccess)
                            {
                                foreach (vw_RecintosRH recinto in recintos)
                                {
                                    permiso = new Permiso();
                                    permiso.IdPantalla = item.IdPantalla;
                                    permiso.IdPerfil = perfil.IdPerfil;
                                    permiso.UsuarioCreacion = usuario;
                                    permiso.FechaCreacion = System.DateTime.Now;
                                    permiso.IdPermisoName = 1;
                                    permiso.IdRecinto = byte.Parse(recinto.IdRecinto.ToString());

                                    db.Permiso.Add(permiso);
                                }

                                if (item.createAD)
                                {
                                    adp = new AccesoDirectoPerfil();
                                    adp.IdPantalla = item.IdPantalla;
                                    adp.IdPerfil = perfil.IdPerfil;
                                    adp.UsuarioCreacion = usuario;
                                    adp.FechaCreacion = System.DateTime.Now;

                                    db.AccesoDirectoPerfil.Add(adp);
                                }
                            }

                        }

                        db.SaveChanges();
                        transaction.Commit();
                        return perfil;
                    }
                    else
                    {
                        Perfil p = db.Perfil.Find(perfil.IdPerfil);
                        p.Perfil1 = perfil.Perfil1;
                        p.Descripcion = perfil.Descripcion;

                        db.Entry(perfil).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                        transaction.Commit();
                        return p;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

        }

        public void DeletePerfil(Perfil perfil)
        {
            Perfil p = db.Perfil.Find(perfil.IdPerfil);
            p.RegAnulado = true;
            db.Entry(p).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
        #endregion

        public List<vw_RecintosRH> Recintos()
        {
            return clsSessionHelper.recintosMemory;
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

    public class UsuarioPerfilSon : UsuarioPerfil
    {
        public string Recinto { get; set; }
    }

    public class PermisoSon : Permiso
    {
        public string Recinto { get; set; }
    }

    public class PantallaToAccess : Pantalla
    {
        public bool canAccess { get; set; }
        public bool createAD { get; set; }
    }
}
