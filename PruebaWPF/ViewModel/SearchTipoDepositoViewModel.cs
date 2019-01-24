using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.ViewModel
{
    class SearchTipoDepositoViewModel
    {
        private SIFOPEntities db = new SIFOPEntities();
        public SearchTipoDepositoViewModel() { }
        
        public bool AutoricePantallaIncrustada(Pantalla pantalla, string PermisoName)
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

        public List<fn_ConsultarInfoExterna_Result> ObtenerTipoDeposito(int tipodeposito, string criterio, bool busquedainterna, string texto, int top)
        {
            List<fn_ConsultarInfoExterna_Result> items = db.fn_ConsultarInfoExterna(tipodeposito, criterio, busquedainterna, texto, top).ToList().Select(s =>
             new fn_ConsultarInfoExterna_Result()
             {
                 IdInterno = s.IdInterno,
                 Id = s.Id,
                 Nombre = s.Nombre,
                 Info1 = s.Info1,
                 Info2 = s.Info2,
                 IdentificatorType = s.IdentificatorType
             }).ToList();

            if (items.Count > 0)
            {
                return items;
            }
            else
            {
                throw new Exception(clsReferencias.MESSAGE_Cero_Search);
            }
        }
    }
}
