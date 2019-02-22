using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Interface;
using PruebaWPF.Model;
using System;
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

                List<Usuario> usuarios =
                    db.Usuario.ToList().Where(w =>
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

        public void EliminarPerfilUsuario(UsuarioPerfil u)
        {
            UsuarioPerfil del = db.UsuarioPerfil.Find(u.Login, u.IdPerfil, u.IdRecinto);
            db.Entry(del).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
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
        #endregion

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
}
