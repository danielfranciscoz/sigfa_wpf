using PruebaWPF.Model;

namespace PruebaWPF.Helper

{
    class clsSessionHelper
    {
        public clsSessionHelper() { }

        public static Usuario usuario;
        public static bool isMailLogin; //Parametro para manejar si la persona se loguea con su correo o con LDAP
        public static vw_ObtenerPeriodosEspecificos periodoEspecifico;
        public static Programa programa;
        public static string serverName;

        public static string MACMemory;
        public static string SystemName;
        public static string SystemVersion;

        public static int vigenciaOP;

    }
}
