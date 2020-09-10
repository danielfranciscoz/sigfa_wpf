using Microsoft.Reporting.WinForms;
using PruebaWPF.Model;
using PruebaWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.Clases
{
    class Email
    {
        SharedViewModel s = new SharedViewModel();

        public async void SendRecibo(ReportViewer reportViewer, string filename, string name, string mailTo, string mailCC)
        {

            await SendPDF(
                  reportViewer,
                  "Recibo Oficial de Caja " + filename,
                  filename,
                  name,
                  mailTo,
                  mailCC,
                  "Confirmación de pago - UNIVERSIDAD NACIONAL DE INGENIERÍA",
                  clsConfiguration.Llaves.Email_Finanzas.ToString()
                  );
        }

        public async void SendCreatedUser(string name,string roles, string mailTo, string mailCC,Boolean ActivarOp,Boolean ActivarTesoreria) {

            string textOp = "<p style=\"font-weight:bold; padding: 0; margin: 0; \">Para acceder al sitio de Ordenes de pago. Se tendrá que redirigir a su navegador web y transcribir la siguiente dirección URL: <a href=\"https://op.uni.edu.ni/\">https://op.uni.edu.ni/</a> Acá deberá seleccionar ir a Office365 y digitar su correo institucional y contraseña</p>";
            string textTesoreria = "<p style=\"font - weight:bold; padding: 0; margin: 0; \">Para acceder al sistema de tesorería deberá descargarlo desde la URL <a href=\"http://si.uni.edu.ni/tesoreria/SIGFA-Tesorer%C3%ADa.application\">Sistema de Tesorería</a>, a continuación debe instalar el archivo descargado e iniciar sesión con sus credenciales de Office365</p>";

            List<String> lista = new List<string>();
            lista.Add(name);
            lista.Add(mailTo);
            lista.Add(roles);
            lista.Add(ActivarOp ? textOp : "");
            lista.Add(ActivarTesoreria ? textTesoreria : "");

            await Task.Run(() =>
            {
                SendMail(@"\\Resources\\html\\UsuarioCreado.html", 
                    lista, 
                    mailTo, 
                    mailCC, 
                    "Confimación de cuenta UNI",
                    clsConfiguration.Llaves.Email_Sistemas.ToString());
            });
            }

        public async void SendUpdatedUser(string name, string roles, string mailTo, string mailCC)
        {
            List<String> lista = new List<string>();
            lista.Add(name);
            lista.Add(roles);

            await Task.Run(() =>
            {
                SendMail(@"\\Resources\\html\\UsuarioEditado.html",
                    lista,
                    mailTo,
                    mailCC,
                    "Actualización de cuenta UNI",
                    clsConfiguration.Llaves.Email_Sistemas.ToString());
            });
        }

        private void SendMail(string archivoHtml, List<string> datos, string mailTo, string mailCC, string Asunto, string EmailKey)
        {
                try
                {
                    Object[] conf = CredencialesSMTP(EmailKey);
                    SmtpClient smtp = (SmtpClient)conf[0];
                    String userMail = conf[1].ToString();
                    String MailNotify = conf[2].ToString();

                    MailMessage message = new MailMessage();

                    message.From = new MailAddress(userMail);

                mailTo = eliminarPuntoComa(mailTo);
                    message.To.Add(mailTo);

                    if (!string.IsNullOrEmpty(mailCC))
                    {
                        message.CC.Add(mailCC);
                    }

                    message.Subject = Asunto;
                    message.IsBodyHtml = true;



                    string directory = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug\", "");
                    using (StreamReader reader = new StreamReader(directory + archivoHtml))
                    {

                    datos.Add(MailNotify);
                        StringBuilder html = new StringBuilder();
                        html.AppendFormat(reader.ReadToEnd(), datos.ToArray());

                        message.Body = html.ToString();

                        smtp.Send(message);

                    }

                }
                catch (Exception ex)
                {
                    s.SaveError(ex);
                }
       
        }

        private string eliminarPuntoComa(string mailTo)
        {
            if (mailTo.StartsWith(";"))
            {
                return eliminarPuntoComa(mailTo.Substring(1));
            }
            else
            {
            return     mailTo;
            }
        }

        private async Task SendPDF(ReportViewer reportViewer, string tittle, string filename, string name, string mailTo, string mailCC, string Asunto, string EmailKey)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            await Task.Run(() =>
            {
                try
                {
                    byte[] bytes = reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    Object[] conf = CredencialesSMTP(EmailKey);
                    SmtpClient smtp = (SmtpClient)conf[0];
                    String userMail = conf[1].ToString();
                    String MailNotify = conf[2].ToString();

                    MemoryStream memoryStream = new MemoryStream(bytes);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    MailMessage message = new MailMessage();
                    Attachment attachment = new Attachment(memoryStream, tittle + ".pdf");
                    message.Attachments.Add(attachment);

                    message.From = new MailAddress(userMail);

                    mailTo = eliminarPuntoComa(mailTo).Replace(";",",");
                    message.To.Add(mailTo);

                    if (!string.IsNullOrEmpty(mailCC))
                    {
                        message.CC.Add(mailCC);
                    }

                    message.Subject = Asunto;
                    message.IsBodyHtml = true;



                    string directory = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug\", "");
                    using (StreamReader reader = new StreamReader(directory + @"\\Resources\\html\\ReciboPago.html"))
                    {
                        StringBuilder html = new StringBuilder();
                        html.AppendFormat(reader.ReadToEnd(), filename, name, MailNotify);

                        message.Body = html.ToString();

                        smtp.Send(message);

                        memoryStream.Close();
                        memoryStream.Dispose();
                    }

                }
                catch (Exception ex)
                {
                    //Si ocurre un error lo capturo pero no lanzo la exepcion para no detener el proceso de creacion de recibo
                    s.SaveError(ex);
                }
            });
        }

        private Object[] CredencialesSMTP(string EmailKey)
        {
            Configuracion config = s.Configuracion(EmailKey);

            string[] datos = config.Valor.Split(';');
            string[] variables = new string[datos.Length];

            for (int i = 0; i < datos.Length; i++)
            {
                variables[i] = datos[i].Split(':')[1];
            }

            return new object[]{ new SmtpClient()
            {
                Host = variables[0],
                Port = int.Parse(variables[1]),
                EnableSsl = bool.Parse(variables[2]),
                Credentials = new System.Net.NetworkCredential(variables[3], variables[4])
            } ,
            variables[3],
            variables[5] //Email de notificación visible al usuario en el aviso de confidencialidad
            };
        }
    }
}
