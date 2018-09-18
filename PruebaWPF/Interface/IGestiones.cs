using System.Collections.Generic;


namespace PruebaWPF.Interface
{
   public interface IGestiones<T> : ISecurity
    {
        void Guardar(T Obj);
        void Modificar(T Obj);
        void Eliminar(T Obj);
        List<T> FindAll();
        List<T> FindByText(string text);
        T FindById(int Id);
        
    }
}
