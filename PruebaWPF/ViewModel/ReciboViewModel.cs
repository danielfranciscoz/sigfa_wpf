using Kinpos.comm;
using Kinpos.Dcl;
using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Interface;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;

namespace PruebaWPF.ViewModel
{
    class ReciboViewModel : IGestiones<ReciboSon>
    {
        private SIFOPEntities db = new SIFOPEntities();
        private Pantalla pantalla;
        IQueryable<vw_RecintosRH> r;
        private SecurityViewModel seguridad;

        public ReciboViewModel() { }

        public ReciboViewModel(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            seguridad = new SecurityViewModel(db);
            r = seguridad.RecintosPermiso(pantalla);
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
            bool usuarioCajero = new LoginViewModel().isCajero(clsSessionHelper.usuario.Login, db, validarOnlyCajero: true);

            IQueryable<Recibo> recibo;

            if (usuarioCajero)
            {
                recibo = db.Recibo.Take(clsConfiguration.Actual().TopRow).Where(w => r.Any(a => w.InfoRecibo.IdRecinto == a.IdRecinto) && w.UsuarioCreacion == clsSessionHelper.usuario.Login);

            }
            else
            {
                recibo = db.Recibo.Take(clsConfiguration.Actual().TopRow).Where(w => r.Any(a => w.InfoRecibo.IdRecinto == a.IdRecinto));

            }


            return UnirRecibos(recibo);
        }

        public List<ReciboSon> FindAllApertura(int IdDetApertura)
        {
            IQueryable<Recibo> recibo = db.Recibo.Where(w => w.IdDetAperturaCaja == IdDetApertura);

            return UnirRecibos(recibo);
        }

        public List<ReciboSon> FindByText(string text)
        {

            if (!text.Equals(""))
            {
                string[] busqueda = text.ToUpper().Trim().Split(' ');

                //IQueryable<Recibo> recibo = db.Recibo
                //    .Where(w => busqueda.All(a => w.IdRecibo == GetIfNumber(a) || w.Serie.Contains(a))
                //    && r.Any(a => w.InfoRecibo.IdRecinto == a.IdRecinto)
                //    );
                return FindAll().Where(w => busqueda.All(a => w.IdRecibo.ToString().Contains(a) || 
                                            w.Serie.ToUpper().Contains(a.ToUpper()) || 
                                            w.Identificador.ToUpper().Contains(a))).ToList();
            }
            else
            {
                return FindAll();
            }
        }

        private int GetIfNumber(string a)
        {
            int value = 0;
            int.TryParse(a, out value);

            return value;
        }

        public List<VariacionCambiariaSon> FindTipoCambio(DateTime fecha)
        {

            return db.VariacionCambiaria.Select(s => new VariacionCambiariaSon()
            {
                Moneda = s.Moneda,
                Valor = s.Valor,
                Fecha = s.Fecha,
            }).ToList().Where(w => DateTime.Compare(w.Fecha.Date, fecha.Date) == 0).ToList();
        }

        public List<VariacionCambiariaSon> FindTipoCambio(ReciboSon recibo, List<DetReciboSon> detalleR)
        {

            //Obtengo todos los tipos de cambio en base a la fecha del recibo
            List<VariacionCambiariaSon> cambios = db.Moneda.Where(w => w.regAnulado == false).ToList().Select(s => new VariacionCambiariaSon()
            {
                Moneda = new Moneda() { IdMoneda = s.IdMoneda, Moneda1 = s.Moneda1, Simbolo = s.Simbolo },
                Valor = ObtenerTasaCambio(s.IdMoneda, recibo.Fecha).Valor
            }).ToList();


            return cambios;
        }

        private List<ReciboSon> UnirRecibos(IQueryable<Recibo> recibo)
        {

            List<ReciboSon> ReciboCompleto = (from r in recibo
                                              join recintos in db.vw_RecintosRH on r.InfoRecibo.IdRecinto equals recintos.IdRecinto into ReciboRecinto
                                              from RecintoTable in ReciboRecinto
                                              join recibodatos in db.ReciboDatos on new { r.IdRecibo, r.Serie } equals new { recibodatos.IdRecibo, recibodatos.Serie } into ReciboDatoRecibo
                                              from ReciboDatoTable in ReciboDatoRecibo.DefaultIfEmpty()
                                              join orden in db.OrdenPago on r.IdOrdenPago equals orden.IdOrdenPago into OrdenRecibo
                                              from OrdenTable in OrdenRecibo.DefaultIfEmpty()
                                              join reciboanulado in db.ReciboAnulado on new { r.IdRecibo, r.Serie } equals new { reciboanulado.IdRecibo, reciboanulado.Serie } into ReciboAnuladoRecibo
                                              from ReciboAnuladoTable in ReciboAnuladoRecibo.DefaultIfEmpty()
                                              join ordenanulada in db.OrdenPago on ReciboAnuladoTable.IdOrdenPago equals ordenanulada.IdOrdenPago into OrdenAnulada
                                              from OrdenAnuladaTable in OrdenAnulada.DefaultIfEmpty()
                                              join area in db.vw_Areas on (ReciboDatoTable != null ? ReciboDatoTable.IdArea : OrdenTable != null ? OrdenTable.IdArea : OrdenAnuladaTable.IdArea) equals area.codigo into Areas
                                              from AreaTable in Areas
                                              orderby r.Fecha descending
                                              select new ReciboSon()
                                              {
                                                  IdRecibo = r.IdRecibo,
                                                  Serie = r.Serie,
                                                  IdDetAperturaCaja = r.IdDetAperturaCaja,
                                                  IdArea = ReciboDatoTable != null ? ReciboDatoTable.IdArea : OrdenTable != null ? OrdenTable.IdArea : OrdenAnuladaTable.IdArea,
                                                  IdFuenteFinanciamiento = r.IdFuenteFinanciamiento,
                                                  IdTipoDeposito = ReciboDatoTable != null ? ReciboDatoTable.IdTipoDeposito : OrdenTable != null ? OrdenTable.IdTipoDeposito : OrdenAnuladaTable.IdTipoDeposito,
                                                  Identificador = ReciboDatoTable != null ? ReciboDatoTable.Identificador : OrdenTable != null ? OrdenTable.Identificador : OrdenAnuladaTable.Identificador,
                                                  TextoIdentificador = ReciboDatoTable != null ? ReciboDatoTable.TextoIdentificador : OrdenTable != null ? OrdenTable.TextoIdentificador : OrdenAnuladaTable.TextoIdentificador,
                                                  TipoDeposito = ReciboDatoTable != null ? ReciboDatoTable.TipoDeposito : OrdenTable != null ? OrdenTable.TipoDeposito : OrdenAnuladaTable.TipoDeposito,
                                                  Recibimos = r.Recibimos,
                                                  Fecha = r.Fecha,
                                                  IdOrdenPago = r.IdOrdenPago,
                                                  regAnulado = r.regAnulado,
                                                  IdInfoRecibo = r.IdInfoRecibo,
                                                  InfoRecibo = r.InfoRecibo,
                                                  DetAperturaCaja = r.DetAperturaCaja,
                                                  OrdenPago = r.regAnulado ? ReciboAnuladoTable.IdOrdenPago.HasValue ? OrdenAnuladaTable : null : r.OrdenPago,
                                                  FuenteFinanciamiento = r.FuenteFinanciamiento,
                                                  UsuarioCreacion = r.UsuarioCreacion,
                                                  Recinto = RecintoTable.Siglas,
                                                  Area = AreaTable.descripcion.ToUpper(),
                                                  ReciboAnulado = ReciboAnuladoTable,
                                                  ReciboDatos = ReciboDatoTable
                                              }


                        ).ToList();



            return ReciboCompleto;
        }

        public List<Rectificaciones> FindRectificaciones(int idArqueoDetApertura)
        {
            return new ArqueoViewModel().FindRectificaciones(idArqueoDetApertura);
        }

        public List<Asiento> FindAsientoRecibo(ReciboSon recibo)
        {
            List<Asiento> asientos = (from a in db.Asiento
                                      join area in db.vw_Areas on a.IdArea equals area.codigo into Areas
                                      from AsientoTable in Areas.DefaultIfEmpty()
                                      orderby a.Naturaleza, a.Monto descending
                                      where a.IdRecibo == recibo.IdRecibo && a.Serie == recibo.Serie
                                      select new { a, AsientoTable }
                                      ).ToList().Select(s => new Asiento()
                                      {
                                          IdRecibo = s.a.IdRecibo,
                                          Serie = s.a.Serie,
                                          Area = s.AsientoTable?.descripcion,
                                          CuentaContable = s.a.CuentaContable,
                                          Naturaleza = s.a.Naturaleza,
                                          Monto = s.a.Monto
                                      }).ToList();

            return asientos;
        }


        public int getTipoArancelOrden(OrdenPagoSon orden)
        {
            DetOrdenPagoArancel det = orden.DetOrdenPagoArancel.First();
            return db.ArancelPrecio.First(f => f.IdArancelPrecio == det.IdPrecioArancel).ArancelArea.Arancel.IdTipoArancel;
        }

        public List<ArancelPrecio> ObtenerAranceles(string IdArea, int IdTipoDeposito, int IdTipoArancel, string criterio, int? IdPrematricula, int? IdMatricula)
        {

            List<ArancelSIRAValidate> arancelesSIRA = null;

            if (IdMatricula != null || IdPrematricula != null)
            {

                if (IdTipoArancel == IdMatricula || IdTipoArancel == IdPrematricula) //Si es matricula o prematricula hay que insertar el pago en el SIRA
                {
                    if (string.IsNullOrEmpty(criterio))
                    {
                        return new List<ArancelPrecio>();
                    }
                    arancelesSIRA = new List<ArancelSIRAValidate>();

                    List<SqlParameter> p = new List<SqlParameter>();
                    p.Add(new SqlParameter("@carnet", criterio));
                    //p.Add(new SqlParameter("@isMatricula", true));

                    arancelesSIRA = clsCallProcedure<ArancelSIRAValidate>.GetFromQuery(db, CatalogoConsultas.arancelesSIRA, p).ToList();

                    if (!arancelesSIRA.Any())
                    {
                        if (IdTipoArancel == IdMatricula)
                        {
                            throw new Exception("El estudiante no posee aranceles de Matricula por pagar");
                        }
                        else
                        {
                            throw new Exception("El candidato no posee aranceles de Prematricula por pagar");
                        }
                    }
                    else if (arancelesSIRA.First().Mensaje != null)
                    {
                        throw new Exception(arancelesSIRA.First().Mensaje);
                    }

                    var arancel = db.ArancelPrecio.ToList().Where(w => arancelesSIRA.Any(a => a.IdArancelPrecio == w.IdArancelPrecio)).OrderBy(o => o.ArancelArea.Arancel.Nombre);

                    return arancel.ToList();
                }

            }

            //Obtengo los Ids de Arancel que estan asociadas tanto al área como al tipo de depósito seleccionado por el usuario
            IEnumerable<int> areas;

            if (arancelesSIRA != null)
            {
                areas = db.ArancelArea.Where(w => w.IdArea == IdArea && arancelesSIRA.Any(a => a.IdArancel == w.IdArancel)).Select(s => s.IdArancel).ToArray();
            }
            else
            {
                areas = db.ArancelArea.Where(w => w.IdArea.Equals(IdArea) && w.regAnulado == false && w.Arancel.IdTipoArancel == IdTipoArancel && w.Arancel.regAnulado == false).Select(s => s.IdArancel).ToArray();
            }

            IEnumerable<int> TipoDepositos = db.ArancelTipoDeposito.Where(w => w.IdTipoDeposito == IdTipoDeposito && w.Arancel.IdTipoArancel == IdTipoArancel && w.Arancel.regAnulado == false).Select(s => s.IdArancel).ToArray();

            //Obtengo los Ids en donde se intersecta la información anteriormente obtenida
            var IdIntersect = TipoDepositos.Intersect(areas).ToList();

            //Finalmente con los Ids recuperados por la intersección se obtienen los aranceles.     
            var r = db.ArancelPrecio.Where(w => IdIntersect.Any(w1 => w.ArancelArea.IdArancel == w1 && w.ArancelArea.IdArea == IdArea && w.ArancelArea.regAnulado == false) && w.regAnulado == false).OrderBy(o => o.ArancelArea.Arancel.Nombre).ToList();

            return r;
        }


        public Exoneracion FindExoneracionArancel(string identificador, int idTipoDeposito, int idArancelPrecio)
        {
            Exoneracion exoneracion = db.Exoneracion.FirstOrDefault(f => f.Identificador == identificador && f.IdTipoDeposito == idTipoDeposito && f.IdArancelPrecio == idArancelPrecio && f.regAnulado == false && f.Autorizadopor != null && !f.DetOrdenPagoArancel.Any(a => a.OrdenPago.regAnulado == false) && !f.ReciboDet.Any(a => a.Recibo.regAnulado == false));
            return exoneracion;
        }
        public List<TipoArancel> ObtenerTipoArancel()
        {
            //Obtendo todos los aranceles que pueden pagar las áreas 
            var arancelT = db.ArancelArea.Where(w => w.regAnulado == false && w.Arancel.regAnulado == false).Select(s => s.Arancel).ToList();

            //Filtro por los diferentes tipos de arancel
            return arancelT.Select(s => s.TipoArancel).Distinct().ToList();
        }

        public double ConvertirDivisa(int? MonedaConvertir, int? MonedaFinal, decimal? Monto, DateTime? fecha = null)
        {
            double valor = 0;
            using (SIFOPEntities dbN = new SIFOPEntities())
            {
                if (Monto == null || MonedaConvertir == null || MonedaFinal == null)
                {
                    return 0;
                }
                var m = dbN.sp_ConvertirDivisas(MonedaConvertir, MonedaFinal, (Double)Monto, fecha).FirstOrDefault();

                valor = Double.Parse(m.ToString());
            }
            return valor;
        }

        public List<DetOrdenPagoSon> FindAllDetailsOrderPay(OrdenPago op)
        {

            return db.DetOrdenPagoArancel.Where(w => w.IdOrdenPago == op.IdOrdenPago && w.regAnulado == false).Select(s => new DetOrdenPagoSon
            {
                IdDetalleOrdenPago = s.IdDetalleOrdenPago,
                IdOrdenPago = s.IdOrdenPago,
                Concepto = s.Concepto,
                regAnulado = s.regAnulado,
                ArancelPrecio = s.ArancelPrecio,
                IdPrecioArancel = s.IdPrecioArancel,
                PrecioVariable = s.PrecioVariable,
                Exoneracion = s.Exoneracion,
                IdExoneracion = s.IdExoneracion
            }).ToList();
        }

        public ReciboSon FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public List<ReciboSon> FindRecibo(int Id, string Serie)
        {

            List<ReciboSon> result = UnirRecibos(db.Recibo.Where(w => w.IdRecibo == Id && w.Serie == Serie));
            return result;
        }

        public bool isAgenteExterno(int IdTipoDeposito)
        {
            return db.Configuracion.First(a => a.Llave == clsConfiguration.Llaves.IdAgenteExterno.ToString()).Valor == IdTipoDeposito.ToString();
        }

        public bool isPreMatricula(int IdTipoDeposito)
        {
            return db.Configuracion.First(a => a.Llave == clsConfiguration.Llaves.IdPrematricula.ToString()).Valor == IdTipoDeposito.ToString();
        }
        public bool isMatricula(int IdTipoDeposito)
        {
            return db.Configuracion.First(a => a.Llave == clsConfiguration.Llaves.IdMatricula.ToString()).Valor == IdTipoDeposito.ToString();
        }

        public List<DetReciboSon> DetallesRecibo(ReciboSon recibo)
        {
            List<DetReciboSon> detalles;
            if (recibo.OrdenPago == null)
            {
                detalles = db.ReciboDet.Select(s => new DetReciboSon()
                {
                    IdRecibo = s.IdRecibo,
                    Serie = s.Serie,
                    ArancelPrecio = s.ArancelPrecio,
                    IdPrecioArancel = s.IdPrecioArancel,
                    Concepto = s.Concepto,
                    Monto = s.Monto,
                    Exoneracion = s.Exoneracion,
                }).Where(w => w.IdRecibo == recibo.IdRecibo && w.Serie == recibo.Serie).ToList();
            }
            else
            {
                detalles = FindAllDetailsOrderPay(recibo.OrdenPago).Select(
                    s => new DetReciboSon()
                    {
                        ArancelPrecio = s.ArancelPrecio,
                        IdPrecioArancel = s.IdPrecioArancel,
                        Concepto = s.Concepto,
                        Monto = s.PrecioVariable,
                        Exoneracion = s.Exoneracion
                    }).ToList();
            }

            return detalles;
        }

        public List<ReciboPagoSon> ReciboFormaPago(ReciboSon recibo, bool incluirRectificacion = true)
        {

            var consulta = db.ReciboPago
                        .Where(w => w.IdRecibo == recibo.IdRecibo && w.Serie == recibo.Serie)

                        .Select(s => new ReciboPagoSon()
                        {
                            ReciboPagoBono = s.ReciboPagoBono,
                            ReciboPagoCheque = s.ReciboPagoCheque,
                            ReciboPagoDeposito = s.ReciboPagoDeposito,
                            ReciboPagoTarjeta = s.ReciboPagoTarjeta,
                            IdReciboPago = s.IdReciboPago,
                            IdRecibo = s.IdRecibo,
                            Serie = s.Serie,
                            IdFormaPago = s.IdFormaPago,
                            Monto = s.Monto,
                            IdMoneda = s.IdMoneda,
                            FechaCreacion = s.FechaCreacion,
                            UsuarioCreacion = s.UsuarioCreacion,
                            regAnulado = s.regAnulado,
                            FormaPago = s.FormaPago,
                            Moneda = s.Moneda,
                            IdRectificacion = s.IdRectificacion,
                            RectificacionPago1 = s.RectificacionPago1,
                            //     ConfirmacionPago = s.ConfirmacionPago,
                            //      isConfirmacion = s.isConfirmacion
                        });

            if (incluirRectificacion)
            {
                consulta = consulta.Where(w => w.IdRectificacion == null);
            }
            else
            {
                consulta = consulta.Where(w => w.RectificacionPago1 == null);
            }

            return consulta.ToList();
        }

        public List<ReciboPagoSon> ReciboFormaPagoConsolidado(ReciboSon recibo)
        {
            return db.ReciboPago.Where(w => w.IdRecibo == recibo.IdRecibo && w.Serie == recibo.Serie).ToList()
                .GroupBy(g => new { g.IdFormaPago, g.IdMoneda, g.FormaPago.FormaPago1, g.Moneda.Moneda1, g.Moneda.Simbolo })
                .Select(s => new ReciboPagoSon()
                {
                    //ReciboPagoBono = s.ReciboPagoBono,
                    //ReciboPagoCheque = s.ReciboPagoCheque,
                    //ReciboPagoDeposito = s.ReciboPagoDeposito,
                    //ReciboPagoTarjeta = s.ReciboPagoTarjeta,
                    //IdReciboPago = s.IdReciboPago,
                    //IdRecibo = s.IdRecibo,
                    //Serie = s.Serie,
                    //IdFormaPago = s.IdFormaPago,
                    Monto = s.Sum(sum => sum.Monto),
                    //IdMoneda = s.IdMoneda,
                    //FechaCreacion = s.FechaCreacion,
                    //UsuarioCreacion = s.UsuarioCreacion,
                    //regAnulado = s.regAnulado,
                    FormaPago = new FormaPago() { FormaPago1 = s.Key.FormaPago1 },
                    Moneda = new Moneda() { Moneda1 = s.Key.Moneda1, Simbolo = s.Key.Simbolo },
                    //ConfirmacionPago = s.ConfirmacionPago,
                    //isConfirmacion = s.isConfirmacion
                }).ToList();
        }



        public void Guardar(ReciboSon Obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Este método valida que en esta caja se haya configurado el COM Port para el pago automatico con tarjeta
        /// </summary>
        /// <returns></returns>
        public POSBanpro VerificarPOS()
        {

            return new TesoreriaViewModel().GetConfiguracionPOS();
        }

        /// <summary>
        /// Este método valida que no se agregue un numero de autorizacion que ya exista en la base de datos
        /// </summary>
        /// <param name="tarjeta"></param>
        /// <returns>False en caso que no exista, Exception en caso que ya exista</returns>
        public bool ValidarNumAutorizacion(ReciboPagoTarjeta tarjeta, List<ReciboPagoSon> pagos)
        {

            bool existeAutorizado = false;
            string razon = "";



            if (pagos.Count > 1)
            {

                existeAutorizado = pagos.Where(w => w.ObjInfoAdicional?.GetType() == typeof(ReciboPagoTarjeta)).Select(s => ((ReciboPagoTarjeta)s.ObjInfoAdicional)).Any(a => a.Autorizacion == tarjeta.Autorizacion && a.IdTarjeta == tarjeta.IdTarjeta);

                if (existeAutorizado)
                {
                    razon = "los pagos con tarjeta actualmente ingresados.";

                }
                else
                {
                    existeAutorizado = db.ReciboPagoTarjeta.Any(a => a.Autorizacion == tarjeta.Autorizacion);
                    razon = "los pagos con tarjeta almacenados";
                }
            }

            if (!existeAutorizado) //Si la autorizacion no se encuentra en memoria, la mando a buscar a la base de datos
            {

                existeAutorizado = db.ReciboPagoTarjeta.Any(a => a.Autorizacion == tarjeta.Autorizacion);
                razon = "pagos con tarjeta almacenados.";
            }


            if (existeAutorizado)
            {
                throw new Exception(string.Format("El número de autorización que intenta ingresar ya existe, especificamente entre {0}", razon));
            }

            return existeAutorizado;
        }

        public VoucherBanco GenerarVoucher(ReciboPagoSon reciboPago, int IdDetApertura, POSBanpro pos)
        {
            VoucherBanco voucher = new VoucherBanco();

            DCL_RS232 dcl = new DCL_RS232();

            dcl.StopBits = pos.StopBits;
            dcl.Baudrate = pos.Baudrate;
            dcl.ComPort = pos.ComPort;
            dcl.DataBits = pos.DataBits;
            dcl.Parity = pos.Parity;
            dcl.Timeout = pos.Timeout;
            dcl.ProcessBIN = pos.ProcessBIN;
            dcl.setAmount(reciboPago.Monto.ToString("00.00").Replace(".", "").Replace(",", ""));
            dcl.setCurrency(reciboPago.Moneda.CodigoISO);
            dcl.setPrintBeforeSendData(false);//En el codigo de ejemplo hace uso de un checkbox pero este siempre se encuentra en estado descheckeado, es decir False

            DCL_Result returnedData = dcl.Sale(null);

            if (returnedData != null)
            {
                String responseCode = returnedData.GetValue_ASCII2String(0);

                var a = Kinpos.Dcl.Util.Utilidad.GetDCLResponseText(responseCode);
                if (dcl.Success)
                {
                    if (responseCode.Equals("00"))
                    {
                        voucher.Autorizacion = returnedData.GetValue_ASCII2String(1);
                        voucher.Referencia = returnedData.GetValue_ASCII2String(2);
                        voucher.Factura = returnedData.GetValue_BCD2String(3);
                        voucher.Stan = returnedData.GetValue_BCD2String(4);
                        voucher.Tarjeta = returnedData.GetValue_ASCII2String(9).Replace("*", ""); //la tarjeta viene con asteriscos al inicio, eso no me interesa, por lo tanto los remuevo
                        voucher.IdDetApertura = IdDetApertura;
                        voucher.Monto = reciboPago.Monto;
                        voucher.IdMoneda = reciboPago.Moneda.IdMoneda;
                    }
                }
                else
                {
                    string reasonfail = Kinpos.Dcl.Util.Utilidad.GetDCLResponseText(dcl.DCL_ResponseCode);
                    Exception exception = new Exception(reasonfail);
                    new SharedViewModel().SaveError(exception);
                    throw exception;

                }
            }

            voucher.FechaCreacion = System.DateTime.Now;
            voucher.UsuarioCreacion = clsSessionHelper.usuario.Login;
            db.VoucherBanco.Add(voucher);

            try
            {
                db.SaveChanges();
            }
            catch (Exception exception)
            {
                AnularVoucher(voucher.Factura, pos);
                new SharedViewModel().SaveError(exception);
                throw exception;
            }

            return voucher;
        }

        public void AnularVoucher(ReciboPagoTarjeta recibopago, POSBanpro pos)
        {
            VoucherBanco original = db.VoucherBanco.FirstOrDefault(f => f.IdVoucherBanco == recibopago.IdVoucherBanco.Value);

            string anulaVoucher = AnularVoucher(original.Factura, pos);
            if (anulaVoucher != null)
            {
                original.StanAnulado = anulaVoucher;
                original.FechaAnulado = System.DateTime.Now;

                db.Entry(original).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            else
            {
                throw new Exception("No se pudo anular la transacción desde el POS, compruebe que el equipo se encuentra correctamente configurado.");
            }
        }

        private string AnularVoucher(string factura, POSBanpro pos)
        {
            string data = null;

            DCL_RS232 dcl = new DCL_RS232();
            dcl.StopBits = pos.StopBits;
            dcl.Baudrate = pos.Baudrate;
            dcl.ComPort = pos.ComPort;
            dcl.DataBits = pos.DataBits;
            dcl.Parity = pos.Parity;
            dcl.Timeout = pos.Timeout;
            dcl.ProcessBIN = pos.ProcessBIN;
            dcl.setInvoice(factura);
            dcl.setPrintBeforeSendData(false);
            DCL_Result returnedData = dcl.Void(null);
            //Anulo el vouvher y no lo guardo en la base de datos porque esta anulacion ocurre cuando el voucher no pudo guardarse en la base de datos

            if (returnedData != null)
            {

                String responseCode = returnedData.GetValue_ASCII2String(0);

                if (responseCode.Equals("00"))
                {
                    data = returnedData.GetValue_BCD2String(4); //retorno el stan que es lo unico que cambia
                }
            }

            return data;
        }

        public ReciboSon GenerarRecibo(TipoArancel tipoArancel, ReciboSon recibo, OrdenPagoSon ordenPago, List<DetOrdenPagoSon> detalleRecibo, List<ReciboPagoSon> detallePago, List<MonedaMonto> diferencias, int? IdMatricula, int? IdPrematricula)
        {
            //TODA LA LOGICA DEL ASIENTO CONTABLE SE ENCUENTRA COMENTAREADA, PARA ACTIVAR LOS ASIENTOS SOLO DEBEMOS QUITAR LOS COMENTARIOS
            bool? isSIRAPagado = null;
            bool isMatricula = true; //true:Matricula; false:Prematricula
            Recibo roc = new Recibo();
            ReciboDatos datos = null; // no creo la instancia porque no se si será usado
            //List<Asiento> LineasIngreso; //Esta lista contiene todos los movimientos correspondientes al ingreso, luego será unida a la lista que contiene la contrapartida del ingreso y la variacion cambiaria

            Exception exception = null;
            if (ValidarOrdenActiva(ordenPago))
            {

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<ReciboDet> detalles = null;
                        OrdenPago orden;
                        int idCordoba = int.Parse(db.Configuracion.First(f => f.Llave == clsConfiguration.Llaves.Moneda_Nacional.ToString()).Valor);


                        //Información general del recibo que debe ser almacenada
                        roc.IdRecibo = recibo.IdRecibo;
                        roc.Serie = recibo.Serie;
                        roc.IdDetAperturaCaja = recibo.IdDetAperturaCaja;
                        roc.IdFuenteFinanciamiento = recibo.IdFuenteFinanciamiento;
                        roc.Recibimos = recibo.Recibimos;
                        roc.Fecha = System.DateTime.Now;
                        roc.IdInfoRecibo = recibo.IdInfoRecibo;
                        roc.UsuarioCreacion = clsSessionHelper.usuario.Login;
                        roc.isRecibimosPorCuenta = recibo.isRecibimosPorCuenta;

                        if (ordenPago.IdOrdenPago == 0) // se crea el recibo sin una orden de pago
                        {
                            //TODO Unable to determine the principal end of the 'SIFOPModel.FK_ReciboDatos_Recibo' relationship. Multiple added entities may have the same primary key.
                            datos = new ReciboDatos();
                            datos.Recibo = roc;
                            datos.IdRecibo = roc.IdRecibo;
                            datos.Serie = roc.Serie;
                            datos.IdArea = ordenPago.IdArea == "" ? "000" : ordenPago.IdArea;
                            datos.IdTipoDeposito = ordenPago.IdTipoDeposito;
                            datos.Identificador = ordenPago.Identificador;
                            datos.TextoIdentificador = ordenPago.TextoIdentificador;
                            roc.IdOrdenPago = null;
                            

                            detalles = new List<ReciboDet>(detalleRecibo.Select((s,i) => new ReciboDet()
                            {
                                IdReciboDet=i+1,
                                IdRecibo = recibo.IdRecibo,
                                Serie = recibo.Serie,
                                IdPrecioArancel = s.ArancelPrecio.IdArancelPrecio,
                                Concepto = s.Concepto,
                                Monto = s.Total,
                                Exoneracion = s.Exoneracion,
                                UsuarioCreacion = clsSessionHelper.usuario.Login,
                                FechaCreacion = System.DateTime.Now,
                                //ArancelPrecio = s.ArancelPrecio
                            })).ToList();

                            roc.ReciboDet = null;

                            //LineasIngreso = new List<Asiento>(detalles.Select(s => new Asiento()
                            //{
                            //    IdRecibo = roc.IdRecibo,
                            //    Serie = roc.Serie,
                            //    Monto = Math.Round(s.Monto * new ReciboViewModel().ObtenerTasaCambioDia(s.ArancelPrecio.IdMoneda).Valor, 2, MidpointRounding.AwayFromZero),
                            //    IdArea = datos.IdArea,
                            //    IdCuentaContable = s.ArancelPrecio.ArancelArea.Arancel.IdCuentaContable,
                            //    Naturaleza = clsReferencias.Haber,
                            //}));

                            if (tipoArancel.IdTipoArancel == IdMatricula || tipoArancel.IdTipoArancel == IdPrematricula) //Si es matricula o prematricula hay que insertar el pago en el SIRA
                            {
                                isSIRAPagado = false; //indico que el valor deja de ser null y pongo en falso el pago

                                var xml = new XElement("Pagos",
                                    from det in detalles
                                    select new XElement("DetallePago",
                                        new XElement("IdReciboDet", det.IdReciboDet),
                                        new XElement("IdRecibo", det.IdRecibo),
                                        new XElement("Serie", det.Serie),
                                        new XElement("IdPrecioArancel", det.IdPrecioArancel),
                                        new XElement("Monto", det.Monto),
                                        new XElement("Exonerado", det.Exoneracion == null ? 0 : det.Exoneracion.Exonerado),
                                        new XElement("UsuarioCreacion", det.UsuarioCreacion),
                                        new XElement("FechaCreacion", det.FechaCreacion)
                                    )
                                    );

                                isMatricula = (tipoArancel.IdTipoArancel == IdMatricula); //Si son iguales entonces es una matricula, en caso contrario es una prematricula
                                db.sp_InsertarPagoSIRA(ordenPago.Identificador, xml.ToString(), isMatricula);
                                isSIRAPagado = true; //si se ejecuta el pago lo paso a positivo

                                if (isSIRAPagado.Value)
                                {
                                    ReciboSIRA sira = new ReciboSIRA();
                                    sira.IdRecibo = roc.IdRecibo;
                                    sira.Serie = roc.Serie;
                                    sira.FechaCreacion = roc.Fecha;
                                    sira.isMatricula = isMatricula;
                                    db.ReciboSIRA.Add(sira);
                                }
                            }

                            db.Recibo.Add(roc);
                            db.ReciboDatos.Add(datos);

                        }
                        else
                        {
                            roc.IdOrdenPago = recibo.IdOrdenPago;
                            orden = db.OrdenPago.Find(ordenPago.IdOrdenPago);
                            orden.CodRecibo = roc.IdRecibo + "-" + roc.Serie;

                            //LineasIngreso = new List<Asiento>(FindAllDetailsOrderPay(orden).Select(s => new Asiento()
                            //{
                            //    IdRecibo = roc.IdRecibo,
                            //    Serie = roc.Serie,
                            //    Monto = Math.Round(s.Total * new ReciboViewModel().ObtenerTasaCambioDia(s.ArancelPrecio.IdMoneda).Valor, 2, MidpointRounding.AwayFromZero),
                            //    IdArea = orden.IdArea,
                            //    IdCuentaContable = s.ArancelPrecio.ArancelArea.Arancel.IdCuentaContable,
                            //    Naturaleza = clsReferencias.Haber,
                            //}));

                            db.Entry(orden).State = System.Data.Entity.EntityState.Modified;

                            db.Recibo.Add(roc);

                        }


                        Asiento asientoVariacion = new Asiento();

                        if (diferencias.Any()) //Se guardan las diferencias cambiarias en caso de existir
                        {

                            //Obtengo la variacion en cordobas
                            ReciboDiferencias variacionCordoba = diferencias.Select(s => new ReciboDiferencias()
                            {
                                IdRecibo = roc.IdRecibo,
                                Serie = roc.Serie,
                                IdMoneda = s.IdMoneda,
                                Recibo = roc,
                                Monto = Decimal.Parse(s.Valor.ToString())
                            }).Where(w => w.Monto != 0 && w.IdMoneda == idCordoba).FirstOrDefault();

                            db.ReciboDiferencias.Add(variacionCordoba);


                            if (variacionCordoba != null)
                            {
                                int IdCuentaVariacion;

                                //Obtengo la cuenta contable en base a la variacion siendo positiva o negativa pero solo para la moneda nacional, me apoyo en las variables de configuracion para no crear una nueva tabla solo para almacenar las 2 cuentas
                                if (variacionCordoba.Monto > 0)
                                {
                                    IdCuentaVariacion = int.Parse(db.Configuracion.FirstOrDefault(w => w.Llave == clsConfiguration.Llaves.Variacion_Positiva.ToString()).Valor);
                                }
                                else
                                {
                                    IdCuentaVariacion = int.Parse(db.Configuracion.FirstOrDefault(w => w.Llave == clsConfiguration.Llaves.Variacion_Negativa.ToString()).Valor);
                                }

                                CuentaContable cuenta = db.CuentaContable.FirstOrDefault(w => w.IdCuentaContable == IdCuentaVariacion);

                                if (cuenta != null)
                                {
                                    asientoVariacion = new Asiento()
                                    {
                                        IdRecibo = roc.IdRecibo,
                                        Serie = roc.Serie,
                                        Monto = Math.Abs(variacionCordoba.Monto),
                                        IdCuentaContable = cuenta.IdCuentaContable,
                                        Naturaleza = cuenta.Tipo,
                                    };

                                    //LineasIngreso.Add(asientoVariacion);
                                }
                                else
                                {
                                    throw new Exception("No se encontró la parametrización para la variación cambiaria, corrija esto para poder generar el recibo");
                                }
                            }


                        }

                        if (detalles != null)
                        {
                            db.ReciboDet.AddRange(detalles);
                        }

                        //List<Asiento> asientos = new List<Asiento>();
                        int idpago = 0;
                        foreach (ReciboPagoSon item in detallePago)
                        {
                            ReciboPago pago = new ReciboPago()
                            {
                                IdReciboPago = idpago,
                                IdRecibo = roc.IdRecibo,
                                Serie = roc.Serie,
                                IdFormaPago = item.IdFormaPago,
                                Monto = item.Monto,
                                IdMoneda = item.IdMoneda,
                                FechaCreacion = System.DateTime.Now,
                                UsuarioCreacion = clsSessionHelper.usuario.Login
                            };

                            db.ReciboPago.Add(pago);

                            if (item.ObjInfoAdicional?.GetType() == typeof(ReciboPagoCheque))
                            {
                                ReciboPagoCheque rc = (ReciboPagoCheque)item.ObjInfoAdicional;
                                rc.IdReciboPago = pago.IdReciboPago;

                                db.ReciboPagoCheque.Add(rc);
                            }
                            else if (item.ObjInfoAdicional?.GetType() == typeof(ReciboPagoTarjeta))
                            {
                                ReciboPagoTarjeta rt = (ReciboPagoTarjeta)item.ObjInfoAdicional;
                                rt.IdReciboPago = pago.IdReciboPago;

                                db.ReciboPagoTarjeta.Add(rt);
                            }
                            else if (item.ObjInfoAdicional?.GetType() == typeof(ReciboPagoBono))
                            {
                                ReciboPagoBono rb = (ReciboPagoBono)item.ObjInfoAdicional;
                                rb.IdReciboPago = pago.IdReciboPago;

                                db.ReciboPagoBono.Add(rb);
                            }
                            else if (item.ObjInfoAdicional?.GetType() == typeof(ReciboPagoDeposito))
                            {
                                ReciboPagoDeposito rd = (ReciboPagoDeposito)item.ObjInfoAdicional;
                                rd.IdReciboPago = pago.IdReciboPago;

                                db.ReciboPagoDeposito.Add(rd);
                            }



                            ////Creando el asiento contable para el movimiento del recibo, previamente ya tengo en memoria el registro de ingreso y la variacion cambiaria, esto viene a realizar la partida doble
                            //MovimientoIngreso movimiento = db.MovimientoIngreso.FirstOrDefault(f => f.IdFormaPago == item.IdFormaPago && f.IdMoneda == item.IdMoneda && f.IdRecinto == roc.InfoRecibo.IdRecinto);

                            //if (movimiento != null)
                            //{

                            //    asientos = movimiento.DetalleMovimientoIngreso.Where(w => w.regAnulado == false).Select(s => new Asiento()
                            //    {
                            //        IdRecibo = roc.IdRecibo,
                            //        Serie = roc.Serie,
                            //        IdCuentaContable = s.IdCuentaContable,
                            //        Naturaleza = s.Naturaleza,
                            //        Monto = Math.Round(Math.Round(item.TipoCambio * item.Monto, 2, MidpointRounding.AwayFromZero) * s.FactorPorcentual, 2, MidpointRounding.AwayFromZero)
                            //        //el doble redondeo es 100% necesario NUNCA TOCAR, se hace asi porque el sistema realiza el calculo del diferencial cambiario en base a una cifra redondeda, en este caso es necesario llegar a esa cifra redondeando, luego se hace la multiplicacion por el factor porcentual y se vuelve a redondear para continuar con los calculos.
                            //    }).Union(asientos).ToList();

                            idpago++;
                            //}
                            //else
                            //{
                            //    throw new Exception(string.Format("No se ha encontrado la parametrización contable para el pago de {0} {1} en {2}, no es posible continuar hasta que se ingrese esta información", item.SimboloMoneda, item.Monto, item.FormaPago.FormaPago1));
                            //}

                        }
                        //db.ReciboPago.AddRange(pagos);

                        //asientos = LineasIngreso.Union(asientos).ToList();
                        //if (HayPartidaDoble(asientos)) //Aun hay que corregir el error de decimales, podes usar de ejemplo la tasa 33.9366 pagando $50
                        //// Se deja de esta manera puesto que el asiento contable no se generará desde este sistema por el momento, realizado el 10/02/2020
                        //{
                        //    db.Asiento.AddRange(asientos);
                        //}

                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        if (isSIRAPagado.HasValue)
                        {
                            if (isSIRAPagado.Value)
                            {
                                db.sp_RevertirPagoSIRA(datos.Identificador, roc.IdRecibo, roc.Serie, null, isMatricula);
                            }
                        }
                        transaction.Rollback();

                        exception = ex;
                    }
                }

                if (exception != null)
                {
                    new SharedViewModel().SaveError(exception);
                    throw exception;
                }

            }
            return recibo;

        }

        private bool ValidarOrdenActiva(OrdenPagoSon ordenPago)
        {
            OrdenPago orden = db.OrdenPago.Find(ordenPago.IdOrdenPago);
            if (orden != null)
            {
                if (orden.regAnulado)
                {
                    throw new Exception(clsReferencias.Error_OP_Anulada);
                }
            }
            return true;
        }

        private bool HayPartidaDoble(List<Asiento> asientos)
        {
            decimal[] a = asientos.GroupBy(g => g.Naturaleza).Select(s => Math.Round(s.Sum(p => p.Monto), 2)).ToArray();
            if ((a[0] - a[1] == 0))
            {
                return true;
            }
            else
            {
                throw new Exception("Los cálculos de la parametrización no han creado partida doble, corrija esto para poder continuar");
            }
        }

        private bool ValidarMismoCajero(int apertura)
        {
            bool isSame = db.Recibo.Where(w => w.IdDetAperturaCaja == apertura).All(a => a.UsuarioCreacion == clsSessionHelper.usuario.Login);

            return isSame;
        }

        public void AnularRecibo(ReciboAnulado reciboAnulado)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Recibo r = db.Recibo.Find(reciboAnulado.IdRecibo, reciboAnulado.Serie);
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

                    if (r.ReciboDet.Any())
                    {
                        if (r.ReciboSIRA != null)
                        {
                            db.sp_RevertirPagoSIRA(r.ReciboDatos.Identificador, r.IdRecibo, r.Serie, reciboAnulado.Motivo, r.ReciboSIRA.isMatricula);
                        }
                    }

                    List<Asiento> asientos = db.Asiento.Where(w => w.IdRecibo == reciboAnulado.IdRecibo && w.Serie == reciboAnulado.Serie).ToList();

                    asientos.ForEach(a =>
                    {
                        a.regAnulado = true;
                        db.Entry(a).State = System.Data.Entity.EntityState.Modified;
                    });

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
            string MAC = clsSessionHelper.MACMemory;
            Caja serieCajero = db.Caja.Where(w1 => w1.MAC == MAC && w1.regAnulado == false).FirstOrDefault();

            //"No hemos podido obtener un código de recibo válido, es probable que desde este ordenador no sea posible generar recibos, por favor revise la configuración de tesorería."

            if (serieCajero == null)
            {
                throw new Exception(clsReferencias.Error_NoCaja);
            }

            var apertura = db.DetAperturaCaja.Where(w => w.IdCaja == serieCajero.IdCaja && DbFunctions.TruncateTime(w.AperturaCaja.FechaApertura) == DbFunctions.TruncateTime(DateTime.Now) && w.FechaCierre == null).ToList();
            if (apertura.Count == 0)
            {
                throw new Exception(clsReferencias.Error_Caja_NoAperturada);
            }

            bool usuarioCajero = new LoginViewModel().isCajero(clsSessionHelper.usuario.Login, db);

            if (!usuarioCajero)
            {
                throw new Exception(clsReferencias.Error_No_EsCajero);
            }

            bool mismocajero = ValidarMismoCajero(apertura.First().IdDetAperturaCaja);
            if (!mismocajero)
            {
                throw new Exception(clsReferencias.Error_No_MismoCajero);

            }
            var r = db.Recibo.Where(w => w.Serie.Equals(serieCajero.IdSerie.ToString()));
            int Idrecibo;

            if (r.Any())
            {
                Idrecibo = r.Max(s => s.IdRecibo);
            }
            else
            {
                Idrecibo = 0;
            }


            InfoRecibo info = db.InfoRecibo.FirstOrDefault(w => w.IdRecinto == serieCajero.IdRecinto && w.regAnulado == false);

            if (info == null)
            {
                throw new Exception("No se ha encontrado la configuración de encabezado y pie de recibo para el recinto al cual pertenece esta caja");
            }

            if (Idrecibo == 0)
            {
                try
                {
                    var id = db.Configuracion.FirstOrDefault(w => w.Llave == clsConfiguration.Llaves.Consecutivo_Recibo.ToString());
                    if (id == null)
                    {
                        Idrecibo = 0;
                    }
                    else
                    {
                        Idrecibo = int.Parse(id.Valor.ToString()) - 1;
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return new string[] { (Idrecibo + 1).ToString(), serieCajero.IdSerie, apertura.First().IdDetAperturaCaja.ToString(), info.IdInfoRecibo.ToString() };
        }

        public List<FuenteFinanciamiento> ObtenerFuentesFinanciamiento()
        {
            return db.FuenteFinanciamiento.Where(w => w.RegAnulado == false && w.Tiene_Ingreso).OrderBy(w => w.Nombre).ToList();
        }

        public List<Banco> ObtenerBancos()
        {
            return db.Banco.Where(w => w.RegAnulado == false).OrderBy(o => o.Nombre).ToList();
        }

        public List<CiaTarjetaCredito> ObtenerTarjetas()
        {
            return db.CiaTarjetaCredito.Where(w => w.RegAnulado == false).OrderBy(o => o.Nombre).ToList();
        }

        public List<TipoDeposito> ObtenerTipoCuenta(string IdArea, TipoArancel tipoArancel)
        {
            if (tipoArancel.IdDepositanteUnico.HasValue)
            {
                List<TipoDeposito> lista = new List<TipoDeposito>();
                lista.Add(tipoArancel.TipoDeposito);

                return lista;
            }
            else
            {
                var aranceles = db.ArancelArea.Where(w => w.IdArea == IdArea && w.regAnulado == false).Select(s => s.Arancel).Where(w => w.IdTipoArancel == tipoArancel.IdTipoArancel).ToList();
                var depositantes = db.ArancelTipoDeposito.ToList().Where(a => aranceles.Any(b => a.IdArancel == b.IdArancel)).Select(s => s.TipoDeposito).Distinct();
                return depositantes.OrderBy(w => w.Nombre).ToList();
            }
        }

        public List<Moneda> ObtenerMonedas(int? IdFormaPago)
        {
            if (IdFormaPago.HasValue)
            {
                //Obtengo las monedas pertenecientes a la forma de pago parametrizada
                return db.MovimientoIngreso.Where(w => w.IdFormaPago == IdFormaPago && !w.Moneda.regAnulado).Select(s => s.Moneda).Distinct().ToList();

            }
            else
            {
                //Obtengo todas las monedas, esto es usado para los calculos de variaciones cambiaras
                return db.Moneda.Where(w => !w.regAnulado).ToList();
            }
        }

        public List<FormaPago> ObtenerFormasPago()
        {
            ////Obtengo las formas de pago que se encuentran parametrizadas (QUEDA COMENTAREADO PARA EL FUTURO)
            //return db.MovimientoIngreso.Select(s => s.FormaPago).Distinct().Where(w => !w.regAnulado).OrderBy(o => o.FormaPago1).ToList();

            return db.FormaPago.Where(w => !w.regAnulado).OrderBy(o => o.FormaPago1).ToList();
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

        /// <summary>
        /// Obtiene todos los tipos de cambios correspondientes a la fecha especificada
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns></returns>
        public List<VariacionCambiariaSon> ObtenerTasaCambio(DateTime? fecha)
        {
            var tc = db.fn_ObtenerTasaCambio(fecha, null)
                .ToList()
                .Select(a => new VariacionCambiariaSon()
                {
                    Moneda = new Moneda() { Simbolo = a.Moneda },
                    Valor = Decimal.Parse(a.Valor.ToString()),
                    Fecha = DateTime.Parse(a.Fecha.ToString())
                }).ToList();


            return tc ?? new List<VariacionCambiariaSon>();
        }

        public VariacionCambiaria ObtenerTasaCambioDia(int moneda)
        {
            return ObtenerTasaCambio(moneda, null);
        }

        public bool Authorize_Recinto(string PermisoName, int IdRecinto)
        {
            if (seguridad.Authorize(pantalla, PermisoName, IdRecinto))
            {
                return true;
            }
            else
            {
                throw new AuthorizationException(PermisoName, IdRecinto, db);
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

    public class ReciboSon : Recibo
    {
        public string Recinto { get; set; }
        public string Area { get; set; }
        public string IdArea { get; set; }
        public int IdTipoDeposito { get; set; }
        public string Identificador { get; set; }
        public string TextoIdentificador { get; set; }
        //  public string IdentificacionAgenteExterno { get; set; }
        public TipoDeposito TipoDeposito { get; set; }

        //  public string IdentificadorFinal => IdentificacionAgenteExterno == null ? Identificador : IdentificacionAgenteExterno;

        //IdentificacionAgenteExterno = isAgenteExterno(ReciboDatoTable != null ? ReciboDatoTable.IdTipoDeposito : OrdenTable != null ? OrdenTable.IdTipoDeposito : OrdenAnuladaTable.IdTipoDeposito) ? db.AgenteExternoCat.FirstOrDefault(f=>f.IdAgenteExterno.ToString()==(ReciboDatoTable != null ? ReciboDatoTable.Identificador : OrdenTable != null ? OrdenTable.Identificador : OrdenAnuladaTable.Identificador)).Identificacion: "",

    }

    public class DetReciboSon : ReciboDet
    {

        public decimal MontoVirtual => ArancelPrecio.ArancelArea.Arancel.isPrecioVariable == true ? Monto : ArancelPrecio.Precio;

        public decimal Descuento => Exoneracion == null ? 0 : Exoneracion.Exonerado;
        public decimal Total => MontoVirtual - Descuento;

        //Estos campos son creados para el reporte
        public string Arancel => ArancelPrecio.ArancelArea.Arancel.Nombre;
        public string SimboloMoneda => ArancelPrecio.Moneda.Simbolo;
    }

    public class DetOrdenPagoSon : DetOrdenPagoArancel
    {

        public decimal MontoVirtual => ArancelPrecio.ArancelArea.Arancel.isPrecioVariable == true ? PrecioVariable : ArancelPrecio.Precio;
        public decimal Descuento => Exoneracion == null ? 0 : Exoneracion.Exonerado;
        public decimal Total => MontoVirtual - Descuento;


    }

    public class ReciboPagoSon : ReciboPago, ICloneable
    {

        public ReciboPagoSon()
        {

        }

        public bool isRectificacion => (RectificacionPago1 != null);

        public decimal TipoCambio => new ReciboViewModel().ObtenerTasaCambioDia(IdMoneda).Valor;

        //Estos campos son creados para el reporte
        public string FormaPagoRecibo => FormaPago.FormaPago1;
        public string SimboloMoneda => Moneda.Simbolo;

        public Object ObjInfoAdicional => GetDataAdicional();
        public string StringInfoAdicional => GetDataAdicionalString();

        private Object GetDataAdicional()
        {
            Object info = null;
            if (ReciboPagoCheque != null)
            {
                ReciboPagoCheque rc = ReciboPagoCheque;

                if (rc != null)
                {
                    info = rc;
                    //info = string.Format("{0}, Cuenta {1}, Cheque No.{2}", rc.Banco.Nombre, rc.Cuenta, rc.NumeroCK);
                }
            }
            else if (ReciboPagoTarjeta != null)
            {
                ReciboPagoTarjeta rt = ReciboPagoTarjeta;

                if (rt != null)
                {
                    info = rt;
                    //info = string.Format("{0}, Tarjeta ****{1} Autorización {2}", rt.CiaTarjetaCredito.Nombre, rt.Tarjeta, rt.Autorizacion);
                }
            }
            else if (ReciboPagoDeposito != null)
            {
                ReciboPagoDeposito rd = ReciboPagoDeposito;

                if (rd != null)
                {
                    info = rd;
                    //info = string.Format("{0}, Transacción No.{1}, Obs. {2}", rd.Tipo ? "Transferencia" : "Minuta", rd.Transaccion, rd.Observacion);
                }
            }
            else if (ReciboPagoBono != null)
            {
                ReciboPagoBono rb = ReciboPagoBono;

                if (rb != null)
                {
                    info = rb;
                    //info = string.Format("Emitipo por {0}, Bono No.{1}", rb.Emisor, rb.Numero);
                }
            }

            return info;

        }

        private String GetDataAdicionalString()
        {
            String info = "";
            if (ReciboPagoCheque != null)
            {
                ReciboPagoCheque rc = (ReciboPagoCheque)ObjInfoAdicional;

                if (rc != null)
                {
                    info = string.Format("{0}, Cuenta {1}, Cheque No.{2}", rc.Banco.Nombre, rc.Cuenta, rc.NumeroCK);
                }
            }
            else if (ReciboPagoTarjeta != null)
            {
                ReciboPagoTarjeta rt = (ReciboPagoTarjeta)ObjInfoAdicional;

                if (rt != null)
                {
                    info = string.Format("{0}, Tarjeta ****{1} Autorización {2}", rt.CiaTarjetaCredito.Nombre, rt.Tarjeta, rt.Autorizacion);
                }
            }
            else if (ReciboPagoDeposito != null)
            {
                ReciboPagoDeposito rd = (ReciboPagoDeposito)ObjInfoAdicional;

                if (rd != null)
                {
                    info = string.Format("{0}, Transacción No.{1}, Obs. {2}", rd.Tipo ? "Transferencia" : "Minuta", rd.Transaccion, rd.Observacion);
                }
            }
            else if (ReciboPagoBono != null)
            {
                ReciboPagoBono rb = (ReciboPagoBono)ObjInfoAdicional;

                if (rb != null)
                {
                    info = string.Format("Emitipo por {0}, Bono No.{1}", rb.Emisor, rb.Numero);
                }
            }

            return info;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

    }

}
