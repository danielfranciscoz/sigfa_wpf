using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.Clases
{
    public class ClsComparer<T>
    {
        private static String[] DataArray(T data)
        {
            String[] infoArray;
            try
            {
                infoArray = data.GetType().GetProperties().Select(s =>
                            {
                                object value = s.GetValue(data, null);
                                return value == null ? null : value.ToString();
                            }).ToArray();
            }
            catch (Exception)
            {

                infoArray = null;
            }


            return infoArray;
        }

        /// <summary>
        ///     Este método compara 2 listas e indetifica si son o no son iguales comparando cada uno de los elementos internos.
        /// </summary>
        /// <param name="mainList"></param>
        /// <param name="secondList"></param>
        /// <returns>Retorna true si las listas son iguales, false si no lo son</returns>
        public static Boolean ListComparer(List<T> mainList, List<T> secondList)
        {
            Boolean flag = true;
            if (mainList != null && secondList != null)
            {
                if (mainList.Count != secondList.Count) //Si las listas tienen una cantidad de registros diferentes, entonces no son iguales
                {
                    flag = false;
                }
                else
                {
                    T[] first = mainList.ToArray();
                    T[] second = secondList.ToArray();

                    for (int i = 0; i < mainList.Count; i++)
                    {
                        T arregloMemory = first[i];
                        T arregloDB = second[i];

                        String[] a = DataArray(arregloMemory);
                        String[] b = DataArray(arregloDB);

                        for (int j = 0; j < a.Length; j++)
                        {
                            if (a[j] != b[j])
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                }
            }
            else
            { //En caso que cualquiera de las 2 no sea Null, entonces significa que son diferentes
                flag = false;
            }
            return flag;
        }

    }
}
