namespace PruebaWPF.Referencias
{
    class clsReferencias
    {

        //Ids de Referencia

        public const int PerfilCajero = 8;
        public const string Default = "Default";

        //Estados
        public const string Pendiente = "Pendiente";
        public const string Finalizado = "Finalizado";
        public const string EnProceso = "En proceso...";


        //Naturaleza de cuenta contable
        public const bool Debe = false;
        public const bool Haber = true;

        //Tipos de mensaje
        public const int TYPE_MESSAGE_Exito = 0;
        public const int TYPE_MESSAGE_Error = 1;
        public const int TYPE_MESSAGE_Advertencia = 2;
        public const int TYPE_MESSAGE_Question = 3;
        public const int TYPE_MESSAGE_Information = 4;
        public const int TYPE_MESSAGE_Wait_a_Moment = 5;

        //Información de mensajes 
        public const string MESSAGE_Exito_Export = "Se exportó la información con éxito.";
        public const string MESSAGE_NoSelection = "Debe seleccionar un registro para poder realizar esta acción.";

        public const string MESSAGE_Exito_Save = "Se guardó la información con éxito.";
        public const string MESSAGE_Exito_Delete = "Se eliminó la información con éxito.";
        public const string MESSAGE_Exito_Anular = "La información fue anulada con éxito.";

        public const string MESSAGE_Exito_Save_COUNT = "Se guardó la información con éxito, registros insertados: ";

        public const string MESSAGE_Cero_Save = "Se intentó guardar, sin embargo toda la información ya se encuentra almacenada, utilice otros parámetros e intente de nuevo";

        public const string MESSAGE_Cero_Registro = "No se detectaron nuevos registros que guardar.";

        public const string MESSAGE_Cero_Registro_Table = "Para poder continuar debe agregar al menos un registro a la tabla.";

        public const string MESSAGE_Cero_Search = "No hemos encontrado resultados para sus criterios de búsqueda.";

        public const string MESSAGE_Confirm_Delete = "Esta a punto de eliminar un registro de la base de datos, este proceso es irreversible, ¿Realmente desea continuar?";

        public const string MESSAGE_Total_Menor_Cero = "No pueden existir montos menores a cero, por favor revise la información.";

        public const string MESSAGE_Error_Title = "Ocurrió un error inesperado.";

        public const string MESSAGE_Wrong_User = "Usuario o contraseña incorrecta.";
        public const string MESSAGE_User_NoAccess = "El usuario no posee accesos al sistema.";
        public const string MESSAGE_CajeroNoAutorizado = "Las credenciales proporcionadas no se corresponde con las del cajero que emitió recibos en esta caja, por favor verifique su información e intente nuevamente.";


        //Mensaje de validaciones
        public const string SinFecha = "Debe seleccionar o digitar una fecha.";
        public const string NumeroMal = "Se detectó un error de escritura, por favor corrija la cantida escrita para poder continuar.";
        public const string ConfirmarCajeroEntrega = "Por motivos de seguridad, es necesario que el cajero ingrese sus credenciales para confirmar su participación en el arqueo, cabe destacar que solo aceptaremos credenciales del cajero que haya generado los recibos en esta caja desde que fue aperturada hasta que fue cerrada.";
        public const string ConfirmarCajero = "Por motivos de seguridad, es necesario que un cajero ingrese sus credenciales para confirmar su participación en el arqueo.";

        public enum Informes
        {
            cierre_caja = 1,
            arqueo_caja = 2
        }

    }
}
