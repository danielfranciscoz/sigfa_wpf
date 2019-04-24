using PruebaWPF.Helper;
using PruebaWPF.Interface;
using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.ViewModel
{
    class ArqueoViewModel : IGestiones<Arqueo>
    {

        private SecurityViewModel seguridad;
        private Pantalla pantalla;
        SIFOPEntities db = new SIFOPEntities();

        public ArqueoViewModel(Pantalla pantalla)
        {
            seguridad = new SecurityViewModel();
            this.pantalla = pantalla;
        }

        public void Eliminar(Arqueo Obj)
        {
            throw new NotImplementedException();
        }

        #region Paso 1, creando el arqueo

        /// <summary>
        /// Detecta la apertura no arqueada por medio del MAC de la computadora, en la primera posición retorna la apertura, en la segunda posición el arqueo que no ha sido finalizado (si y solamente si existe algún arqueno no terminado)
        /// Siempre retorna la información en orden ascendente, es decir de la apertura mas antigua a la mas reciente.
        /// </summary>
        /// <returns>object[DetAperturaCaja,Arqueo]</returns>
        public object[] DetectarApertura()
        {
            DetAperturaCaja apertura = new DetAperturaCaja();
            Caja c = db.Caja.Where(w => w.MAC == clsSessionHelper.MACMemory && w.regAnulado == false).FirstOrDefault();

            if (c == null)
            {
                throw new Exception("No hemos podido detectar la caja a la cual pertenece este equipo, los arqueos solo se pueden realizar en los equipos destinados como cajas de la Universidad.");
            }

            List<DetAperturaCaja> aperturas = db.DetAperturaCaja.Where(w => w.IdCaja == c.IdCaja && w.FechaCierre != null).ToList();

            if (!aperturas.Any())
            {
                throw new Exception("Esta caja no posee cierres, por lo tanto no puede ser arqueada");
            }

            List<Arqueo> enProceso = aperturas.Where(w => w.Arqueo != null).Select(w => w.Arqueo).ToList();

            if (enProceso.Any())
            {
                if (enProceso.Where(w => w.isFinalizado == false).Any()) // El arqueo sin finalizar debe ser retornado hasta que se finalice
                {
                    Arqueo a = enProceso.Where(w => w.isFinalizado == false).First();

                    return new Object[] { a.DetAperturaCaja, a };
                }
            }

            List<DetAperturaCaja> noArqueadas = aperturas.Where(w => w.Arqueo == null).ToList();

            if (!noArqueadas.Any())
            {
                throw new Exception("Esta caja ya se encuentra arqueada");
            }

            return new Object[] { noArqueadas.First() };



        }

        public List<Arqueo> FindAll()
        {
            return db.Arqueo.ToList();
        }
        public void Guardar(Arqueo Obj)
        {
            if (db.Arqueo.Find(Obj.IdArqueoDetApertura) == null)//Valido que el arqueo no haya sido registrado
            {
                Obj.FechaInicioArqueo = System.DateTime.Now;
                Obj.UsuarioArqueador = clsSessionHelper.usuario.Login;
                //las observaciones, Fecha de Finalizacion del Arqueo y el cajero que entrega, son agregados hasta que se finalice el arqueo

                db.Arqueo.Add(Obj);
                db.SaveChanges();
            }
        }

        #endregion

        #region Paso 2, contabilizando recibos
        public Recibo1 ContabilizarRecibo(string codigo, DetAperturaCaja apertura)
        {
            Recibo1 r;
            if (codigo.Contains("-")) //valido el formato, que contenga el guión
            {
                String[] valores = codigo.Split('-');

                int recibo;

                if (int.TryParse(valores[0], out recibo)) //valido que la primera parte de la cadena corresponda al numero del recibo
                {
                    r = db.Recibo1.Find(recibo, valores[1]);

                    if (r != null) //valido que el recibo exista en la base de datos
                    {
                        if (r.IdDetAperturaCaja == apertura.IdDetAperturaCaja) //valido que el recibo pertenezca a la apertura que se esta arqueando
                        {
                            if (!db.ArqueoRecibo.Any(a => a.IdRecibo == r.IdRecibo && a.Serie == r.Serie)) //Valido que el recibo no haya sido agregado
                            {
                                ArqueoRecibo arqueo = new ArqueoRecibo();
                                arqueo.IdArqueo = apertura.IdDetAperturaCaja; //El id IdDetAperturaCaja es el mismo Id de arqueo porque tienen una relación de uno-uno
                                arqueo.IdRecibo = r.IdRecibo;
                                arqueo.Serie = r.Serie;
                                arqueo.FechaCreacion = System.DateTime.Now;

                                db.ArqueoRecibo.Add(arqueo);
                                db.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("El recibo ya se encuentra contabilizado.");
                            }
                        }
                        else
                        {
                            throw new Exception("El recibo no es parte del arqueo que se está realizando, los recibos pertenecientes a este arqueo se encuentran en el informe de cierre de caja.");
                        }
                    }
                    else
                    {
                        throw new Exception("El Recibo ingresado no existe, por favor verifique que el número y la serie sean correctos con respecto al documento impreso.");
                    }
                }
                else
                {
                    throw new Exception("El código de recibo ingresado no es válido, debe escribirlo de la siguiente manera: Número Guión Letra (No usar espacios es blanco).");
                }
            }
            else
            {
                throw new Exception("El código de recibo ingresado no es válido, por favor asegúrese de ingresar [No.Recibo]-[Serie]. No use corchetes y recuerde el guión de separación.");
            }

            return r;
        }

        public List<Recibo1> FindRecibosContabilizados(Arqueo arqueo)
        {
            return db.ArqueoRecibo.Where(w => w.IdArqueo == arqueo.IdArqueoDetApertura).Select(s => s.Recibo1).ToList();
        }
        #endregion

        #region Paso 3, conteo de efectivo

        public List<ArqueoEfectivoSon> FindConteoEfectivo(DetAperturaCaja apertura)
        {
            List<ArqueoEfectivoSon> resultados = (from denominacion in db.DenominacionMoneda
                                                  join efectivo in db.ArqueoEfectivo on new { moneda = denominacion.Moneda, denominacion = denominacion.Denominacion } equals new { moneda = efectivo.IdMoneda, denominacion = efectivo.Denominacion } into jointTable
                                                  from jointRecord in jointTable.DefaultIfEmpty()
                                                  select new ArqueoEfectivoSon()
                                                  {
                                                      IdArqueoEfectivo = jointRecord.IdArqueoEfectivo,
                                                      IdArqueo = jointRecord.IdArqueo,
                                                      Moneda = denominacion.Moneda1,
                                                      Denominacion = denominacion.Denominacion,
                                                      Cantidad = jointRecord.Cantidad
                                                  }).ToList();

            return resultados;
        }

        public void GuardarEfectivo(List<ArqueoEfectivoSon> efectivo, Arqueo arqueo)
        {

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    ArqueoEfectivo ae;
                    foreach (ArqueoEfectivoSon a in efectivo)
                    {
                        if (a.IdArqueoEfectivo == null)
                        {
                            if (a.Total != null && a.Total > 0)
                            {
                                ae = new ArqueoEfectivo
                                {
                                    IdArqueo = arqueo.IdArqueoDetApertura,
                                    IdMoneda = a.Moneda.IdMoneda,
                                    Denominacion = a.Denominacion,
                                    Cantidad = a.Cantidad.Value,
                                    FechaCreacion = System.DateTime.Now,
                                    UsuarioCreacion = clsSessionHelper.usuario.Login
                                };
                                db.ArqueoEfectivo.Add(ae);
                            }
                        }
                        else
                        {
                            ae = db.ArqueoEfectivo.Find(a.IdArqueoEfectivo);
                            if (a.Cantidad == 0)
                            {
                                db.Entry(ae).State = System.Data.Entity.EntityState.Deleted;
                            }
                            else
                            {
                                if (ae.Cantidad != a.Cantidad)
                                {
                                    ae.Cantidad = a.Cantidad.Value;
                                    db.Entry(ae).State = System.Data.Entity.EntityState.Modified;

                                }
                            }
                        }
                    }

                    db.SaveChanges();
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

        }

        #endregion

        #region Paso 4, Conteo de Documentos de Efectivo

        public List<DocumentosEfectivo> FindDocumentosEfectivo(DetAperturaCaja apertura)
        {
            List<DocumentosEfectivo> bonos = db.ReciboPago.Where(w => w.Recibo1.IdDetAperturaCaja == apertura.IdDetAperturaCaja)
                .Select(s => new DocumentosEfectivo { NoDocumento = s.ReciboPagoBono.Numero, Observacion = "Número de Bono" }).ToList();

            List<DocumentosEfectivo> cheque = db.ReciboPago.Where(w => w.Recibo1.IdDetAperturaCaja == apertura.IdDetAperturaCaja)
        .Select(s => new DocumentosEfectivo { NoDocumento = s.ReciboPagoCheque.NumeroCK.ToString(), Observacion = "Número de Cheque" }).ToList();

            List<DocumentosEfectivo> deposito = db.ReciboPago.Where(w => w.Recibo1.IdDetAperturaCaja == apertura.IdDetAperturaCaja)
        .Select(s => new DocumentosEfectivo { NoDocumento = s.ReciboPagoDeposito.Transaccion, Observacion = "Número de Transacción de Minuta o Transferencia" }).ToList();

            List<DocumentosEfectivo> tarjeta = db.ReciboPago.Where(w => w.Recibo1.IdDetAperturaCaja == apertura.IdDetAperturaCaja)
        .Select(s => new DocumentosEfectivo { NoDocumento = s.ReciboPagoTarjeta.Autorizacion.ToString(), Observacion = "Número de Autorización" }).ToList();

            return bonos.Union(cheque).Union(deposito).Union(tarjeta).ToList();
        }
        #endregion

        #region Paso 5, Finalizando el arqueo

        public List<String> CajeroEntrega(DetAperturaCaja apertura)
        {
            return db.Recibo1.Where(w => w.IdDetAperturaCaja == apertura.IdDetAperturaCaja).Select(s => s.UsuarioCreacion).Distinct().ToList();
        }

        public void FinalizarArqueo(Arqueo Obj)
        {
            Arqueo arqueo = db.Arqueo.Find(Obj.IdArqueoDetApertura);
            arqueo.UsuarioArqueador = clsSessionHelper.usuario.Login;

            arqueo.FechaFinArqueo = System.DateTime.Now;
            arqueo.Observaciones = Obj.Observaciones;
            arqueo.CajeroEntrega = Obj.CajeroEntrega;
            arqueo.isFinalizado = true;

            db.Entry(arqueo).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

        }

        #endregion

        public Arqueo FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public List<Arqueo> FindByText(string text)
        {
            throw new NotImplementedException();
        }

        public void Modificar(Arqueo Obj)
        {
            throw new NotImplementedException();
        }

        public bool Autorice(string PermisoName)
        {
            throw new NotImplementedException();
        }

        public bool Autorice_Recinto(string PermisoName, int IdRecinto)
        {
            throw new NotImplementedException();
        }

    }

    public class DocumentosEfectivo : ReciboPago, INotifyPropertyChanged
    {
        public string NoDocumento { get; set; }
        public string Informacion { get; set; }

        private string observacion { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Observacion
        {
            get
            {
                return observacion;
            }
            set
            {
                if (value != observacion)
                {
                    observacion = value;
                    NotifyPropertyChanged();
                }
            }
        }

    }

    public class ArqueoEfectivoSon : ArqueoEfectivo, INotifyPropertyChanged
    {
        public new int? IdArqueoEfectivo { get; set; }
        public new int? IdArqueo { get; set; }
        private int? CantidadValue { get; set; }
        public double? Total => Cantidad == null ? (double?)null : Denominacion * double.Parse(Cantidad.Value.ToString());

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public new int? Cantidad
        {
            get
            {
                return CantidadValue;
            }
            set
            {
                if (value != CantidadValue)
                {
                    if (value >= 0)
                    {
                        CantidadValue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
        }
    }
}