using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace PruebaWPF.Clases
{
    public class clsCallProcedure<T>
    {
        //Lo dejo protejido para que no sea visible desde los otros paquetes, este método no es usado sin embargo mantengo el código porque es funcional y funciona como una segunda opción
        protected DataTable CallDT(String ProcedureName, Dictionary<String, String> parametros)
        {
            SqlConnection conexion = null;
            try
            {
                string ConectionEF = ConfigurationManager.ConnectionStrings["SIFOPEntities"].ConnectionString;
                EntityConnectionStringBuilder entity = new EntityConnectionStringBuilder(ConectionEF);
                ConectionEF = entity.ProviderConnectionString;

                SqlConnectionStringBuilder conexionString = new SqlConnectionStringBuilder(ConectionEF);

                String TableName = "resultados";
                conexion = new SqlConnection(string.Format("data source={0};initial catalog={1};user id={2};password={3};MultipleActiveResultSets=True;App=EntityFramework", conexionString.DataSource, conexionString.InitialCatalog, conexionString.UserID, conexionString.Password));
                SqlCommand comando = new SqlCommand(ProcedureName, conexion);

                SqlParameter parametro;
                foreach (KeyValuePair<String, String> valor in parametros)
                {
                    parametro = new SqlParameter();
                    parametro.ParameterName = valor.Key;
                    parametro.Value = valor.Value;

                    comando.Parameters.Add(parametro);
                }
                comando.CommandType = CommandType.StoredProcedure;

                conexion.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(comando);
                DataSet tablaResult = new DataSet();
                adapter.Fill(tablaResult, TableName);

                conexion.Close();
                DataTable data = new DataTable();
                data = tablaResult.Tables[TableName];

                return data;
            }
            catch (Exception ex)
            {
                if (conexion.State == ConnectionState.Open)
                {
                    conexion.Close();
                }
                return null;
            }
        }


        public static List<T> GetFromQuery(SIFOPEntities db, string query, List<SqlParameter> parameters)
        {
            return new List<T>(db.Database.SqlQuery<T>(query, parameters.ToArray()));
        }

        public static List<T> GetFromQuery(SIFOPEntities db, string query)
        {
            return new List<T>(db.Database.SqlQuery<T>(query));
        }

    }


}
