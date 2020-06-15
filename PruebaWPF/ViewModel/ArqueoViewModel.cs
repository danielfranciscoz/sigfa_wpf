using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Interface;
using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PruebaWPF.ViewModel
{
    class ArqueoViewModel : IGestiones<Arqueo>
    {

        private SecurityViewModel seguridad;
        private Pantalla pantalla;
        SIFOPEntities db = new SIFOPEntities();

        public ArqueoViewModel(Pantalla pantalla)
        {
            seguridad = new SecurityViewModel(db);
            this.pantalla = pantalla;
        }

        public ArqueoViewModel()
        {
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

        #region Paso 2, contabilizando recibos y confirmando pagos
        public Recibo ContabilizarRecibo(string codigo, DetAperturaCaja apertura)
        {
            Recibo r;
            if (codigo.Contains("-")) //valido el formato, que contenga el guión
            {
                String[] valores = codigo.Split('-');

                int recibo;

                if (int.TryParse(valores[0], out recibo)) //valido que la primera parte de la cadena corresponda al numero del recibo
                {
                    r = db.Recibo.Find(recibo, valores[1]);

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

        public List<Recibo> FindRecibosContabilizados(Arqueo arqueo)
        {
            return db.ArqueoRecibo.Where(w => w.IdArqueo == arqueo.IdArqueoDetApertura).Select(s => s.Recibo).OrderBy(o => o.Serie).ThenBy(t => t.IdRecibo).ToList();
        }

        public List<FormaPago> ObtenerFormasPago()
        {
            return new ReciboViewModel().ObtenerFormasPago();
        }

        public List<Banco> ObtenerBancos()
        {
            return new ReciboViewModel().ObtenerBancos();
        }

        public List<CiaTarjetaCredito> ObtenerTarjetas()
        {
            return new ReciboViewModel().ObtenerTarjetas();
        }

        public void RectificarPago(ReciboPagoSon pago, ReciboPagoSon original, string observacion)
        {
            using (var transaccion = db.Database.BeginTransaction())
            {
                try
                {
                    int idRectificacion = 0;
                    ReciboPago rectificacion = new ReciboPago()
                    {
                        IdReciboPago = idRectificacion,
                        IdRecibo = original.IdRecibo,
                        Serie = original.Serie,
                        IdFormaPago = pago.IdFormaPago,
                        Monto = original.Monto,
                        IdMoneda = original.IdMoneda,
                        FechaCreacion = System.DateTime.Now,
                        UsuarioCreacion = clsSessionHelper.usuario.Login,

                    };

                    db.ReciboPago.Add(rectificacion);
                    //db.SaveChanges();

                    if (pago.ReciboPagoCheque != null)
                    {
                        ReciboPagoCheque rc = new ReciboPagoCheque()
                        {
                            IdReciboPago = pago.IdReciboPago,
                            IdBanco = pago.ReciboPagoCheque.IdBanco,
                            Cuenta = pago.ReciboPagoCheque.Cuenta,
                            NumeroCK = pago.ReciboPagoCheque.NumeroCK
                        };

                        db.ReciboPagoCheque.Add(rc);
                    }
                    else if (pago.ReciboPagoTarjeta != null)
                    {
                        ReciboPagoTarjeta rt = new ReciboPagoTarjeta()
                        {
                            IdReciboPago = pago.IdReciboPago,
                            IdTarjeta = pago.ReciboPagoTarjeta.IdTarjeta,
                            Tarjeta = pago.ReciboPagoTarjeta.Tarjeta,
                            Autorizacion = pago.ReciboPagoTarjeta.Autorizacion
                        };

                        db.ReciboPagoTarjeta.Add(rt);
                    }
                    else if (pago.ReciboPagoBono != null)
                    {
                        ReciboPagoBono rb = new ReciboPagoBono()
                        {
                            IdReciboPago = pago.IdReciboPago,
                            Numero = pago.ReciboPagoBono.Numero,
                            Emisor = pago.ReciboPagoBono.Emisor
                        };

                        db.ReciboPagoBono.Add(rb);
                    }
                    else if (pago.ReciboPagoDeposito != null)
                    {
                        ReciboPagoDeposito rd = new ReciboPagoDeposito()
                        {
                            IdReciboPago = pago.IdReciboPago,
                            Tipo = pago.ReciboPagoDeposito.Tipo,
                            Transaccion = pago.ReciboPagoDeposito.Transaccion,
                            Observacion = pago.ReciboPagoDeposito.Observacion
                        };

                        db.ReciboPagoDeposito.Add(rd);
                    }



                    RectificacionPago rp = new RectificacionPago()
                    {
                        IdReciboPago = original.IdReciboPago,
                        IdRectificacionPago = rectificacion.IdReciboPago,
                        Observacion = observacion,
                        UsuarioCreacion = clsSessionHelper.usuario.Login,
                        FechaCreacion = DateTime.Now
                    };

                    db.RectificacionPago.Add(rp);

                    ReciboPago reciboPago = db.ReciboPago.Find(original.IdReciboPago);

                    reciboPago.IdRectificacion = rectificacion.IdReciboPago;

                    db.Entry(reciboPago).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                    transaccion.Commit();
                }
                catch (Exception ex)
                {

                    transaccion.Rollback();
                    throw ex;
                }
            }

        }
        #endregion

        #region Paso 3, conteo de efectivo

        public List<ArqueoEfectivoSon> FindConteoEfectivo(int IdDetApertura, bool isArqueo)
        {
            List<ArqueoEfectivoSon> resultados = (from denominacion in db.DenominacionMoneda
                                                  join efectivo in db.ArqueoEfectivo on new { moneda = denominacion.Moneda, denominacion = denominacion.Denominacion, IdArqueo = IdDetApertura } equals new { moneda = efectivo.IdMoneda, denominacion = efectivo.Denominacion, efectivo.IdArqueo } into joinTable
                                                  from joinRecord in joinTable.DefaultIfEmpty()
                                                  where (joinRecord.Cantidad != null && isArqueo == false) || isArqueo
                                                  select new ArqueoEfectivoSon()
                                                  {
                                                      IdArqueoEfectivo = joinRecord.IdArqueoEfectivo,
                                                      IdArqueo = joinRecord.IdArqueo,
                                                      Moneda = denominacion.Moneda1,
                                                      Denominacion = denominacion.Denominacion,
                                                      Cantidad = joinRecord.Cantidad
                                                  }

                                                  ).ToList();

            if (resultados.All(a => a.Total == null) && !isArqueo)
            {
                return new List<ArqueoEfectivoSon>();
            }
            else
            {
                return resultados;
            }

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

        public List<Moneda> FindMonedas()
        {
            return db.Moneda.Where(w => w.regAnulado == false).ToList();
        }

        public List<DocumentosEfectivo> FindDocumentosEfectivo(DetAperturaCaja apertura)
        {
            List<DocumentosEfectivo> pagos = db.ReciboPago
                                        .Where(w => w.Recibo.IdDetAperturaCaja == apertura.IdDetAperturaCaja && w.FormaPago.isDoc && w.IdRectificacion == null && !w.regAnulado && !w.Recibo.regAnulado)
                                        .Select(s=>new DocumentosEfectivo() {
                                            IdRecibo = s.IdRecibo,
                                            Serie = s.Serie,
                                            Moneda = s.Moneda,
                                            FormaPago = s.FormaPago,
                                            Monto = s.Monto,
                                            NoDocumento = s.ReciboPagoTarjeta != null ? s.ReciboPagoTarjeta.Autorizacion.ToString() : s.ReciboPagoCheque != null ? s.ReciboPagoCheque.NumeroCK.ToString() : s.ReciboPagoDeposito != null ? s.ReciboPagoDeposito.Transaccion : s.ReciboPagoBono != null? s.ReciboPagoBono.Emisor:""
                                        })
                                        .ToList()
                                        
                                        ;

            return pagos;
        }

        public List<ArqueoNoEfectivoSon> FindDocumentosArqueados(int IdArqueoDetApertura)
        {
            return db.ArqueoNoEfectivo.Where(w => w.IdArqueo == IdArqueoDetApertura && w.FormaPago.isDoc).Select(s =>
              new ArqueoNoEfectivoSon()
              {
                  IdArqueoNoEfectivo = s.IdArqueoNoEfectivo,
                  IdArqueo = s.IdArqueo,
                  IdRecibo = s.IdRecibo,
                  Serie = s.Serie,
                  IdFormaPago = s.IdFormaPago,
                  FormaPago = s.FormaPago,
                  NoDocumento = s.NoDocumento,
                  Moneda = s.Moneda,
                  MontoFisico = s.MontoFisico,
                  MonedaFisica = s.MonedaFisica,
                  IdReciboPago = s.IdReciboPago,
                  FechaCreacion = s.FechaCreacion,
                  UsuarioCreacion = s.UsuarioCreacion
              }).ToList();
        }

        public List<FormaPago> FindFormasPagoDocumentos(int IdArqueoDetApertura)
        {
            return db.ReciboPago
                        .Where(w => w.Recibo.IdDetAperturaCaja == IdArqueoDetApertura && w.regAnulado == false && w.FormaPago.isDoc && w.FormaPago.Identificador != null)
                        .Select(s => s.FormaPago)
                        .Distinct()
                        .ToList();
        }

        public int? FindReciboPago(ArqueoNoEfectivoSon noefectivo)
        {
            ReciboPago pagos = db.ReciboPago.FirstOrDefault(w => w.IdRecibo == noefectivo.IdRecibo && w.Serie == noefectivo.Serie && w.IdFormaPago == noefectivo.IdFormaPago && w.IdMoneda == noefectivo.MonedaFisica && w.Monto == noefectivo.MontoFisico
             && (noefectivo.NoDocumento == w.ReciboPagoCheque.NumeroCK.ToString() ||
                    noefectivo.NoDocumento == w.ReciboPagoTarjeta.Autorizacion.ToString() ||
                    noefectivo.NoDocumento == w.ReciboPagoBono.Numero ||
                    noefectivo.NoDocumento == w.ReciboPagoDeposito.Transaccion)
            );
            return pagos?.IdReciboPago;
        }

        public int FindTotalDocumentos(int IdArqueoDetApertura)
        {
            return db.ReciboPago.Count(c => c.Recibo.IdDetAperturaCaja == IdArqueoDetApertura && c.FormaPago.isDoc && !c.regAnulado && !c.Recibo.regAnulado);
        }

        public List<ReciboPagoSon> FindConsolidadoDocumentos(int IdDetApertura)
        {
            List<ReciboPagoSon> consulta = db.ReciboPago
                                .Where(w => w.Recibo.IdDetAperturaCaja == IdDetApertura && w.FormaPago.isDoc && w.IdRectificacion == null && !w.regAnulado && !w.Recibo.regAnulado)
                                .GroupBy(g => new { g.FormaPago.FormaPago1, g.Moneda.Simbolo })
                                .ToList()
                                .Select(s => new ReciboPagoSon()
                                {
                                    FormaPago = new FormaPago() { FormaPago1 = s.Key.FormaPago1 },
                                    Moneda = new Moneda() { Simbolo = s.Key.Simbolo },
                                    Monto = s.Sum(ss => ss.Monto)
                                })
                                .OrderBy(o => o.FormaPago.FormaPago1).ThenBy(t => t.Moneda.Moneda1).ThenByDescending(t => t.Monto)
                                .ToList();

            return consulta;
        }

        /// <summary>
        /// Este metodo retorna la cantidad de pagos que se realizaron en la apertura, y la cantidad de pagos que se han confirmado en el arqueo
        /// [0] => cantidad de pagos
        /// [1] => cantidad de pagos confirmados
        /// </summary>
        /// <param name="IdArqueoDetApertura"></param>
        /// <returns></returns>
        public int[] FindTotalPagos(int IdArqueoDetApertura)
        {
            List<ReciboPago> pagos = db.ReciboPago.Where(c => c.Recibo.IdDetAperturaCaja == IdArqueoDetApertura && c.regAnulado == false).ToList();
            int totalPagos = pagos.Count();
            int confirmados = 0;//pagos.Count(c => c.ConfirmacionPago != null);
            return new int[] { totalPagos, confirmados };
        }


        public void GuardarNoEfectivo(List<ArqueoNoEfectivoSon> documentosMemory, Arqueo arqueo)
        {

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {

                    //Eliminando los que no estan 
                    List<ArqueoNoEfectivo> documentosBD = db.ArqueoNoEfectivo.Where(w => w.IdArqueo == arqueo.IdArqueoDetApertura).ToList();
                    if (!documentosMemory.Any())
                    {
                        if (documentosBD.Any())
                        {
                            db.ArqueoNoEfectivo.RemoveRange(documentosBD);
                        }
                    }
                    else
                    {
                        documentosBD = documentosBD.Where(w => !documentosMemory.Any(a => a.IdArqueoNoEfectivo == w.IdArqueoNoEfectivo && a.IdArqueoNoEfectivo > 0)).ToList();

                        if (documentosBD.Any())
                        {
                            db.ArqueoNoEfectivo.RemoveRange(documentosBD);
                        }
                    }

                    ArqueoNoEfectivo ae;
                    foreach (ArqueoNoEfectivoSon a in documentosMemory)
                    {
                        if (a.IdArqueoNoEfectivo == 0)
                        {
                            ae = new ArqueoNoEfectivo
                            {
                                IdArqueo = arqueo.IdArqueoDetApertura,
                                IdRecibo = a.IdRecibo,
                                Serie = a.Serie,
                                IdFormaPago = a.IdFormaPago,
                                IdReciboPago = a.IdReciboPago,
                                NoDocumento = a.NoDocumento,
                                MontoFisico = a.MontoFisico,
                                MonedaFisica = a.MonedaFisica,
                                FechaCreacion = System.DateTime.Now,
                                UsuarioCreacion = clsSessionHelper.usuario.Login
                            };
                            db.ArqueoNoEfectivo.Add(ae);
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

        #region Paso 5, Finalizando el arqueo

        public List<String> CajeroEntrega(DetAperturaCaja apertura)
        {
            return db.Recibo.Where(w => w.IdDetAperturaCaja == apertura.IdDetAperturaCaja).Select(s => s.UsuarioCreacion).Distinct().ToList();
        }

        public List<fn_TotalesArqueo_Result>[] SaldoTotalArqueo(DetAperturaCaja apertura)
        {
            List<fn_TotalesArqueo_Result>[] total = new List<fn_TotalesArqueo_Result>[2];
            total[0] = db.fn_TotalesArqueo(apertura.IdDetAperturaCaja, false).ToList();
            //     total[1] = db.fn_TotalesArqueo(apertura.IdDetAperturaCaja, true).ToList();

            return total;
        }

        public List<Rectificaciones> FindRectificaciones(int apertura)
        {
            var consulta = db.ReciboPago
                .OrderBy(f => f.RectificacionPago.FirstOrDefault().FechaCreacion)
                .Where(w => w.IdRectificacion != null && w.Recibo.IdDetAperturaCaja == apertura)
                .ToList()
                .Select(s => new Rectificaciones(
                        new ReciboPagoSon()
                        {
                            IdRecibo = s.IdRecibo,
                            Serie = s.Serie,
                            Moneda = s.Moneda,
                            Monto = s.Monto,
                            FormaPago = s.ReciboPago2.FormaPago,
                            RectificacionPago = s.RectificacionPago,
                            ReciboPagoBono = s.ReciboPago2.ReciboPagoBono,
                            ReciboPagoTarjeta = s.ReciboPago2.ReciboPagoTarjeta,
                            ReciboPagoCheque = s.ReciboPago2.ReciboPagoCheque,
                            ReciboPagoDeposito = s.ReciboPago2.ReciboPagoDeposito
                        },
                        new ReciboPagoSon()
                        {
                            FormaPago = s.FormaPago,
                            ReciboPagoBono = s.ReciboPagoBono,
                            ReciboPagoTarjeta = s.ReciboPagoTarjeta,
                            ReciboPagoCheque = s.ReciboPagoCheque,
                            ReciboPagoDeposito = s.ReciboPagoDeposito
                        })
           
                )
                .ToList();

            return consulta;
        }

        public List<ArqueoNoEfectivoSon> FindDocumentosNoEnlazados(int IdDetAperturaCaja)
        {
            List<ArqueoNoEfectivo> arqueo = db.ArqueoNoEfectivo.Where(w => w.IdArqueo == IdDetAperturaCaja && w.IdReciboPago != null).ToList();
            var apagos = db.ReciboPago.Where(w => w.Recibo.IdDetAperturaCaja == IdDetAperturaCaja && w.FormaPago.isDoc).ToList().Where(w => !arqueo.Exists(a => a.IdReciboPago == w.IdReciboPago)).ToList();

            List<ArqueoNoEfectivoSon> pagos = apagos
                .Select(s => new ArqueoNoEfectivoSon()
                {
                    IdReciboPago = s.IdReciboPago,
                    FormaPago = s.FormaPago,
                    NoDocumento = s.ReciboPagoTarjeta != null ? s.ReciboPagoTarjeta.Autorizacion.ToString() : s.ReciboPagoCheque != null ? s.ReciboPagoCheque.NumeroCK.ToString() : s.ReciboPagoDeposito != null ? s.ReciboPagoDeposito.Transaccion : s.ReciboPagoBono.Numero,
                    Moneda = s.Moneda,
                    MontoFisico = s.Monto
                })
                .ToList();

            return pagos;
        }

        public int IdEfectivo()
        {
            return int.Parse(db.Configuracion.Find(clsConfiguration.Llaves.Id_Efectivo.ToString()).Valor);
        }

        public void FinalizarArqueo(Arqueo Obj, List<fn_TotalesArqueo_Result> diferencias)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Arqueo arqueo = db.Arqueo.Find(Obj.IdArqueoDetApertura);

                    arqueo.Observaciones = Obj.Observaciones;
                    arqueo.CajeroEntrega = Obj.CajeroEntrega;
                    arqueo.isFinalizado = true;

                    db.Entry(arqueo).State = System.Data.Entity.EntityState.Modified;
                    DiferenciasArqueo da;

                    foreach (fn_TotalesArqueo_Result diferencia in diferencias)
                    {
                        if (diferencia.Diferencia.Value != 0)
                        {
                            da = new DiferenciasArqueo();

                            da.IdArqueo = arqueo.IdArqueoDetApertura;
                            da.IdFormaPago = diferencia.IdFormaPago == null ? diferencia.IdFormaPagoArqueo.Value : diferencia.IdFormaPago.Value;
                            da.IdMoneda = diferencia.IdMonedaDiferencia.Value;
                            da.Monto = diferencia.Diferencia.Value;

                            db.DiferenciasArqueo.Add(da);
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

        public void ValidarCajeroEntrega(String usuario, String password)
        {

        }
        #endregion

        #region Informe de Arqueo
        public List<DiferenciasArqueo> DiferenciasArqueo(int IdArqueoDetApertura)
        {
            return db.DiferenciasArqueo.Where(w => w.IdArqueo == IdArqueoDetApertura).ToList();
        }
        #endregion

        public ReciboSon ConvertToReciboSon(Recibo recibo)
        {
            if (recibo != null)
            {
                return new ReciboSon() { IdRecibo = recibo.IdRecibo, Serie = recibo.Serie, Fecha = recibo.Fecha, IdOrdenPago = recibo.IdOrdenPago, ReciboPago = recibo.ReciboPago };

            }
            else
            {
                return new ReciboSon();
            }
        }

        public List<VariacionCambiariaSon> FindTipoCambios(int IdArqueo)
        {

            Recibo recibo = db.ArqueoRecibo.FirstOrDefault(f => f.IdArqueo == IdArqueo)?.Recibo;

            if (recibo != null)
            {
                return FindTipoCambios(recibo.Fecha);
            }
            else
            {
                return new List<VariacionCambiariaSon>();
            }

        }

        public List<VariacionCambiariaSon> FindTipoCambios(Recibo agregado)
        {
            return new ReciboViewModel().FindTipoCambio(ConvertToReciboSon(agregado), null);
        }

        /// <summary>
        /// Encuentra el tipo de cambio para la apertura especificada
        /// </summary>
        /// <param name="apertura"></param>
        /// <returns></returns>
        public List<VariacionCambiariaSon> FindTipoCambios(AperturaCaja apertura)
        {
            return new ReciboViewModel().ObtenerTasaCambio(apertura.FechaApertura);
        }

        public List<VariacionCambiariaSon> FindTipoCambios(DateTime fecha)
        {
            return new ReciboViewModel().FindTipoCambio(fecha);
        }

        public Arqueo FindById(int Id)
        {
            return db.Arqueo.Find(Id);
        }

        public List<Arqueo> FindByText(string text)
        {
            if (!text.Equals(""))
            {
                return db.Arqueo.ToList().Where(
                   w => (w.FechaInicioArqueo.Year.ToString() + "/" + w.FechaInicioArqueo.ToString("MM")).Contains(text)
                ).OrderByDescending(a => a.IdArqueoDetApertura).ToList();

            }
            else
            {
                return FindAll();
            }
        }

        public void Modificar(Arqueo Obj)
        {
            throw new NotImplementedException();
        }

        public bool Authorize(string PermisoName)
        {
            throw new NotImplementedException();
        }

        public bool Authorize_Recinto(string PermisoName, int IdRecinto)
        {
            throw new NotImplementedException();
        }

    }


    public class Rectificaciones
    {
        private readonly ReciboPagoSon rectificacion;
        private readonly ReciboPagoSon original;
        public Rectificaciones(ReciboPagoSon rectificacion, ReciboPagoSon original)
        {
            this.rectificacion = rectificacion;
            this.original = original;
        }

        public string Recibo => string.Format("{0}-{1}", rectificacion.IdRecibo, rectificacion.Serie);

        public string FormaPagoOriginal => original.FormaPago.FormaPago1;
        public string DetalleOriginal => original.StringInfoAdicional;
        public string Moneda => rectificacion.Moneda.Simbolo;
        public decimal Monto => rectificacion.Monto;
        public string FormaPagoRectificacion => rectificacion.FormaPago.FormaPago1;
        public string DetalleRectificacion => rectificacion.StringInfoAdicional;
        public string Observacion => rectificacion.RectificacionPago.FirstOrDefault().Observacion;
        public string UsuarioCreacion => rectificacion.RectificacionPago.FirstOrDefault().UsuarioCreacion;
        public DateTime FechaCreacion => rectificacion.RectificacionPago.FirstOrDefault().FechaCreacion;


    }


    public class ArqueoNoEfectivoSon : ArqueoNoEfectivo
    {

        public string Recibo => string.Format("{0}-{1}", IdRecibo, Serie);
        public string ComentarioSistema => (IdReciboPago == null) ? "" : "Validado fisicamente";
        public String Origen => IdArqueo == 0 ? "Recibo" : "Arqueo";
        public string Documento => FormaPago.FormaPago1;
        public string SimboloMoneda => Moneda.Simbolo;
    }

    public class DocumentosEfectivo : ReciboPago //, INotifyPropertyChanged
    {
        //public int IdArqueoNoEfectivo { get; set; }
        public string Recibo => string.Format("{0}-{1}", IdRecibo, Serie);
        public string NoDocumento { get; set; }
        //public string Informacion { get; set; }
        //private string observacion { get; set; }
        //private bool isOkDocument { get; set; }
        public string Documento => FormaPago.FormaPago1;
        public string SimboloMoneda => Moneda.Simbolo;

        public event PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        //public string Observacion
        //{
        //    get
        //    {
        //        return observacion;
        //    }
        //    set
        //    {
        //        if (value != observacion)
        //        {
        //            observacion = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}

    }

    public class ArqueoEfectivoSon : ArqueoEfectivo, INotifyPropertyChanged
    {

        public new int? IdArqueoEfectivo { get; set; }
        public new int? IdArqueo { get; set; }
        private int? CantidadValue { get; set; }
        public double? Total => Cantidad == null ? (double?)null : Denominacion * double.Parse(Cantidad.Value.ToString());
        public string SimboloMoneda => Moneda.Simbolo;

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
