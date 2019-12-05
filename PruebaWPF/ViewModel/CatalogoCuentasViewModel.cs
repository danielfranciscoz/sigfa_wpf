using PruebaWPF.Interface;
using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PruebaWPF.ViewModel
{
    class CatalogoCuentasViewModel : IGestiones<CuentaContable>
    {
        private SIFOPEntities db = new SIFOPEntities();
        private Pantalla pantalla;
        private SecurityViewModel seguridad;
        //List<vw_RecintosRH> r;

        public CatalogoCuentasViewModel(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            seguridad = new SecurityViewModel(db);
            //r = seguridad.RecintosPermiso(pantalla);
        }

        public CatalogoCuentasViewModel() { }

        public bool Authorize(string PermisoName)
        {
            throw new NotImplementedException();
        }

        public bool Authorize_Recinto(string PermisoName, int IdRecinto)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(CuentaContable Obj)
        {
            throw new NotImplementedException();
        }

        public List<CuentaContable> FindAll()
        {
            return Finding().ToList();
        }

       
        public CuentaContable FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public List<CuentaContable> FindByText(string text)
        {
            return Finding().Where(w =>w.CuentaContable1.StartsWith(text) || w.Descripcion.Contains(text)).ToList();
        }

        private IQueryable<CuentaContable> Finding() {
            return db.CuentaContable.Where(w => w.RegAnulado == false).OrderBy(o => o.CuentaContable1).ThenBy(t => t.IdOrden);
        }

        public void Guardar(CuentaContable Obj)
        {
            throw new NotImplementedException();
        }

        public void Modificar(CuentaContable Obj)
        {
            throw new NotImplementedException();
        }

        public Pantalla Pantalla(string UId)
        {
            return new PantallaViewModel().FindById(UId);
        }

    }
}
