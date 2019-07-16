using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Interface;
using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.ViewModel
{
    class TesoreriaViewModel : ISecurity
    {
        private SIFOPEntities db = new SIFOPEntities();
        private Pantalla pantalla;
        List<vw_RecintosRH> r;
        private SecurityViewModel seguridad;
        public TesoreriaViewModel(Pantalla pantalla)
        {
            seguridad = new SecurityViewModel();
            r = seguridad.RecintosPermiso(pantalla);

            this.pantalla = pantalla;
        }

        public TesoreriaViewModel()
        {

        }

        #region Caja

        public List<CajaSon> FindAllCajas()
        {

            return db.Caja.Where(w => w.regAnulado == false).OrderByDescending(o => o.IdCaja).Take(clsConfiguration.Actual().TopRow).ToList().Select(s => new CajaSon()
            {
                IdCaja = s.IdCaja,
                Nombre = s.Nombre,
                MAC = s.MAC,
                UsuarioCreacion = s.UsuarioCreacion,
                IdRecinto = s.IdRecinto,
                IdSerie = s.IdSerie,
                regAnulado = s.regAnulado,
                FechaCreacion = s.FechaCreacion,
                Recinto = clsSessionHelper.recintosMemory.Where(w => w.IdRecinto == s.IdRecinto).Select(a => a.Siglas).FirstOrDefault().ToString(),
            }).Where(b => r.Any(a => b.IdRecinto == a.IdRecinto)).ToList();
        }

        public string FindMacActual()
        {

            byte[] bytes = NetworkInterface.GetAllNetworkInterfaces().ToList().FirstOrDefault().GetPhysicalAddress().GetAddressBytes();

            string MAC = "";
            for (int i = 0; i < bytes.Length; i++)
            {

                MAC = MAC + "" + bytes[i].ToString("X2");

                if (i != bytes.Length - 1)
                {
                    MAC = MAC + "-";

                }
            }

            return MAC;
        }

        public void SaveUpdateCaja(Caja c)
        {
            var cajas = db.Caja.Where(w => (w.MAC == c.MAC || (w.Nombre == c.Nombre && w.IdRecinto == c.IdRecinto)) && w.regAnulado == false).ToList();

            Caja caja;
            if (c.IdCaja == 0)
            {
                if (cajas.Count() == 0)
                {
                    caja = new Caja();

                    AtributosCaja(caja, c);

                    caja.UsuarioCreacion = clsSessionHelper.usuario.Login;
                    caja.FechaCreacion = System.DateTime.Now;

                    db.Caja.Add(caja);
                }
                else
                {
                    ExcepcionCaja();
                }
            }
            else
            {
                if (cajas.Where(w => w.IdCaja != c.IdCaja).Count() == 0)
                {
                    caja = db.Caja.Find(c.IdCaja);

                    AtributosCaja(caja, c);

                    db.Entry(caja).State = System.Data.Entity.EntityState.Modified;

                }
                else
                {
                    ExcepcionCaja();
                }
            }
            db.SaveChanges();

        }

        /// <summary>
        /// Verifica que la caja tenga todas las aperturas arqueadas
        /// </summary>
        /// <param name="caja"></param>
        /// <returns> si todas las aperturas se encuentran arqueadas, False en caso contrario</returns>
        public bool VeriricarAperturasArquedas(Caja caja)
        {
            List<DetAperturaCaja> aperturas = db.DetAperturaCaja.Where(w => w.IdCaja == caja.IdCaja).ToList();
            var asd = db.Arqueo.ToList().Where(w => aperturas.Any(a => a.IdDetAperturaCaja == w.IdArqueoDetApertura && w.isFinalizado));
            return db.Arqueo.ToList().Where(w => aperturas.Any(a => a.IdDetAperturaCaja == w.IdArqueoDetApertura && w.isFinalizado)).Count() == aperturas.Count();
        }

        private void ExcepcionCaja()
        {
            throw new Exception("No es posible agregar una caja con el mismo nombre o la misma direccón MAC, por favor verifique la información. Es posible que el MAC haya sido utilizado para otro recinto de la Universidad.");
        }

        private void AtributosCaja(Caja caja, Caja c)
        {
            caja.Nombre = c.Nombre;
            caja.MAC = c.MAC;
            caja.IdRecinto = c.IdRecinto;
            caja.IdSerie = c.IdSerie;
        }

        public void EliminarCaja(CajaSon c)
        {
            Caja caja = db.Caja.Find(c.IdCaja);
            if (VeriricarAperturasArquedas(caja))
            {

            }
            caja.regAnulado = true;
            db.Entry(caja).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        #endregion

        #region Serie de Recibo
        public List<SerieRecibo> FindAllSeries()
        {
            return db.SerieRecibo.ToList();
        }

        public List<SerieRecibo> FindAddSeries(string IdSerie)
        {
            return db.SerieRecibo.Where(w => w.regAnulado == false && (!db.Caja.Any(c => c.IdSerie == w.IdSerie && c.regAnulado == false)) || w.IdSerie == IdSerie).ToList();
        }

        public void SaveSerie(SerieRecibo serie)
        {
            if (db.SerieRecibo.Where(w => w.IdSerie == serie.IdSerie).Count() == 0)
            {
                serie.UsuarioCreacion = clsSessionHelper.usuario.Login;
                serie.FechaCreacion = System.DateTime.Now;
                db.SerieRecibo.Add(serie);
                db.SaveChanges();
            }
            else
            {
                ExcepcionSerie();
            }
        }

        public void EliminarSerie(SerieRecibo c)
        {
            SerieRecibo serie = db.SerieRecibo.Find(c.IdSerie);
            serie.regAnulado = true;
            db.Entry(serie).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        private void ExcepcionSerie()
        {
            throw new Exception("No es posible registrar una seria que ya existe o haya existido en el sistema.");
        }

        #endregion

        #region Tarjeta

        public List<CiaTarjetaCredito> FindAllTarjetas()
        {
            return db.CiaTarjetaCredito.Where(w => w.RegAnulado == false).OrderBy(o => o.Nombre).ToList();
        }

        public void SaveUpdateTarjeta(CiaTarjetaCredito t)
        {

            CiaTarjetaCredito tarjeta;
            if (t.IdCiaTarjetaCredito == 0)
            {

                tarjeta = new CiaTarjetaCredito();
                tarjeta.Nombre = t.Nombre;
                tarjeta.Siglas = t.Siglas;

                tarjeta.LoginCreacion = clsSessionHelper.usuario.Login;

                db.CiaTarjetaCredito.Add(tarjeta);

            }
            else
            {

                tarjeta = db.CiaTarjetaCredito.Find(t.IdCiaTarjetaCredito);
                tarjeta.Nombre = t.Nombre;
                tarjeta.Siglas = t.Siglas;

                db.Entry(tarjeta).State = System.Data.Entity.EntityState.Modified;

            }
            db.SaveChanges();
        }

        public void EliminarTarjeta(CiaTarjetaCredito t)
        {
            CiaTarjetaCredito tarjeta = db.CiaTarjetaCredito.Find(t.IdCiaTarjetaCredito);
            tarjeta.RegAnulado = true;
            db.Entry(tarjeta).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
        #endregion

        #region Banco

        public List<Banco> FindAllBancos()
        {
            return db.Banco.Where(w => w.RegAnulado == false).OrderBy(o => o.Nombre).ToList();
        }

        public void SaveUpdateBanco(Banco b)
        {
            Banco banco;
            if (b.IdBanco == 0)
            {

                banco = new Banco();
                banco.Nombre = b.Nombre;
                banco.Siglas = b.Siglas;

                banco.LoginCreacion = clsSessionHelper.usuario.Login;

                db.Banco.Add(banco);

            }
            else
            {

                banco = db.Banco.Find(b.IdBanco);
                banco.Nombre = b.Nombre;
                banco.Siglas = b.Siglas;

                db.Entry(banco).State = System.Data.Entity.EntityState.Modified;

            }
            db.SaveChanges();
        }

        public void EliminarBanco(Banco b)
        {
            Banco banco = db.Banco.Find(b.IdBanco);
            banco.RegAnulado = true;
            db.Entry(banco).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
        #endregion

        #region Forma de Pago

        public List<FormaPago> FindAllFormasPago()
        {
            return db.FormaPago.Where(w => w.regAnulado == false).OrderBy(o => o.FormaPago1).ToList();
        }

        public void SaveUpdateFormaPago(FormaPago fp)
        {
            FormaPago formapago;
            if (fp.IdFormaPago == 0)
            {

                formapago = new FormaPago();
                formapago.FormaPago1 = fp.FormaPago1;
                formapago.FechaCreacion = System.DateTime.Now;
                formapago.UsuarioCreacion = clsSessionHelper.usuario.Login;

                db.FormaPago.Add(formapago);

            }
            else
            {

                formapago = db.FormaPago.Find(fp.IdFormaPago);
                formapago.FormaPago1 = fp.FormaPago1;

                db.Entry(formapago).State = System.Data.Entity.EntityState.Modified;

            }
            db.SaveChanges();
        }

        public void EliminarFormaPago(FormaPago fp)
        {
            FormaPago formapago = db.FormaPago.Find(fp.IdFormaPago);
            formapago.regAnulado = true;
            db.Entry(formapago).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        #endregion

        #region Fuentes de Financiamiento

        public List<FuenteFinanciamientoSon> FindAllFuentesFinanciamiento()
        {
            return db.FuenteFinanciamiento.Where(w => w.RegAnulado == false).OrderBy(o => o.Nombre).Select(s => new FuenteFinanciamientoSon()
            {
                IdFuenteFinanciamiento = s.IdFuenteFinanciamiento,
                Nombre = s.Nombre,
                Siglas = s.Siglas,
                Tiene_Ingreso = s.Tiene_Ingreso,
                Tiene_Egreso = s.Tiene_Egreso,
                IdFuente_SIPPSI = s.IdFuente_SIPPSI,
                LoginCreacion = s.LoginCreacion,
                RegAnulado = s.RegAnulado,
                FuenteSIPPSI = db.vw_FuentesSIPPSI.Where(a => a.IdFuenteFinanciamiento == s.IdFuente_SIPPSI).FirstOrDefault().Nombre
            }).ToList();
        }

        public List<vw_FuentesSIPPSI> FindAllFuentesSIPSSI()
        {
            return db.vw_FuentesSIPPSI.OrderBy(o => o.Nombre).ToList();
        }

        public void SaveUpdateFF(FuenteFinanciamiento ff)
        {

            FuenteFinanciamiento fuente;
            if (ff.IdFuenteFinanciamiento == 0)
            {

                fuente = new FuenteFinanciamiento();
                AtributosFF(fuente, ff);
                fuente.FechaCreacion = System.DateTime.Now;
                fuente.LoginCreacion = clsSessionHelper.usuario.Login;

                db.FuenteFinanciamiento.Add(fuente);

            }
            else
            {

                fuente = db.FuenteFinanciamiento.Find(ff.IdFuenteFinanciamiento);
                AtributosFF(fuente, ff);

                db.Entry(fuente).State = System.Data.Entity.EntityState.Modified;

            }
            db.SaveChanges();
        }

        public void EliminarFF(FuenteFinanciamiento ff)
        {
            FuenteFinanciamiento fuente = db.FuenteFinanciamiento.Find(ff.IdFuenteFinanciamiento);
            fuente.RegAnulado = true;
            db.Entry(fuente).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        private void AtributosFF(FuenteFinanciamiento fuente, FuenteFinanciamiento ff)
        {
            fuente.Nombre = ff.Nombre;
            fuente.Siglas = ff.Siglas;
            fuente.Tiene_Ingreso = ff.Tiene_Ingreso;
            fuente.Tiene_Egreso = ff.Tiene_Egreso;
            fuente.IdFuente_SIPPSI = ff.IdFuente_SIPPSI;
        }

        #endregion

        #region Moneda

        public List<Moneda> FindAllMonedas()
        {
            return db.Moneda.Where(w => w.regAnulado == false).OrderBy(o => o.Moneda1).ToList();
        }

        public void SaveUpdateMoneda(Moneda m)
        {
            Moneda moneda;
            if (m.IdMoneda == 0)
            {

                moneda = new Moneda();
                AtributosMoneda(moneda, m);

                moneda.FechaCreacion = System.DateTime.Now;
                moneda.UsuarioCreacion = clsSessionHelper.usuario.Login;

                db.Moneda.Add(moneda);

            }
            else
            {

                moneda = db.Moneda.Find(m.IdMoneda);
                AtributosMoneda(moneda, m);

                db.Entry(moneda).State = System.Data.Entity.EntityState.Modified;

            }
            db.SaveChanges();
        }

        public void EliminarMoneda(Moneda m)
        {
            Moneda moneda = db.Moneda.Find(m.IdMoneda);
            moneda.regAnulado = true;
            db.Entry(moneda).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
        private void AtributosMoneda(Moneda moneda, Moneda m)
        {
            moneda.Moneda1 = m.Moneda1;
            moneda.Simbolo = m.Simbolo;
            moneda.WebService = m.WebService;
        }

        #endregion

        #region Identificacion Agente Externo

        public List<IdentificacionAgenteExterno> FindAllIdentifiaciones()
        {
            return db.IdentificacionAgenteExterno.Where(w => w.regAnulado == false).OrderBy(o => o.Identificacion).ToList();
        }

        public void SaveUpdateIdentificacion(IdentificacionAgenteExterno i)
        {
            IdentificacionAgenteExterno identificacion;
            if (i.IdIdentificacion == 0)
            {

                identificacion = new IdentificacionAgenteExterno();
                AtributosIdentificacion(identificacion, i);

                identificacion.FechaCreacion = System.DateTime.Now;
                identificacion.UsuarioCreacion = clsSessionHelper.usuario.Login;

                db.IdentificacionAgenteExterno.Add(identificacion);

            }
            else
            {

                identificacion = db.IdentificacionAgenteExterno.Find(i.IdIdentificacion);
                AtributosIdentificacion(identificacion, i);

                db.Entry(identificacion).State = System.Data.Entity.EntityState.Modified;

            }
            db.SaveChanges();
        }

        public void EliminarIdentificacion(IdentificacionAgenteExterno i)
        {
            IdentificacionAgenteExterno identificacion = db.IdentificacionAgenteExterno.Find(i.IdIdentificacion);
            identificacion.regAnulado = true;
            db.Entry(identificacion).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        private void AtributosIdentificacion(IdentificacionAgenteExterno identificacion, IdentificacionAgenteExterno i)
        {
            identificacion.Identificacion = i.Identificacion;
            identificacion.MaxCaracteres = i.MaxCaracteres;
            identificacion.isMaxMin = i.isMaxMin;
        }

        #endregion

        #region Encabezado y Pie de Recibo

        public List<InfoReciboSon> FindAllInfoRecibos()
        {
            return db.InfoRecibo.Where(w => w.regAnulado == false).ToList().Select(s => new InfoReciboSon()
            {
                IdInfoRecibo = s.IdInfoRecibo,
                IdRecinto = s.IdRecinto,
                Encabezado = s.Encabezado,
                Pie = s.Pie,
                UsuarioCreacion = s.UsuarioCreacion,
                FechaCreacion = s.FechaCreacion,
                Recinto = clsSessionHelper.recintosMemory.Where(w => w.IdRecinto == s.IdRecinto).Select(a => a.Siglas).FirstOrDefault().ToString(),
                regAnulado = s.regAnulado
            }).Where(b => r.Any(a => b.IdRecinto == a.IdRecinto)).ToList();
        }

        public void SaveInfoRecibo(InfoRecibo infoRecibo)
        {
            if (infoRecibo.IdInfoRecibo != 0)
            {
                InfoRecibo i = db.InfoRecibo.Find(infoRecibo.IdInfoRecibo);
                i.regAnulado = true;
                db.Entry(i).State = System.Data.Entity.EntityState.Modified;
                infoRecibo.IdInfoRecibo = 0;
            }

            infoRecibo.UsuarioCreacion = clsSessionHelper.usuario.Login;
            infoRecibo.FechaCreacion = System.DateTime.Now;
            db.InfoRecibo.Add(infoRecibo);

            db.SaveChanges();
        }

        public List<vw_RecintosRH> RecintosInfo(string PermisoName)
        {
            var recintos = seguridad.RecintosPermiso(pantalla, PermisoName).ToList();

            var b = db.InfoRecibo.Where(w => w.regAnulado == false).Select(s => s.IdRecinto).ToList();

            return recintos.Where(w => b.All(c => c != w.IdRecinto)).ToList();

        }


        #endregion
        public List<vw_RecintosRH> Recintos(string PermisoName)
        {
            return seguridad.RecintosPermiso(pantalla, PermisoName);
        }

        public bool Authorize_Recinto(string PermisoName, int IdRecinto)
        {
            if (seguridad.Authorize(pantalla, PermisoName, IdRecinto))
            {
                return true;
            }
            else
            {
                throw new AuthorizationException(PermisoName, IdRecinto);
            }
        }

        public bool Authorize(string PermisoName)
        {
            if (seguridad.Authorize(pantalla, PermisoName))
            {
                return true;
            }
            else
            {
                throw new AuthorizationException(PermisoName);
            }
        }

    }
    public class CajaSon : Caja, ICloneable
    {
        public string Recinto { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class InfoReciboSon : InfoRecibo, ICloneable
    {
        public string Recinto { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class FuenteFinanciamientoSon : FuenteFinanciamiento
    {
        public string FuenteSIPPSI { get; set; }
    }
}
