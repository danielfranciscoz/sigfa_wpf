using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PruebaWPF.Model;

namespace PruebaWPF.Clases
{
    class PerfilComparer : IEqualityComparer<Model.Perfil>
    {
        public bool Equals(Perfil x, Perfil y)
        {
            return x.IdPerfil == y.IdPerfil;
        }

        public int GetHashCode(Perfil obj)
        {
            return obj.IdPerfil.GetHashCode();
        }
    }
}
