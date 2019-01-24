using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Interface;
using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.ViewModel
{
    class ReciboViewModel : IGestiones<ReciboSon>
    {
        private SIFOPEntities db = new SIFOPEntities();
        private Pantalla pantalla;

        public ReciboViewModel() { }

        public ReciboViewModel(Pantalla pantalla)
        {
            this.pantalla = pantalla;
        }

        public Pantalla Pantalla(string UId)
        {
            return new PantallaViewModel().FindById(UId);
        }

        public void Eliminar(ReciboSon recibo)
        {
            throw new NotImplementedException();
        }

        public List<ReciboSon> FindAll()
        {

            List<Recibo1> recibo = db.Recibo1.Take(clsConfiguration.Actual().TopRow).ToList().Where(w => new SecurityViewModel().RecintosPermiso(pantalla).Any(a => w.Caja.IdRecinto == a.IdRecinto)).ToList();

            return UnirRecibos(recibo);
        }

        public List<ReciboSon> FindByText(string text)
        {

            if (!text.Equals(""))
            {
                string[] busqueda = text.Trim().Split(' ');
                List<Recibo1> recibo = db.Recibo1
                    .Where(w => busqueda.All(a => w.IdRecibo.ToString().Equals(a) || w.Serie.Contains(a)))
                    .ToList()
                    .Where(w => new SecurityViewModel().RecintosPermiso(pantalla).Any(a => w.Caja.IdRecinto == a.IdRecinto))
                    .ToList();
                return UnirRecibos(recibo);
            }
            else
            {
                return FindAll();
            }
        }

        public List<VariacionCambiariaSon> FindTipoCambio(ReciboSon recibo)
        {
            List<VariacionCambiariaSon> datos = new List<VariacionCambiariaSon>();
            var monedasDetalles = DetallesRecibo(recibo).Select(s => new VariacionCambiaria() { Moneda = s.ArancelPrecio.Moneda });
            var monedasPago = recibo.ReciboPago.Select(s => new VariacionCambiaria() { Moneda = s.Moneda });
            var monedas = monedasDetalles.Union(monedasPago).GroupBy(g => g.Moneda).Select(s => new VariacionCambiaria() { Moneda = s.Key }).ToList();

            foreach (var item in monedas)
            {
                VariacionCambiariaSon v = new VariacionCambiariaSon()
                {
                    Moneda = item.Moneda,
                    Valor = ObtenerTasaCambio(item.Moneda.IdMoneda, recibo.Fecha).Valor
                };
                datos.Add(v);
            }


            return datos;
        }

        private List<ReciboSon> UnirRecibos(List<Recibo1> recibo)
        {

            //La información se obtiene de la orden de pago, cuando está asociada al recibo
            var ListaConOrdenes = recibo.Where(w => w.IdOrdenPago != null).Select(r => new ReciboSon()
            {
                IdRecibo = r.IdRecibo,
                Serie = r.Serie,
                IdCaja = r.IdCaja,
                IdPeriodoEspecifico = r.IdPeriodoEspecifico,
                IdArea = r.OrdenPago.IdArea,
                IdFuenteFinanciamiento = r.IdFuenteFinanciamiento,
                IdTipoDeposito = r.OrdenPago.IdTipoDeposito,
                Identificador = r.OrdenPago.Identificador,
                Recibimos = r.OrdenPago.Recibimos,
                Fecha = r.Fecha,
                IdOrdenPago = r.OrdenPago.IdOrdenPago,
                UsuarioCreacion = r.UsuarioCreacion,
                regAnulado = r.regAnulado,
                IdInfoRecibo = r.IdInfoRecibo,
                Recinto = db.vw_RecintosRH.Where(w => w.IdRecinto == r.Caja.IdRecinto).Select(s1 => s1.Siglas).FirstOrDefault().ToString(),
                Area = db.vw_Areas.Where(w => w.codigo == r.OrdenPago.IdArea).Select(s1 => s1.descripcion).FirstOrDefault().ToString().ToUpper(),
                InfoRecibo = r.InfoRecibo,
                Caja = r.Caja,
                OrdenPago = r.OrdenPago,
                FuenteFinanciamiento = r.FuenteFinanciamiento,
                TipoDeposito = r.OrdenPago.TipoDeposito,
                ReciboPago = r.ReciboPago
            }).ToList();

            //La información se obtiene del detalle del recibo
            var listaSinOrdenes = recibo.Where(w => w.IdOrdenPago == null && w.regAnulado == false).Select(r => new ReciboSon()
            {
                IdRecibo = r.IdRecibo,
                Serie = r.Serie,
                IdCaja = r.IdCaja,
                IdPeriodoEspecifico = r.IdPeriodoEspecifico,
                IdArea = r.IdArea,
                IdFuenteFinanciamiento = r.IdFuenteFinanciamiento,
                IdTipoDeposito = r.IdTipoDeposito,
                Identificador = r.Identificador,
                Recibimos = r.Recibimos,
                Fecha = r.Fecha,
                IdOrdenPago = r.IdOrdenPago,
                UsuarioCreacion = r.UsuarioCreacion,
                regAnulado = r.regAnulado,
                IdInfoRecibo = r.IdInfoRecibo,
                Recinto = db.vw_RecintosRH.Where(w => w.IdRecinto == r.Caja.IdRecinto).Select(s1 => s1.Siglas).FirstOrDefault().ToString(),
                Area = db.vw_Areas.Where(w => w.codigo == r.IdArea).Select(s1 => s1.descripcion).FirstOrDefault().ToString().ToUpper(),
                InfoRecibo = r.InfoRecibo,
                Caja = r.Caja,
                OrdenPago = r.OrdenPago,
                FuenteFinanciamiento = r.FuenteFinanciamiento,
                TipoDeposito = r.TipoDeposito,
                ReciboPago = r.ReciboPago,
                ReciboDet = r.ReciboDet
            }).ToList();

            //La información se obtiene de la orden de pago, cuando el recibo está anulado
            var ListaAnuladosConOrdenes = recibo.Where(w => w.regAnulado == true && w.ReciboAnulado.IdOrdenPago != null).Select(r => new ReciboSon()
            {
                IdRecibo = r.IdRecibo,
                Serie = r.Serie,
                IdCaja = r.IdCaja,
                IdPeriodoEspecifico = r.IdPeriodoEspecifico,
                IdArea = r.ReciboAnulado.OrdenPago.IdArea,
                IdFuenteFinanciamiento = r.IdFuenteFinanciamiento,
                IdTipoDeposito = r.ReciboAnulado.OrdenPago.IdTipoDeposito,
                Identificador = r.ReciboAnulado.OrdenPago.Identificador,
                Recibimos = r.ReciboAnulado.OrdenPago.Recibimos,
                Fecha = r.Fecha,
                IdOrdenPago = r.ReciboAnulado.OrdenPago.IdOrdenPago,
                UsuarioCreacion = r.UsuarioCreacion,
                regAnulado = r.regAnulado,
                IdInfoRecibo = r.IdInfoRecibo,
                Recinto = db.vw_RecintosRH.Where(w => w.IdRecinto == r.Caja.IdRecinto).Select(s1 => s1.Siglas).FirstOrDefault().ToString(),
                Area = db.vw_Areas.Where(w => w.codigo == r.ReciboAnulado.OrdenPago.IdArea).Select(s1 => s1.descripcion).FirstOrDefault().ToString().ToUpper(),
                InfoRecibo = r.InfoRecibo,
                Caja = r.Caja,
                OrdenPago = r.ReciboAnulado.OrdenPago,
                FuenteFinanciamiento = r.FuenteFinanciamiento,
                TipoDeposito = r.ReciboAnulado.OrdenPago.TipoDeposito,
                ReciboPago = r.ReciboPago,
                ReciboAnulado = r.ReciboAnulado
            }).ToList();

            //La información se obtiene del detalle del recibo anulado
            var ListaAnuladosSinOrdenes = recibo.Where(w => w.regAnulado == true && w.ReciboAnulado.IdOrdenPago == null).Select(r => new ReciboSon()
            {
                IdRecibo = r.IdRecibo,
                Serie = r.Serie,
                IdCaja = r.IdCaja,
                IdPeriodoEspecifico = r.IdPeriodoEspecifico,
                IdArea = r.IdArea,
                IdFuenteFinanciamiento = r.IdFuenteFinanciamiento,
                IdTipoDeposito = r.IdTipoDeposito,
                Identificador = r.Identificador,
                Recibimos = r.Recibimos,
                Fecha = r.Fecha,
                IdOrdenPago = r.IdOrdenPago,
                UsuarioCreacion = r.UsuarioCreacion,
                regAnulado = r.regAnulado,
                IdInfoRecibo = r.IdInfoRecibo,
                Recinto = db.vw_RecintosRH.Where(w => w.IdRecinto == r.Caja.IdRecinto).Select(s1 => s1.Siglas).FirstOrDefault().ToString(),
                Area = db.vw_Areas.Where(w => w.codigo == r.IdArea).Select(s1 => s1.descripcion).FirstOrDefault().ToString().ToUpper(),
                InfoRecibo = r.InfoRecibo,
                Caja = r.Caja,
                OrdenPago = r.OrdenPago,
                FuenteFinanciamiento = r.FuenteFinanciamiento,
                TipoDeposito = r.TipoDeposito,
                ReciboPago = r.ReciboPago,
                ReciboDet = r.ReciboDet,
                ReciboAnulado = r.ReciboAnulado
            }).ToList();


            return ListaConOrdenes.Union(listaSinOrdenes).Union(ListaAnuladosConOrdenes).Union(ListaAnuladosSinOrdenes).OrderByDescending(o => o.IdRecibo).ThenBy(o => o.Serie).ToList();
        }

        public List<ArancelPrecio> ObtenerAranceles(string IdArea, int IdTipoDeposito)
        {
            //Obtengo los Ids de Arancel que estan asociadas tanto al área como al tipo de depósito seleccionado por el usuario
            var areas = db.ArancelArea.Where(w => w.IdArea.Equals(IdArea)).Select(s => s.IdArancel);
            var TipoDepositos = db.ArancelTipoDeposito.Where(w => w.IdTipoDeposito == IdTipoDeposito).Select(s => s.IdArancel);

            //Obtengo los Ids en donde se intersecta la información anteriormente obtenida
            var IdIntersect = TipoDepositos.Intersect(areas).ToList();

            //Finalmente con los Ids recuperados por la intersección se obtienen los aranceles.            
            return db.ArancelPrecio.Where(w => IdIntersect.Any(w1 => w.IdArancel == w1) && w.regAnulado == false && w.Arancel.regAnulado == false).ToList();
        }

        public double ConvertirDivisa(int MonedaConvertir, int MonedaFinal, double Monto)
        {
            double valor = 0;
            using (SIFOPEntities dbN = new SIFOPEntities())
            {
                var m = dbN.sp_ConvertirDivisas(MonedaConvertir, MonedaFinal, Monto).FirstOrDefault();

                valor = Double.Parse(m.ToString());
            }
            return valor;
        }

        public List<DetOrdenPagoSon> FindAllDetailsOrderPay(OrdenPago op)
        {

            return op.DetOrdenPagoArancel.Select(s => new DetOrdenPagoSon
            {
                IdDetalleOrdenPago = s.IdDetalleOrdenPago,
                IdOrdenPago = s.IdOrdenPago,
                Concepto = s.Concepto,
                Descuento = s.Descuento,
                regAnulado = s.regAnulado,
                ArancelPrecio = s.ArancelPrecio,
                PrecioVariable = s.PrecioVariable,
            }).Where(w => w.regAnulado == false).ToList();
        }

        public ReciboSon FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public List<ReciboSon> FindRecibo(int Id, string Serie)
        {

            List<ReciboSon> result = UnirRecibos(db.Recibo1.Where(w => w.IdRecibo == Id && w.Serie == Serie).ToList());          
            return result;
        }

        public List<DetReciboSon> DetallesRecibo(ReciboSon recibo)
        {
            List<DetReciboSon> detalles;
            if (recibo.IdOrdenPago == null)
            {
                detalles = recibo.ReciboDet.Select(s => new DetReciboSon()
                {
                    IdRecibo = s.IdRecibo,
                    Serie = s.Serie,
                    ArancelPrecio = s.ArancelPrecio,
                    Concepto = s.Concepto,
                    Monto = s.Monto,
                    Descuento = s.Descuento,
                }).Where(w => w.IdRecibo == recibo.IdRecibo && w.Serie == recibo.Serie).ToList();
            }
            else
            {
                detalles = FindAllDetailsOrderPay(recibo.OrdenPago).Select(
                    s => new DetReciboSon()
                    {
                        ArancelPrecio = s.ArancelPrecio,
                        Concepto = s.Concepto,
                        Monto = s.PrecioVariable,
                        Descuento = s.Descuento,
                    }).ToList();
            }

            return detalles;
        }

        public List<ReciboPagoSon> ReciboFormaPago(ReciboSon recibo)
        {
            return recibo.ReciboPago.Select(s => new ReciboPagoSon()
            {
                DetalleAdicional = ObtenerObjetoAdicional(s)[0],
                InfoAdicional = ObtenerObjetoAdicional(s)[1].ToString(),
                IdReciboPago =s.IdReciboPago,
                IdRecibo = s.IdRecibo,
                Serie = s.Serie,
                IdFormaPago = s.IdFormaPago,
                Monto = s.Monto,
                IdMoneda = s.IdMoneda,
                FechaCreacion = s.FechaCreacion,
                UsuarioCreacion = s.UsuarioCreacion,
                regAnulado = s.regAnulado,
                FormaPago = s.FormaPago,
                Moneda = s.Moneda

            }).ToList();
        }

        private object[] ObtenerObjetoAdicional(ReciboPago r)
        {
            Object[] o = new Object[2];
            switch (r.IdFormaPago)
            {
                case 2: //Cheque
                    ReciboPagoCheque rc = r.ReciboPagoCheque;

                    o[0] = rc;
                    o[1] = string.Format("{0}, Cuenta {1}, Cheque No.{2}", rc.Banco.Nombre, rc.Cuenta, rc.NumeroCK);
                    break;
                case 3: //Tarjeta
                    ReciboPagoTarjeta rt =r.ReciboPagoTarjeta;
                   
                    o[0] = rt;
                    o[1] = string.Format("{0}, Autorización {1}", rt.CiaTarjetaCredito.Nombre, rt.Autorizacion);
                    break;
                case 4: //Bono
                    ReciboPagoBono rb = r.ReciboPagoBono;

                    o[0] = rb;
                    o[1] = string.Format("Emitipo por {0}, Bono No.{1}", rb.Emisor, rb.Numero);
                    break;
                default:
                    o[0] = null;
                    o[1] = "";
                    break;
                    //Efectivo
            }

            return o;
        }

        public void Guardar(ReciboSon Obj)
        {
            throw new NotImplementedException();
        }

        public ReciboSon GenerarRecibo(ReciboSon recibo, OrdenPagoSon ordenPago, List<DetOrdenPagoSon> detalleRecibo, List<ReciboPagoSon> detallePago)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    List<ReciboDet> detalles = null;
                    OrdenPago orden;
                    Recibo1 roc = new Recibo1();

                    //Información general del recibo que debe ser almacenada
                    roc.IdRecibo = recibo.IdRecibo;
                    roc.Serie = recibo.Serie;
                    roc.IdCaja = recibo.IdCaja;
                    roc.IdPeriodoEspecifico = recibo.IdPeriodoEspecifico;
                    roc.IdFuenteFinanciamiento = recibo.IdFuenteFinanciamiento;
                    roc.Fecha = System.DateTime.Now;
                    roc.UsuarioCreacion = clsSessionHelper.usuario.Login;
                    roc.IdInfoRecibo = recibo.IdInfoRecibo;

                    if (ordenPago.IdOrdenPago == 0) // se crea el recibo sin una orden de pago
                    {
                        roc.IdArea = ordenPago.IdArea;
                        roc.IdTipoDeposito = ordenPago.IdTipoDeposito;
                        roc.Identificador = ordenPago.Identificador;
                        roc.Recibimos = ordenPago.Recibimos;
                        roc.IdOrdenPago = null;

                        detalles = new List<ReciboDet>(detalleRecibo.Select(s => new ReciboDet()
                        {
                            IdRecibo = roc.IdRecibo,
                            Serie = roc.Serie,
                            IdPrecioArancel = s.ArancelPrecio.IdArancelPrecio,
                            Concepto = s.Concepto,
                            Monto = s.PrecioVariable,
                            Descuento = s.Descuento,
                            UsuarioCreacion = clsSessionHelper.usuario.Login,
                            FechaCreacion = System.DateTime.Now
                        }));


                    }
                    else
                    {
                        roc.IdOrdenPago = recibo.IdOrdenPago;
                        orden = db.OrdenPago.Find(ordenPago.IdOrdenPago);
                        orden.CodRecibo = roc.IdRecibo + "-" + roc.Serie;
                        db.Entry(orden).State = System.Data.Entity.EntityState.Modified;
                    }

                    db.Recibo1.Add(roc);
                    if (detalles != null)
                    {
                        db.ReciboDet.AddRange(detalles);
                    }

                    foreach (ReciboPagoSon item in detallePago)
                    {
                       ReciboPago pago = new ReciboPago()
                        {
                            IdRecibo = roc.IdRecibo,
                            Serie = roc.Serie,
                            IdFormaPago = item.IdFormaPago,
                            Monto = item.Monto,
                            IdMoneda = item.IdMoneda,
                            FechaCreacion = System.DateTime.Now,
                            UsuarioCreacion = clsSessionHelper.usuario.Login
                        };

                        db.ReciboPago.Add(pago);
                        db.SaveChanges();

                        switch (item.IdFormaPago)
                        {
                            case 2: //Cheque
                                ReciboPagoCheque rc = (ReciboPagoCheque)item.DetalleAdicional;
                                rc.IdReciboPago = pago.IdReciboPago;

                                db.ReciboPagoCheque.Add(rc);
                                break;
                            case 3: //Tarjeta
                                ReciboPagoTarjeta rt = (ReciboPagoTarjeta)item.DetalleAdicional;
                                rt.IdReciboPago = pago.IdReciboPago;

                                db.ReciboPagoTarjeta.Add(rt);
                                break;
                            case 4: //Bono
                                ReciboPagoBono rb = (ReciboPagoBono)item.DetalleAdicional;
                                rb.IdReciboPago = pago.IdReciboPago;

                                db.ReciboPagoBono.Add(rb);
                                break;
                            default:
                                
                                break;
                                //Efectivo
                        }

                    }
                    //db.ReciboPago.AddRange(pagos);

                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return recibo;

        }

        public void AnularRecibo(ReciboAnulado reciboAnulado)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Recibo1 r = db.Recibo1.Find(reciboAnulado.IdRecibo, reciboAnulado.Serie);
                    r.regAnulado = true;
                    if (r.IdOrdenPago != null)
                    {
                        reciboAnulado.IdOrdenPago = r.IdOrdenPago;

                        OrdenPago o = db.OrdenPago.Find(r.IdOrdenPago);
                        o.CodRecibo = null;
                        db.Entry(o).State = System.Data.Entity.EntityState.Modified;

                        r.IdOrdenPago = null;
                    }
                    db.Entry(r).State = System.Data.Entity.EntityState.Modified;

                    reciboAnulado.UsuarioAnulacion = clsSessionHelper.usuario.Login;
                    reciboAnulado.FechaAnulacion = System.DateTime.Now;

                    db.ReciboAnulado.Add(reciboAnulado);

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

        public void Modificar(ReciboSon Obj)
        {
            throw new NotImplementedException();

        }


        /// <summary>
        /// Método para obtener la información general del recibo que no se encuentra relacionada y se obtiene en tiempo de ejecución
        /// Posición 0 = Consecutivo de recibo, 1=Serie (basado en la caja), 2=IdCaja (caja actual), 3=IdInfoRecibo (información del recibo en base al recinto de la caja)
        /// </summary>
        /// <returns>String[]</returns>
        public String[] ObtenerCodigoRecibo()
        {
            string MAC = new TesoreriaViewModel().FindMacActual();
            Caja serieCajero = db.Caja.Where(w1 => w1.MAC == MAC && w1.regAnulado == false).FirstOrDefault();

            var r = db.Recibo1.Where(w => w.Serie.Equals(serieCajero.IdSerie.ToString()));
            int Idrecibo;

            if (r.Count() > 0)
            {
                Idrecibo = r.Max(s => s.IdRecibo);
            }
            else {
                Idrecibo = 0;
            }


            InfoRecibo info = db.InfoRecibo.Where(w => w.IdRecinto == serieCajero.IdRecinto && w.regAnulado == false).FirstOrDefault();

            if (Idrecibo == 0)
            {
                try
                {
                    var id = db.Configuracion.Where(w => w.Llave == clsConfiguration.Llaves.Consecutivo_Recibo.ToString()).FirstOrDefault();
                    if (id == null)
                    {
                        Idrecibo = 0;
                    }
                    else
                    {
                        Idrecibo = int.Parse(id.Valor.ToString()) - 1;
                    }

                }
                catch (Exception)
                {
                    Idrecibo = 0;
                }
            }
            return new string[] { (Idrecibo + 1).ToString(), serieCajero.IdSerie, serieCajero.IdCaja.ToString(), info.IdInfoRecibo.ToString() };
        }

        public List<FuenteFinanciamiento> ObtenerFuentesFinanciamiento()
        {
            return db.FuenteFinanciamiento.Where(w => w.RegAnulado == false && w.Tiene_Ingreso).OrderBy(w => w.Nombre).ToList();
        }

        public List<Banco> ObtenerBancos()
        {
            return db.Banco.Where(w => w.RegAnulado == false).OrderBy(o=>o.Nombre).ToList();
        }

        public List<CiaTarjetaCredito> ObtenerTarjetas()
        {
            return db.CiaTarjetaCredito.Where(w => w.RegAnulado == false).OrderBy(o => o.Nombre).ToList();
        }

        public List<TipoDeposito> ObtenerTipoCuenta()
        {
            return db.TipoDeposito.OrderBy(w => w.Nombre).ToList();
        }

        public List<Moneda> ObtenerMonedas()
        {
            return db.Moneda.ToList();
        }

        public List<FormaPago> ObtenerFormasPago()
        {
            return db.FormaPago.ToList();
        }

        /// <summary>
        /// Obtiene la tasa cambiaria de una moneda en una fecha determinada, si la fecha es null se obtiene la tasa de cambio del día
        /// </summary>
        /// <param name="moneda"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        public VariacionCambiaria ObtenerTasaCambio(int moneda, DateTime? fecha)
        {
            var tc = db.fn_ObtenerTasaCambio(fecha, moneda).ToList().Select(a => new VariacionCambiaria() { IdMoneda = moneda, Valor = Decimal.Parse(a.Valor.ToString()), Fecha = DateTime.Parse(a.Fecha.ToString()) }).FirstOrDefault();

            if (tc is null)
            {
                tc = new VariacionCambiaria();
            }
            return tc;
        }

        public VariacionCambiaria ObtenerTasaCambioDia(int moneda)
        {
            return ObtenerTasaCambio(moneda, null);
        }

        public bool Autorice_Recinto(string PermisoName, int IdRecinto)
        {
            if (new SecurityViewModel().Autorize(pantalla, PermisoName, IdRecinto))
            {
                return true;
            }
            else
            {
                throw new AuthorizationException(PermisoName, IdRecinto);
            }
        }

        public bool Autorice(string PermisoName)
        {
            if (new SecurityViewModel().Autorize(pantalla, PermisoName))
            {
                return true;
            }
            else
            {
                throw new AuthorizationException(PermisoName);
            }
        }
    }

    public class ReciboSon : Recibo1
    {
        public string Recinto { get; set; }
        public string Area { get; set; }

    }

    public class DetReciboSon : ReciboDet
    {

        public decimal MontoVirtual => ArancelPrecio.Arancel.isPrecioVariable == true ? Monto : ArancelPrecio.Precio;
        public decimal Total => MontoVirtual - Descuento;

        //Estos campos son creados para el reporte
        public string Arancel => ArancelPrecio.Arancel.Nombre;
        public string SimboloMoneda => ArancelPrecio.Moneda.Simbolo;
    }

    public class DetOrdenPagoSon : DetOrdenPagoArancel
    {

        public decimal MontoVirtual => ArancelPrecio.Arancel.isPrecioVariable == true ? PrecioVariable : ArancelPrecio.Precio;
        public decimal Total => MontoVirtual - Descuento;


    }



    public class ReciboPagoSon : ReciboPago
    {
        public decimal TipoCambio => new ReciboViewModel().ObtenerTasaCambioDia(IdMoneda).Valor;

        //Estos campos son creados para el reporte
        public string FormaPagoRecibo => FormaPago.FormaPago1;
        public string SimboloMoneda => Moneda.Simbolo;
        public String InfoAdicional { get; set; }
        public Object DetalleAdicional { get; set; }
    }
}


