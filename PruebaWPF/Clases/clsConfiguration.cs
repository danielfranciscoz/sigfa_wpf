using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PruebaWPF.Clases
{
    class clsConfiguration
    {

        public int Sleep { get; set; }
        public bool AutoLoad { get; set; }
        public int TopRow { get; set; }


        public static clsConfiguration Actual()
        {
            return new clsConfiguration();
        }

        public static int MiliSecondSleep()
        {
            return new clsConfiguration().Sleep * 1000;
        }
        public static clsConfiguration Default()
        {
            return new clsConfiguration()
            {
                Sleep = 3,
                AutoLoad = true,
                TopRow = 1000
            };
        }

        public clsConfiguration()
        {
            Properties.Settings settings = Properties.Settings.Default;
            Sleep = settings.ThreadSleep;
            AutoLoad = settings.AutomaticReload;
            TopRow = settings.TopRow;
        }


        public enum Llaves
        {
            Consecutivo_Recibo,
            Saldo_Inicial_Cajas
        };



        //public static int[] IdMonedaWebService()
        //{
        //    return Array.ConvertAll(Properties.Settings.Default.IdMonedaWS.Split(char.Parse(",")), int.Parse);
        //}

        public void Save()
        {
            Properties.Settings settings = Properties.Settings.Default;
            settings.AutomaticReload = AutoLoad;
            settings.TopRow = TopRow;
            settings.ThreadSleep = Sleep;

            settings.Save();
        }

    }
}
