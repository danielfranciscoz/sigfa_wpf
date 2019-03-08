using PruebaWPF.Helper;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.ViewModel
{
    class LoginViewModel
    {
        private SIFOPEntities db = new SIFOPEntities();

        public Usuario ObtenerUsuario(String Usuario)
        {
            return db.Usuario.Where(w => w.Login == Usuario).FirstOrDefault();
        }

        public void RecintosMemory()
        {
            clsSessionHelper.recintosMemory = db.vw_RecintosRH.ToList();
        }
        
        public void AreasMemory()
        {
            clsSessionHelper.areasMemory = db.vw_Areas.ToList();
        }

        public void MacMemory()
        {
            clsSessionHelper.MACMemory = new TesoreriaViewModel().FindMacActual();
        }

        public List<UsuarioPerfil> ObtenerPerfilesUsuario(String Usuario)
        {
            return db.UsuarioPerfil.Where(w => (w.Login == Usuario || w.Usuario.LoginEmail == Usuario) && w.Perfil.isWeb==false && w.RegAnulado == false && w.Usuario.RegAnulado == false && w.Perfil.RegAnulado==false).ToList();
        }

        //public List<UsuarioPerfil> ObtenerPerfiles(String Usuario)
        //{
        //    return s.UsuarioPerfil.Where(w => w.Login == Usuario && w.RegAnulado == false).ToList();
        //}

        public List<UsuarioPrograma> ObtenerProgramas(String Usuario)
        {
            return db.UsuarioPrograma.Where(a => a.RegAnulado == 0 && a.Login == Usuario).ToList();
        }

        public List<vw_ObtenerPeriodosEspecificos> ObtenerPeriodosEspecificos()
        {
            return db.vw_ObtenerPeriodosEspecificos.ToList();
        }

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
            clsSessionHelper.perfiles = perfiles;
            clsSessionHelper.usuario = perfiles.FirstOrDefault().Usuario;
        }
    }
}
