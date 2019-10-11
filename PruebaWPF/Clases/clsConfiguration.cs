using PruebaWPF.Helper;
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
        public bool rememberMe { get; set; }
        public string userRemember { get; set; }

        public clsConfiguration()
        {
            Properties.Settings settings = Properties.Settings.Default;
            Sleep = settings.ThreadSleep;
            AutoLoad = settings.AutomaticReload;
            TopRow = settings.TopRow;
            rememberMe = settings.rememberSession;
            userRemember = settings.UserRemember;
        }

        public static clsConfiguration Actual()
        {
            return new clsConfiguration();
        }

        public static int MiliSecondSleep()
        {
            return Actual().Sleep * 1000;
        }

        public static clsConfiguration Default()
        {
            return new clsConfiguration()
            {
                Sleep = 3,
                AutoLoad = true,
                TopRow = 1000,
                rememberMe = false,
                userRemember = ""
            };
        }

        public enum Llaves
        {
            Consecutivo_Recibo,
            IdMatricula,
            IdPrematricula,
            Perfil_Cajero,
            Saldo_Inicial_Cajas,
            IdAgenteExterno,
            Id_Efectivo

        };

        public void Save()
        {
            Properties.Settings settings = Properties.Settings.Default;
            settings.AutomaticReload = AutoLoad;
            settings.TopRow = TopRow;
            settings.ThreadSleep = Sleep;
            settings.rememberSession = rememberMe;

            if (rememberMe)
            {
                settings.UserRemember = clsSessionHelper.usuario.Login;
            }
            else
            {
                settings.UserRemember = "";
            }

            settings.Save();
        }

        public static void saveUser(string user)
        {
            if (new clsConfiguration().rememberMe)
            {
                Properties.Settings.Default.UserRemember = user;
                Properties.Settings.Default.Save();
            }
        }

    }
}
