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

        public AuthorizationException(string PermisoName) : base(string.Format("Se denegó el permiso {0} , no es posible realizar la acción soliticada.",PermisoName)) { }
        public AuthorizationException(string PermisoName,int IdRecinto) : base(string.Format("Se denegó el permiso {0} para el recinto {1}, no es posible realizar la acción soliticada.",PermisoName, clsSessionHelper.recintosMemory.Where(w => w.IdRecinto == IdRecinto).Select(s => s.Siglas).FirstOrDefault())) { }

    }
}
