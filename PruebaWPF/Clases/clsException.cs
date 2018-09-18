using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Confortex.Clases
{
    public class clsException
    {
        private Exception ex;
        public clsException(Exception ex) {
            this.ex = ex;
        }

        public String ErrorMessage() {
            string message = "";
            if (ex.InnerException != null)
            {
                if (ex.InnerException.InnerException != null)
                {
                    message = ex.InnerException.InnerException.Message;
                }
                else {
                    message = ex.InnerException.Message;
                }
            }
            else {
                message = ex.Message;
            }
            return message;
        }
    }
}