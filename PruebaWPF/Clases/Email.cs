using Microsoft.Reporting.WinForms;
using PruebaWPF.Model;
using PruebaWPF.ViewModel;
using System;
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
                  "Confirmación de pago - UNIVERSIDAD NACIONAL DE INGENIERÍA"
                  );
        }

        private async Task SendPDF(ReportViewer reportViewer, string tittle, string filename, string name, string mailTo, string mailCC, string Asunto)
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
                    Object[] conf = CredencialesSMTP();
                    SmtpClient smtp = (SmtpClient)conf[0];
                    String userMail = conf[1].ToString();
                    String MailNotify = conf[2].ToString();

                    MemoryStream memoryStream = new MemoryStream(bytes);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    MailMessage message = new MailMessage();
                    Attachment attachment = new Attachment(memoryStream, tittle  + ".pdf");
                    message.Attachments.Add(attachment);

                    message.From = new MailAddress(userMail);
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

        private Object[] CredencialesSMTP()
        {
            Configuracion config = s.Configuracion(clsConfiguration.Llaves.Email.ToString());

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
