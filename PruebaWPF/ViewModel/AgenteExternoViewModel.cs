﻿using PruebaWPF.Clases;
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
    class AgenteExternoViewModel : IGestiones<AgenteExternoCat>
    {
        private SIFOPEntities db = new SIFOPEntities();
        private Pantalla pantalla;

        public AgenteExternoViewModel()
        {

        }
        public AgenteExternoViewModel(Pantalla pantalla)
        {
            this.pantalla = pantalla;
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

        public void Eliminar(AgenteExternoCat Obj)
        {
            AgenteExternoCat a = db.AgenteExternoCat.Find(Obj.IdAgenteExterno);
            a.regAnulado = true;

            db.Entry(a).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public List<AgenteExternoCat> FindAll()
        {
            return db.AgenteExternoCat.Where(w => w.regAnulado == false).Take(clsConfiguration.Actual().TopRow).OrderBy(a => a.Nombre).ToList();
        }

        public AgenteExternoCat FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public List<AgenteExternoCat> FindByText(string text)
        {
            if (!text.Equals(""))
            {
                string[] busqueda = text.Trim().Split(' ');

                //Este método retorna un error cuando se realiza la búsqueda demasiado rápido, todo es culpa de entity y los métodos asíncronos
                return db.AgenteExternoCat.Where(
                   w => busqueda.All(a => w.Nombre.Contains(a))
                && w.regAnulado == false).OrderBy(a => a.Nombre).ToList();

            }
            else
            {
                return FindAll();
            }
        }

        public List<IdentificacionAgenteExterno> FindIdentificaciones(int? idInclude)
        {
            if (idInclude == null)
            {
                return db.IdentificacionAgenteExterno.Where(w => w.regAnulado == false).ToList();
            }
            else
            {
                //incluye el tipo de identificacion que usa el registro, esto es para el update
                return db.IdentificacionAgenteExterno.Where(w => w.regAnulado == false || w.IdIdentificacion == idInclude).ToList();
            }
        }

        public void Guardar(AgenteExternoCat Obj)
        {
            AgenteExternoCat agente = new AgenteExternoCat();
            AtributosAgente(agente, Obj);
            agente.UsuarioCreacion = clsSessionHelper.usuario.Login;
            agente.FechaCreacion = System.DateTime.Now;

            db.AgenteExternoCat.Add(agente);
            db.SaveChanges();
        }

        public void Modificar(AgenteExternoCat Obj)
        {
            AgenteExternoCat agente = db.AgenteExternoCat.Find(Obj.IdAgenteExterno);
            AtributosAgente(agente, Obj);

            db.Entry(agente).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        private void AtributosAgente(AgenteExternoCat agente, AgenteExternoCat obj)
        {
            agente.Nombre = obj.Nombre;
            agente.IdIdentificacion = obj.IdIdentificacion;
            agente.Identificacion = obj.Identificacion;
            agente.Procedencia = obj.Procedencia;
        }

    }
}
