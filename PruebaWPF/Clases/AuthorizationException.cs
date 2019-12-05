using PruebaWPF.Model;
using System;
using System.Linq;

namespace PruebaWPF.Clases
{
    class AuthorizationException : Exception
    {

        public AuthorizationException()
        {

        }

        public AuthorizationException(string PermisoName)
            : base(string.Format(
                "Se denegó el permiso {0} , no es posible realizar la acción soliticada."
                , PermisoName)
                  )
        { }

        public AuthorizationException(string PermisoName, int IdRecinto, SIFOPEntities db) :
            base(string.Format(
                "Se denegó el permiso {0} para el recinto {1}, no es posible realizar la acción soliticada."
                , PermisoName
                , db.vw_RecintosRH.FirstOrDefault(w => w.IdRecinto == IdRecinto).Siglas)
                )
        { }

    }
}
