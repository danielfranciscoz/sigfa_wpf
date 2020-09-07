using System;
using System.Data.Entity.Validation;
using System.Linq;

namespace PruebaWPF.Clases
{
    public class clsException
    {
        private Exception ex;
        public clsException(Exception ex)
        {
            this.ex = ex;
        }

        public String ErrorMessage()
        {
            //string message = "";

            //if (ex.InnerException != null)
            //{
            //    if (ex.InnerException.InnerException != null)
            //    {
            //        message = ex.InnerException.InnerException.Message;
            //    }
            //    else
            //    {
            //        message = ex.InnerException.Message;
            //    }
            //}
            //else
            //{
            //    message = ex.Message;
            //}
            //return message;

            return ErrorMessage(ex);
        }

        public string ErrorMessage(Exception e, string msgs = "")
        {
            if (e == null) return string.Empty;
            if (msgs == "") msgs = e.Message;
            if (e.InnerException != null)
                msgs += "\r\nMensaje Interno: " + ErrorMessage(e.InnerException);
            return msgs;
        }

        public String DataValidationErrors()
        {
            string mensaje = "";
            if (ex.GetType() == typeof(DbEntityValidationException))
            {
                DbEntityValidationException errors = (DbEntityValidationException)ex;
                if (errors.EntityValidationErrors.Any())
                {
                    var asd = string.Join(", ", errors.EntityValidationErrors.Select(s => new
                    {
                        entidad = s.Entry.Entity.GetType().Name,
                        estado = s.Entry.State,
                        detalleerror = string.Join(":", s.ValidationErrors.Select(ss => new { campo = ss.PropertyName, error = ss.ErrorMessage }))
                    }));

                    foreach (var eve in errors.EntityValidationErrors)
                    {
                        mensaje = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            var wasi = string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                     ve.PropertyName, ve.ErrorMessage);
                        }
                    }

                    mensaje = asd;
                }
            }

            return mensaje;
        }
    }
}