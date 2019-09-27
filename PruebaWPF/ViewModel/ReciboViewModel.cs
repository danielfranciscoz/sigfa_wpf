using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Interface;
using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;

namespace PruebaWPF.ViewModel
{
    class ReciboViewModel : IGestiones<ReciboSon>
    {
        private SIFOPEntities db = new SIFOPEntities();
        private Pantalla pantalla;
        List<vw_RecintosRH> r;
        private SecurityViewModel seguridad;

        public ReciboViewModel() { }

        public ReciboViewModel(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            seguridad = new SecurityViewModel();
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

            List<Recibo1> recibo = db.Recibo1.Take(clsConfiguration.Actual().TopRow).ToList().Where(w => r.Any(a => w.InfoRecibo.IdRecinto == a.IdRecinto)).ToList();

            return UnirRecibos(recibo);
        }

        public List<ReciboSon> FindAllApertura(int IdDetApertura)
        {
            List<Recibo1> recibo = db.Recibo1.Where(w => w.IdDetAperturaCaja == IdDetApertura).ToList();

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
                    .Where(w => r.Any(a => w.InfoRecibo.IdRecinto == a.IdRecinto))
                    .ToList();
                return UnirRecibos(recibo);
            }
            else
            {
                return FindAll();
            }
        }

        public List<VariacionCambiariaSon> FindTipoCambio(ReciboSon recibo, List<DetReciboSon> detalleR)
        {
            List<VariacionCambiariaSon> datos = new List<VariacionCambiariaSon>();

            List<VariacionCambiaria> monedasDetalles;

            if (detalleR == null)
            {
                monedasDetalles = DetallesRecibo(recibo).Select(s => new VariacionCambiaria() { Moneda = s.ArancelPrecio.Moneda }).ToList();
            }
            else
            {
                monedasDetalles = detalleR.Select(s => new VariacionCambiaria() { Moneda = s.ArancelPrecio.Moneda }).ToList();
            }

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
                IdDetAperturaCaja = r.IdDetAperturaCaja,
                IdArea = r.OrdenPago.IdArea,
                IdFuenteFinanciamiento = r.IdFuenteFinanciamiento,
                IdTipoDeposito = r.OrdenPago.IdTipoDeposito,
                Identificador = r.OrdenPago.Identificador,
                TextoIdentificador = r.OrdenPago.TextoIdentificador,
                TipoDeposito = r.OrdenPago.TipoDeposito,
                Recibimos = r.Recibimos,
                Fecha = r.Fecha,
                IdOrdenPago = r.OrdenPago.IdOrdenPago,
                regAnulado = r.regAnulado,
                IdInfoRecibo = r.IdInfoRecibo,
                InfoRecibo = r.InfoRecibo,
                DetAperturaCaja = r.DetAperturaCaja,
                OrdenPago = r.OrdenPago,
                FuenteFinanciamiento = r.FuenteFinanciamiento,
                UsuarioCreacion = r.UsuarioCreacion,
                Serie = r.Serie,
                Recinto = clsSessionHelper.recintosMemory.Find(w => w.IdRecinto == r.InfoRecibo.IdRecinto).Siglas,
                Area = clsSessionHelper.areasMemory.Find(w => w.codigo == r.OrdenPago.IdArea).descripcion.ToUpper(),
            }).ToList();

            //La información se obtiene del detalle del recibo
            var listaSinOrdenes = recibo.Where(w => w.IdOrdenPago == null && w.regAnulado == false).Select(r => new ReciboSon()
            {
                IdRecibo = r.IdRecibo,
                Serie = r.Serie,
                IdDetAperturaCaja = r.IdDetAperturaCaja,
                IdArea = r.ReciboDatos.IdArea,
                IdFuenteFinanciamiento = r.IdFuenteFinanciamiento,
                IdTipoDeposito = r.ReciboDatos.IdTipoDeposito,
                Identificador = r.ReciboDatos.Identificador,
                TextoIdentificador = r.ReciboDatos.TextoIdentificador,
                Recibimos = r.Recibimos,
                Fecha = r.Fecha,
                IdOrdenPago = r.IdOrdenPago,
                regAnulado = r.regAnulado,
                IdInfoRecibo = r.IdInfoRecibo,
                InfoRecibo = r.InfoRecibo,
                DetAperturaCaja = r.DetAperturaCaja,
                OrdenPago = r.OrdenPago,
                FuenteFinanciamiento = r.FuenteFinanciamiento,
                TipoDeposito = r.ReciboDatos.TipoDeposito,
                UsuarioCreacion = r.UsuarioCreacion,
                Recinto = clsSessionHelper.recintosMemory.Find(w => w.IdRecinto == r.InfoRecibo.IdRecinto).Siglas,
                Area = clsSessionHelper.areasMemory.FirstOrDefault(w => w.codigo == r.ReciboDatos.IdArea).descripcion.ToUpper(),
            }).ToList();

            //La información se obtiene de la orden de pago, cuando el recibo está anulado
            var ListaAnuladosConOrdenes = recibo.Where(w => w.regAnulado == true && w.ReciboAnulado.IdOrdenPago != null).Select(r => new ReciboSon()
            {
                IdRecibo = r.IdRecibo,
                Serie = r.Serie,
                IdDetAperturaCaja = r.IdDetAperturaCaja,
                IdArea = r.ReciboAnulado.OrdenPago.IdArea,
                IdFuenteFinanciamiento = r.IdFuenteFinanciamiento,
                IdTipoDeposito = r.ReciboAnulado.OrdenPago.IdTipoDeposito,
                Identificador = r.ReciboAnulado.OrdenPago.Identificador,
                TextoIdentificador = r.ReciboAnulado.OrdenPago.TextoIdentificador,
                Recibimos = r.Recibimos,
                Fecha = r.Fecha,
                IdOrdenPago = r.ReciboAnulado.OrdenPago.IdOrdenPago,
                regAnulado = r.regAnulado,
                IdInfoRecibo = r.IdInfoRecibo,
                InfoRecibo = r.InfoRecibo,
                DetAperturaCaja = r.DetAperturaCaja,
                OrdenPago = r.ReciboAnulado.OrdenPago,
                FuenteFinanciamiento = r.FuenteFinanciamiento,
                TipoDeposito = r.ReciboAnulado.OrdenPago.TipoDeposito,
                UsuarioCreacion = r.UsuarioCreacion,
                Recinto = clsSessionHelper.recintosMemory.Find(w => w.IdRecinto == r.InfoRecibo.IdRecinto).Siglas,
                Area = clsSessionHelper.areasMemory.Find(w => w.codigo == r.ReciboAnulado.OrdenPago.IdArea).descripcion.ToUpper(),
                ReciboAnulado = r.ReciboAnulado
            }).ToList();

            //La información se obtiene del detalle del recibo anulado
            var ListaAnuladosSinOrdenes = recibo.Where(w => w.regAnulado == true && w.ReciboAnulado.IdOrdenPago == null).Select(r => new ReciboSon()
            {
                IdRecibo = r.IdRecibo,
                Serie = r.Serie,
                IdDetAperturaCaja = r.IdDetAperturaCaja,
                IdArea = r.ReciboDatos.IdArea,
                IdFuenteFinanciamiento = r.IdFuenteFinanciamiento,
                IdTipoDeposito = r.ReciboDatos.IdTipoDeposito,
                Identificador = r.ReciboDatos.Identificador,
                TextoIdentificador = r.ReciboDatos.TextoIdentificador,
                Recibimos = r.Recibimos,
                Fecha = r.Fecha,
                IdOrdenPago = r.IdOrdenPago,
                regAnulado = r.regAnulado,
                IdInfoRecibo = r.IdInfoRecibo,
                InfoRecibo = r.InfoRecibo,
                DetAperturaCaja = r.DetAperturaCaja,
                OrdenPago = r.OrdenPago,
                FuenteFinanciamiento = r.FuenteFinanciamiento,
                TipoDeposito = r.ReciboDatos.TipoDeposito,
                UsuarioCreacion = r.UsuarioCreacion,
                Recinto = clsSessionHelper.recintosMemory.Find(w => w.IdRecinto == r.InfoRecibo.IdRecinto).Siglas,
                Area = clsSessionHelper.areasMemory.Find(w => w.codigo == r.ReciboDatos.IdArea).descripcion.ToUpper(),
                ReciboAnulado = r.ReciboAnulado
            }).ToList();


            return ListaConOrdenes.Union(listaSinOrdenes).Union(ListaAnuladosConOrdenes).Union(ListaAnuladosSinOrdenes).OrderByDescending(o => o.IdRecibo).ThenBy(o => o.Serie).ToList();
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

                    arancelesSIRA = clsCallProcedure<ArancelSIRAValidate>.GetFromProcedure(db, "cat.sp_ArancelesSIRA @carnet", p).ToList();

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

                    var arancel = db.ArancelPrecio.ToList().Where(w => arancelesSIRA.Any(a => a.IdArancelPrecio == w.IdArancelPrecio));

                    return arancel.ToList();
                }

            }

            //Obtengo los Ids de Arancel que estan asociadas tanto al área como al tipo de depósito seleccionado por el usuario
            IEnumerable<int> areas;

            if (arancelesSIRA != null)
            {
                areas = db.ArancelArea.ToList().Where(w => w.IdArea.Equals(IdArea) && arancelesSIRA.Any(a => a.IdArancel == w.IdArancel)).Select(s => s.IdArancel).ToArray();
            }
            else
            {
                areas = db.ArancelArea.Where(w => w.IdArea.Equals(IdArea) && w.regAnulado == false && w.Arancel.IdTipoArancel == IdTipoArancel && w.Arancel.regAnulado == false).Select(s => s.IdArancel).ToArray();
            }

            IEnumerable<int> TipoDepositos = db.ArancelTipoDeposito.Where(w => w.IdTipoDeposito == IdTipoDeposito && w.Arancel.IdTipoArancel == IdTipoArancel && w.Arancel.regAnulado == false).Select(s => s.IdArancel).ToArray();

            //Obtengo los Ids en donde se intersecta la información anteriormente obtenida
            var IdIntersect = TipoDepositos.Intersect(areas).ToList();

            //Finalmente con los Ids recuperados por la intersección se obtienen los aranceles.     
            var r = db.ArancelPrecio.Where(w => IdIntersect.Any(w1 => w.ArancelArea.IdArancel == w1 && w.ArancelArea.IdArea == IdArea) && w.regAnulado == false).ToList();

            return r;
        }


        public Exoneracion FindExoneracionArancel(string identificador, int idTipoDeposito, int idArancelPrecio)
        {
            Exoneracion exoneracion = db.Exoneracion.FirstOrDefault(f => f.Identificador == identificador && f.IdTipoDeposito == idTipoDeposito && f.IdArancelPrecio == idArancelPrecio && f.regAnulado == false && f.Autorizadopor != null && !f.DetOrdenPagoArancel.Any(a => a.OrdenPago.regAnulado == false) && !f.ReciboDet.Any(a => a.Recibo1.regAnulado == false));
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
            }).ToList();
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
            if (recibo.IdOrdenPago == null)
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

        public List<ReciboPagoSon> ReciboFormaPago(ReciboSon recibo)
        {
            return db.ReciboPago.Where(w => w.IdRecibo == recibo.IdRecibo && w.Serie == recibo.Serie).ToList().Select(s => new ReciboPagoSon()
            {
                DetalleAdicional = ObtenerObjetoAdicional(s)[0],
                InfoAdicional = ObtenerObjetoAdicional(s)[1].ToString(),
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

                    if (rc != null)
                    {
                        o[0] = rc;
                        o[1] = string.Format("{0}, Cuenta {1}, Cheque No.{2}", rc.Banco.Nombre, rc.Cuenta, rc.NumeroCK);
                    }
                    else
                    {
                        o[0] = null;
                        o[1] = "";
                    }
                    break;
                case 3: //Tarjeta
                    ReciboPagoTarjeta rt = r.ReciboPagoTarjeta;

                    if (rt != null)
                    {
                        o[0] = rt;
                        o[1] = string.Format("{0}, Tarjeta ****{1} Autorización {2}", rt.CiaTarjetaCredito.Nombre, rt.Tarjeta, rt.Autorizacion);
                    }
                    else
                    {
                        o[0] = null;
                        o[1] = "";
                    }
                    break;
                case 4: //Bono
                    ReciboPagoBono rb = r.ReciboPagoBono;

                    if (rb != null)
                    {
                        o[0] = rb;
                        o[1] = string.Format("Emitipo por {0}, Bono No.{1}", rb.Emisor, rb.Numero);
                    }
                    else
                    {
                        o[0] = null;
                        o[1] = "";
                    }

                    break;
                case 5: //Deposito
                    ReciboPagoDeposito rd = r.ReciboPagoDeposito;

                    if (rd != null)
                    {
                        o[0] = rd;
                        o[1] = string.Format("{0}, Transacción No.{1}, Obs. {2}", rd.Tipo ? "Transferencia" : "Minuta", rd.Transaccion, rd.Observacion);
                    }
                    else
                    {
                        o[0] = null;
                        o[1] = "";
                    }

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

        public ReciboSon GenerarRecibo(TipoArancel tipoArancel, ReciboSon recibo, OrdenPagoSon ordenPago, List<DetOrdenPagoSon> detalleRecibo, List<ReciboPagoSon> detallePago, int? IdMatricula, int? IdPrematricula)
        {
            bool? isSIRAPagado = null;
            bool isMatricula = true; //true:Matricula; false:Prematricula
            Recibo1 roc = new Recibo1();
            ReciboDatos datos = null; // no creo la instancia porque no se si será usado
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    List<ReciboDet> detalles = null;
                    OrdenPago orden;

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
                        datos = new ReciboDatos();

                        datos.IdRecibo = recibo.IdRecibo;
                        datos.Serie = recibo.Serie;
                        datos.IdArea = ordenPago.IdArea == "" ? "000" : ordenPago.IdArea;
                        datos.IdTipoDeposito = ordenPago.IdTipoDeposito;
                        datos.Identificador = ordenPago.Identificador;
                        datos.TextoIdentificador = ordenPago.TextoIdentificador;
                        roc.IdOrdenPago = null;

                        detalles = new List<ReciboDet>(detalleRecibo.Select(s => new ReciboDet()
                        {
                            IdRecibo = roc.IdRecibo,
                            Serie = roc.Serie,
                            IdPrecioArancel = s.ArancelPrecio.IdArancelPrecio,
                            Concepto = s.Concepto,
                            Monto = s.Total,
                            Exoneracion = s.Exoneracion,
                            UsuarioCreacion = clsSessionHelper.usuario.Login,
                            FechaCreacion = System.DateTime.Now
                        }));

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

                        db.ReciboDatos.Add(datos);

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
                            case 5: //Deposito
                                ReciboPagoDeposito rd = (ReciboPagoDeposito)item.DetalleAdicional;
                                rd.IdReciboPago = pago.IdReciboPago;

                                db.ReciboPagoDeposito.Add(rd);
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
                    if (isSIRAPagado.HasValue)
                    {
                        if (isSIRAPagado.Value)
                        {
                            db.sp_RevertirPagoSIRA(datos.Identificador, roc.IdRecibo, roc.Serie, null, isMatricula);
                        }
                    }
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

                    if (r.ReciboDet.Any())
                    {
                        if (r.ReciboSIRA != null)
                        {
                            db.sp_RevertirPagoSIRA(r.ReciboDatos.Identificador, r.IdRecibo, r.Serie, reciboAnulado.Motivo, r.ReciboSIRA.isMatricula);
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
                throw new Exception("No hemos encontrado este ordenador entre la lista de cajas disponibles para realizar pagos en el sistema, por favor pida ayuda al administrador de tesorería.");
            }

            var apertura = db.DetAperturaCaja.Where(w => w.IdCaja == serieCajero.IdCaja).ToList().Where(w => w.AperturaCaja.FechaApertura.Date == System.DateTime.Now.Date && w.FechaCierre == null).ToList();
            if (apertura.Count == 0)
            {
                throw new Exception("Esta caja no se encuentra aperturada para realizar pagos, contacte al administrador de tesorería para dar apertura a la caja.");
            }
            var r = db.Recibo1.Where(w => w.Serie.Equals(serieCajero.IdSerie.ToString()));
            int Idrecibo;

            if (r.Count() > 0)
            {
                Idrecibo = r.Max(s => s.IdRecibo);
            }
            else
            {
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

        public List<Moneda> ObtenerMonedas()
        {
            return db.Moneda.ToList();
        }

        public List<FormaPago> ObtenerFormasPago()
        {
            return db.FormaPago.Where(w => w.regAnulado == false).OrderBy(o => o.FormaPago1).ToList();
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

    public class ReciboSon : Recibo1
    {
        public string Recinto { get; set; }
        public string Area { get; set; }
        public string IdArea { get; set; }
        public int IdTipoDeposito { get; set; }
        public string Identificador { get; set; }
        public string TextoIdentificador { get; set; }
        public TipoDeposito TipoDeposito { get; set; }
        

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


