using PruebaWPF.Model;
using System;
using System.Collections.Generic;

namespace PruebaWPF.Interface
{
  public interface ISecurity
    {
        
        Boolean Autorice_Recinto(string PermisoName,int IdRecinto);
        Boolean Autorice(string PermisoName);

    }
}
