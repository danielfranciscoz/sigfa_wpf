using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.ViewModel
{
    class SharedViewModel
    {
        private SIFOPEntities db = new SIFOPEntities();

        public SharedViewModel() { }

        public List<vw_Areas> ObtenerAreasRH()
        {

            return db.vw_Areas.Where(w=>w.estado.Equals("A") && db.ArancelArea.Any(w1=>w1.IdArea == w.codigo)).OrderBy(a => a.codigo).ToList();
        }

        public List<vw_Areas> FindAreaByText(string text)
        {
            if (!text.Equals(""))
            {
                string[] busqueda = text.Trim().Split(' ');
                return db.vw_Areas.Where(
                       w => busqueda.All(a => w.nombre.Contains(a) || w.codigo.Contains(text))
                       && w.estado.Equals("A") && db.ArancelArea.Any(w1 => w1.IdArea == w.codigo)
                       ).OrderBy(a => a.codigo).ToList();
            }
            else
            {
                return ObtenerAreasRH();
            }
        }

    }
}
