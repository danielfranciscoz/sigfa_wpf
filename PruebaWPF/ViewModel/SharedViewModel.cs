using PruebaWPF.Helper;
using PruebaWPF.Model;
using System.Collections.Generic;
using System.Linq;

namespace PruebaWPF.ViewModel
{
    class SharedViewModel
    {
        private SIFOPEntities db = new SIFOPEntities();

        public SharedViewModel() { }

        public List<vw_Areas> ObtenerAreasRH(int idtipoArancel)
        {
            var areas = db.ArancelArea.Where(w => w.regAnulado == false && w.Arancel.IdTipoArancel == idtipoArancel && w.Arancel.regAnulado == false);
            return ObtenerAreas().Where(w => areas.Any(a => a.IdArea == w.codigo)).ToList();
        }
        private IQueryable<vw_Areas> ObtenerAreas()
        {
            return db.vw_Areas.Where(w => w.estado.Equals("A")).OrderBy(a => a.codigo).OrderBy(a => a.codigo);
        }

        public List<vw_Areas> ObtenerAreasRH()
        {
            return ObtenerAreas().ToList();
        }

        public List<vw_Areas> FindAreaByText(string text)
        {
            if (!text.Equals(""))
            {
                string[] busqueda = text.Trim().Split(' ');
                return ObtenerAreasRH().Where(
                       w => busqueda.All(a => w.nombre.Contains(a) || w.codigo.Contains(text))
                       ).ToList();
            }
            else
            {
                return ObtenerAreasRH();
            }
        }

        public List<vw_Areas> FindAreaByText(string text, int idtipoArancel)
        {
            if (!text.Equals(""))
            {
                string[] busqueda = text.Trim().Split(' ');
                return ObtenerAreasRH(idtipoArancel).Where(
                       w => busqueda.All(a => w.nombre.Contains(a) || w.codigo.Contains(text))
                       && w.estado.Equals("A")).OrderBy(a => a.codigo).ToList();
            }
            else
            {
                return ObtenerAreasRH(idtipoArancel);
            }
        }

    }
}
