using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Interface;
using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PruebaWPF.ViewModel
{
    class VariacionCambiariaViewModel : IGestiones<VariacionCambiariaSon>
    {
        private SIFOPEntities db = new SIFOPEntities();

        public void Eliminar(VariacionCambiariaSon variacion)
        {
            if (variacion.Fecha.Date < DateTime.Now.Date)
            {
                throw new Exception("No es posible eliminar la tasa cambiaria de una fecha pasada");
            }
            db.VariacionCambiaria.Remove(db.VariacionCambiaria.Find(variacion.IdVariacionCambiaria));
            db.SaveChanges();
        }

        public Pantalla Pantalla(string UId)
        {
            return new PantallaViewModel().FindById(UId);
        }

        public List<VariacionCambiariaSon> FindAll()
        {
            return db.VariacionCambiaria.Select(s => new VariacionCambiariaSon()
            {
                IdVariacionCambiaria = s.IdVariacionCambiaria,
                IdMoneda = s.IdMoneda,
                Moneda = s.Moneda,
                Fecha = s.Fecha,
                Valor = s.Valor,
                LoginCreacion = s.LoginCreacion,
                Usuario = s.Usuario,
                RegAnulado = s.RegAnulado
            })
            .Where(w => w.RegAnulado == false).OrderByDescending(a => a.Fecha).Take(clsConfiguration.Actual().TopRow).ToList();
        }

        public VariacionCambiariaSon FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public VariacionCambiariaSon GetTCDiaBCN(DateTime fecha, Moneda moneda)
        {

            double Tasa = 0;
            if (moneda.IdMoneda == 1)
            {
                Tasa = 1.0000;
            }
            else
            {
                Tasa = new wsTipoCambio.Tipo_Cambio_BCNSoapClient().RecuperaTC_Dia(fecha.Year, fecha.Month, fecha.Day);
            }

            return new VariacionCambiariaSon() { Fecha = fecha, IdMoneda = moneda.IdMoneda, Moneda = moneda, Valor = (Decimal)Tasa, LoginCreacion = clsSessionHelper.usuario.Login, RegAnulado = false };
        }

        public List<VariacionCambiariaSon> GetTCPeriodoBCN(int año, int mes, Moneda moneda)
        {
            //var o = new wsTipoCambio.Tipo_Cambio_BCNSoapClient().RecuperaTC_Mes(año, mes);
            return new wsTipoCambio.Tipo_Cambio_BCNSoapClient().RecuperaTC_Mes(año, mes).Descendants("Tc").Select(d => new VariacionCambiariaSon { Fecha = Convert.ToDateTime(d.Element("Fecha").Value), Valor = Convert.ToDecimal(d.Element("Valor").Value), LoginCreacion = clsSessionHelper.usuario.Login, RegAnulado = false, IdMoneda = moneda.IdMoneda, Moneda = moneda }).ToList();
        }

        public String GetTipoCambioBD()
        {
            String tipo = "";
            var cambios = db.VariacionCambiaria.Where(w => w.Fecha == System.DateTime.Today).Select(s => s.Moneda.Moneda1 + " " + s.Valor);

            if (cambios.Any())
            {
                tipo = string.Join(",", cambios.ToList());
            }
            else
            {
                tipo = "0.00";
            }

            return tipo;
        }

        public List<Moneda> ObtenerMonedas()
        {
            //Solo se obtienen las monedas que poseen servicio web asociado, esta información la obtengo por medio de una configuración
            return db.Moneda.Where(w => w.WebService && !w.regAnulado).ToList();
        }

        public void VariacionAutomatica()
        {
            //La verificacion se hará a partir del 25 de cada mes
            DateTime diaActual = System.DateTime.Now.Date;

            if (diaActual.Day >= 25 && diaActual.Day <= 31)
            {
                diaActual = diaActual.AddMonths(1);

                if (!db.VariacionCambiaria.Any(w => w.Fecha.Month == diaActual.Month && w.Fecha.Year == diaActual.Year && w.RegAnulado == false))
                {
                    GuardarAutoAllCoins(diaActual.Year, diaActual.Month);
                }
            }
            else
            {
                int diasMes = DateTime.DaysInMonth(diaActual.Year, diaActual.Month);

                //Verifico que todos los dias del mes se encuentren ingresados en el sistema
                if (db.VariacionCambiaria.Where(w => w.Fecha.Month == diaActual.Month && w.Fecha.Year == diaActual.Year && w.RegAnulado == false).Count() < diasMes)
                {
                    GuardarAutoAllCoins(diaActual.Year, diaActual.Month);
                }
            }
        }

        private void GuardarAutoAllCoins(int año, int mes)
        {
            foreach (Moneda moneda in ObtenerMonedas())
            {
                List<VariacionCambiariaSon> variaciones = GetTCPeriodoBCN(año, mes, moneda);
                Guardar(variaciones);
            }
        }

        public List<VariacionCambiariaSon> FindByText(string text)
        {

            if (!text.Equals(""))
            {

                //Este método retorna un error cuando se realiza la búsqueda demasiado rápido, todo es culpa de entity y los métodos asíncronos
                return db.VariacionCambiaria.Select(s => new VariacionCambiariaSon()
                {
                    IdVariacionCambiaria = s.IdVariacionCambiaria,
                    IdMoneda = s.IdMoneda,
                    Moneda = s.Moneda,
                    Fecha = s.Fecha,
                    Valor = s.Valor,
                    LoginCreacion = s.LoginCreacion,
                    Usuario = s.Usuario,
                    RegAnulado = s.RegAnulado
                })
                .ToList().Where(
                   w => (w.Fecha.Year.ToString() + "/" + w.Fecha.ToString("MM")).Contains(text)
                && w.RegAnulado == false).OrderByDescending(a => a.Fecha).ToList();

            }
            else
            {
                return FindAll();
            }

        }

        public void Guardar(VariacionCambiariaSon Obj)
        {
            db.VariacionCambiaria.Add((VariacionCambiaria)Obj);
            db.SaveChanges();
        }

        public int Guardar(List<VariacionCambiariaSon> lista)
        {
            var add = new List<VariacionCambiaria>(lista.Select(s => new VariacionCambiaria
            {
                IdVariacionCambiaria = s.IdVariacionCambiaria,
                IdMoneda = s.IdMoneda,
                Moneda = s.Moneda,
                Fecha = s.Fecha,
                Valor = s.Valor,
                LoginCreacion = s.LoginCreacion,
                Usuario = s.Usuario,
                RegAnulado = s.RegAnulado
            }).Where(w => db.VariacionCambiaria.All(a => a.Fecha != w.Fecha || a.IdMoneda != w.Moneda.IdMoneda)).ToList());
            if (add.Count > 0)
            {
                db.VariacionCambiaria.AddRange(add);
                db.SaveChanges();
            }
            return add.Count();
        }

        public void Modificar(VariacionCambiariaSon Obj)
        {
            throw new NotImplementedException();
        }

        public bool Authorize_Recinto(string PermisoName, int IdRecinto)
        {
            throw new NotImplementedException();
        }

        public bool Authorize(string PermisoName)
        {
            throw new NotImplementedException();
        }
    }

    public class VariacionCambiariaSon : VariacionCambiaria
    {


        //Estos campos son creados para el reporte
        public string SimboloMoneda => Moneda.Simbolo;
        public Boolean CanDelete => !(Fecha.Date < System.DateTime.Now.Date);
    }
}
