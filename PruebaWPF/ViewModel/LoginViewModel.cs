using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PruebaWPF.ViewModel
{
    class LoginViewModel
    {
        private SIFOPEntities db = new SIFOPEntities();

        public Usuario ObtenerUsuario(String Usuario)
        {
            return db.Usuario.Where(w => w.Login == Usuario).FirstOrDefault();
        }

        public void MacMemory()
        {
            clsSessionHelper.MACMemory = clsUtilidades.FindMacActual();
        }

        public List<UsuarioPerfil> ObtenerPerfilesUsuario(String Usuario)
        {
         //   var t = new SIFOP_TESTEntities().UsuarioPerfil.ToList();
            return db.UsuarioPerfil.Where(w => (w.Login == Usuario || w.Usuario.LoginEmail == Usuario) && w.Perfil.isWeb == false && w.RegAnulado == false && w.Usuario.RegAnulado == false && w.Perfil.RegAnulado == false).ToList();
        }


        //public List<UsuarioPrograma> ObtenerProgramas(String Usuario)
        //{
        //    return db.UsuarioPrograma.Where(a => a.RegAnulado == 0 && a.Login == Usuario).ToList();
        //}

        //public List<vw_ObtenerPeriodosEspecificos> ObtenerPeriodosEspecificos()
        //{
        //    return db.vw_ObtenerPeriodosEspecificos.ToList();
        //}

        public void SeleccionarPeriodo(string IdPeriodo)
        {
            if (IdPeriodo.Equals(clsReferencias.Default))
            {
                clsSessionHelper.periodoEspecifico = db.vw_ObtenerPeriodosEspecificos.OrderByDescending(w => w.IdPeriodoEspecifico).FirstOrDefault();
            }
            else
            {
                clsSessionHelper.periodoEspecifico = db.vw_ObtenerPeriodosEspecificos.Where(w => w.IdPeriodoEspecifico.ToString() == IdPeriodo).FirstOrDefault();
            }
        }

        internal void SeleccionarPrograma(string IdPrograma)
        {
            clsSessionHelper.programa = db.Programa.Where(w => w.IdPrograma.ToString() == IdPrograma).FirstOrDefault();
        }

        internal void SeleccionarPerfilUsuario(List<UsuarioPerfil> perfiles)
        {
            //clsSessionHelper.perfiles = perfiles;
            clsSessionHelper.usuario = perfiles.FirstOrDefault().Usuario;
        }

        public bool ValidarCredenciales(String Usuario, String Password)
        {
            if (Usuario.Contains("@")) // Verificando la autenticación con Office365
            {
                clsSessionHelper.isMailLogin = true;
                var credencialesOffice = new wsOffice365.authSoapClient();
                return credencialesOffice.Validate(Usuario, Password);
            }
            else //Verificando la autenticación con LDAP
            {
                clsSessionHelper.isMailLogin = false;
                var credencialesLdap = new wsLDAP.LDAPSoapClient();
                return credencialesLdap.EsUsuarioValido(Usuario, Password);
            }
        }

        /// <summary>
        /// Este procedimiento valida que el usuario tenga asociado el perfil de cajero entre todos sus perfiles, 
        /// En caso de que se necesite validar que solo posea perfil cajero (es decir puede tener muchos perfiles pero solo pueden ser de cajero, esto ocurre cuando se es cajero en varios recintos) se deberá hacer uso del parametro validarOnlyCajero
        /// </summary>
        /// <param name="Usuario"></param>
        /// <param name="db_Context"></param>
        /// <param name="validarOnlyCajero"></param>
        /// <returns>Si el usuario es un cajero o no</returns>
        public bool isCajero(String Usuario, SIFOPEntities db_Context, bool validarOnlyCajero = false)
        {
            if (db_Context == null)
            {
                return db.Usuario.Find(Usuario).UsuarioPerfil.Any(w => w.IdPerfil == clsReferencias.PerfilCajero);
            }
            else
            {
                if (validarOnlyCajero)
                {
                    return db_Context.Usuario.Find(Usuario).UsuarioPerfil.Where(w => !w.RegAnulado && !w.Perfil.isWeb).All(w => w.IdPerfil == clsReferencias.PerfilCajero);

                }
                else
                {
                    return db_Context.Usuario.Find(Usuario).UsuarioPerfil.Any(w => w.IdPerfil == clsReferencias.PerfilCajero);

                }
            }
        }
    }
}
