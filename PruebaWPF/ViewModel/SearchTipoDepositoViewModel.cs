using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PruebaWPF.ViewModel
{
    class SearchTipoDepositoViewModel
    {
        private SIFOPEntities db = new SIFOPEntities();
        public SearchTipoDepositoViewModel() { }

        public bool AuthorizePantallaIncrustada(Pantalla pantalla, string PermisoName)
        {
            if (new SecurityViewModel().Authorize(pantalla, PermisoName))
            {
                return true;
            }
            else
            {
                throw new AuthorizationException(PermisoName);
            }
        }

        public List<fn_ConsultarInfoExterna_Result> ObtenerTipoDeposito(int tipodeposito, string criterio, bool busquedainterna, string texto, int top, int? tipoarancel)
        {

            bool? isReingreso = false;
            if (tipoarancel.Value == int.Parse(db.Configuracion.First(f => f.Llave == clsConfiguration.Llaves.IdMatricula.ToString()).Valor))
            {
                isReingreso = IsNewOrReingreso(criterio);
            }
            else
            {
                isReingreso = null;
            }
            List<fn_ConsultarInfoExterna_Result> items = db.fn_ConsultarInfoExterna(tipodeposito, criterio, busquedainterna, texto, top, isReingreso).ToList().Select(s =>
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

        public static bool IsNewOrReingreso(string criterio)
        {
            bool flag = false;

            if (criterio.Contains("-")) //Si contiene guion entonces corresponde a un número de carnet
            {
                flag = true;
            }
            else if (!criterio.StartsWith(System.DateTime.Now.Year.ToString())) //Si no inicia con el año actual entonces es un carnet sin guiones
            {
                flag = true;
            }
            return flag;
        }
    }
}
