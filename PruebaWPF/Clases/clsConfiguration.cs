using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PruebaWPF.Clases
{
    class clsConfiguration
    {
        public enum Llaves 
        {
            Consecutivo_Recibo
        };

        public static int RowCount()
        {
            return Properties.Settings.Default.TopRow;
        }

        /// <summary>
        /// Representa el tiempo entre consultas a la base de datos mientras se encuentre activa la opción de recarga automática
        /// </summary>
        /// <returns>Integer en milisegundos</returns>
        public static int ThreadSpeep()
        {
            return Properties.Settings.Default.ThreadSleep;
        }

        /// <summary>
        /// Configuración que permitirá determinar si se realizara el recargado automático de la información
        /// </summary>
        /// <returns>Boolean</returns>
        public static bool AutomaticReload()
        {
            return Properties.Settings.Default.AutomaticReload;
        }

        public static int[] IdMonedaWebService()
        {
            return Array.ConvertAll(Properties.Settings.Default.IdMonedaWS.Split(char.Parse(",")), int.Parse);
        }

        public static void SaveTop(int Top)
        {
            Properties.Settings.Default.TopRow = Top;
            Properties.Settings.Default.Save();
        }

        public static void SaveAutomaticReload(Boolean AutomaticReload)
        {
            Properties.Settings.Default.AutomaticReload = AutomaticReload;
            Properties.Settings.Default.Save();
        }
    }
}
