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

        //Entornos
        public const string Debug = "Desarrollo";
        public const string Release = "Produccion";

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
        public const string MESSAGE_Informacion_Incompleta = "No puede continuar, es posible que el usuario no tenga registrado Correo Institucional o Número de Trabajador, por favor verifique los datos e intente de nuevo";
        public const string MESSAGE_Cero_Search = "No hemos encontrado resultados para sus criterios de búsqueda.";
        public const string MESSAGE_Confirm_Delete = "Esta a punto de eliminar un registro de la base de datos, este proceso es irreversible, ¿Realmente desea continuar?";
        public const string MESSAGE_Total_Menor_Cero = "No pueden existir montos menores a cero, por favor revise la información.";


        //Mensajes de error
        public const string MESSAGE_Error_Title = "Ocurrió un error inesperado.";
        public const string Error_NoCaja = "No hemos encontrado este ordenador entre la lista de cajas disponibles para realizar pagos en el sistema, por favor pida ayuda al administrador de tesorería.";
        public const string Error_Caja_NoAperturada = "Esta caja no se encuentra aperturada para realizar pagos, contacte al administrador de tesorería para dar apertura a la caja.";
        public const string Error_No_EsCajero = "El usuario no posee privilegios de cajero, por lo tanto no puede acceder a esta pantalla aunque tenga permisos en su perfil.";
        public const string Error_No_MismoCajero = "Los recibos solo pueden ser generados por un único cajero durante la caja esté aperturada.";
        public const string Error_OP_Anulada = "La orden de pago ha sido ANULADA, no es posible generar el recibo, por favor actualice los registros de órdenes de pago y vuelva a intentarlo";
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
            arqueo_caja = 2,
            informe_general_ingresos = 3
        }

    }
}
