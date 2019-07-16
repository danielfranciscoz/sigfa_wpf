using PruebaWPF.Model;
using System;
using System.Collections.Generic;

namespace PruebaWPF.Interface
{
  public interface ISecurity
    {
        
        Boolean Authorize_Recinto(string PermisoName,int IdRecinto);
        Boolean Authorize(string PermisoName);

    }
}
