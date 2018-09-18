using PruebaWPF.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.Clases
{
    class AuthorizationException:Exception
    {
        public AuthorizationException() { }

        public AuthorizationException(string PermisoName) : base("Se denegó el permiso "+PermisoName+" , no es posible realizar la acción soliticada.") { }
        public AuthorizationException(string PermisoName,int IdRecinto) : base("Se denegó el permiso " + PermisoName + " para el recinto "+clsSessionHelper.recintosMemory.Where(w=>w.IdRecinto == IdRecinto).Select(s=>s.Siglas).FirstOrDefault()+", no es posible realizar la acción soliticada.") { }

    }
}
