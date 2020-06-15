using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PruebaWPF.ViewModel
{
    class InformesViewModel
    {
        private SIFOPEntities db = new SIFOPEntities();

        public InformesViewModel() { }

        public Tuple<List<Recibo>, List<ObjetoResumen>, List<ObjetoResumen>, List<ObjetoResumen>, List<ObjetoResumen>, List<ObjetoResumen>, Tuple<List<ReciboPago>>> InformeIngresos(DateTime? startTime, DateTime? endTime, int? IdRecinto, string IdArea, int? IdCaja)
        {
            var recibos = db.Recibo
                .Where(w =>

                (
                    (DbFunctions.TruncateTime(w.Fecha) >= startTime || startTime == null) && (DbFunctions.TruncateTime(w.Fecha) <= endTime || endTime == null)
                )
                &&
                (w.InfoRecibo.IdRecinto == IdRecinto || IdRecinto == null) &&
                (w.DetAperturaCaja.IdCaja == IdCaja || IdCaja == null) &&
                (
                    (
                        w.IdOrdenPago != null ? w.OrdenPago.IdArea == IdArea : w.ReciboDatos.IdArea == IdArea) ||
                        IdArea == null
                ) &&
                !w.regAnulado
                );

            var recintosCount = recibos.Select(s => new { s.InfoRecibo, s.DetAperturaCaja.Caja.Nombre })
                .Join(db.vw_RecintosRH,
                recibo => recibo.InfoRecibo.IdRecinto,
                recinto => recinto.IdRecinto,
                (recibo, recinto) => new { recibo, recinto }
                )
                .GroupBy(g => new { g.recinto.IdRecinto, g.recinto.Siglas, g.recibo.Nombre })
                .OrderByDescending(o => o.Count()).ThenBy(o => o.Key.Siglas)
                .Select(s => new ObjetoResumen
                {
                    IdInt = s.Key.IdRecinto,
                    Name = s.Key.Siglas,
                    SecondName = s.Key.Nombre,
                    Count = s.Count()
                })
                .ToList();


            var areasCount =
                             (recibos
                .Where(w => !w.regAnulado)
                .Select(s => new
                {
                    IdArea = s.ReciboDatos.IdArea ?? s.OrdenPago.IdArea,
                    OPAnulada = s.OrdenPago != null ? s.OrdenPago.regAnulado : false
                })
                ).Where(w => !w.OPAnulada)
                .GroupBy(g => new { g.IdArea })
                .Select(s => new
                {
                    s.Key.IdArea,
                    Total = s.Count()
                })
                .Join(db.vw_Areas,
                pa => pa.IdArea,
                area => area.codigo,
                (pay, area) => new ObjetoResumen
                {
                    Name = area.descripcion,
                    IdString = area.codigo,
                    Count = pay.Total
                }).ToList();

            var pago = db.ReciboPago.Where(w =>

             (
                (DbFunctions.TruncateTime(w.Recibo.Fecha) >= startTime || startTime == null) && (DbFunctions.TruncateTime(w.Recibo.Fecha) <= endTime || endTime == null)
             )
             &&
                (w.Recibo.InfoRecibo.IdRecinto == IdRecinto || IdRecinto == null) &&
                (w.Recibo.DetAperturaCaja.IdCaja == IdCaja || IdCaja == null) &&
                (
                    (
                        w.Recibo.IdOrdenPago != null ? w.Recibo.OrdenPago.IdArea == IdArea : w.Recibo.ReciboDatos.IdArea == IdArea) ||
                        IdArea == null
                ) &&
             !w.Recibo.regAnulado && w.IdRectificacion == null
            );

            var recintosMoney = pago
                .Select(s => new
                {
                    s.Recibo.DetAperturaCaja.Caja.Nombre,
                    s.Recibo.DetAperturaCaja.Caja.IdCaja,
                    s.Recibo.InfoRecibo.IdRecinto,
                    s.Moneda.IdMoneda,
                    s.Moneda.Moneda1,
                    s.Monto
                })
                .GroupBy(g => new { g.IdRecinto, g.Nombre, g.IdCaja, g.Moneda1, g.IdMoneda })
                .Select(s => new
                {
                    s.Key.Nombre,
                    s.Key.IdCaja,
                    s.Key.IdRecinto,
                    s.Key.Moneda1,
                    s.Key.IdMoneda,
                    Monto = s.Sum(ss => ss.Monto)
                })
                .Join(db.vw_RecintosRH,
                recibo => recibo.IdRecinto,
                recinto => recinto.IdRecinto,
                (recibo, recinto) => new { recibo, recinto }
                )
                .OrderBy(o => o.recibo.Moneda1).ThenByDescending(o => o.recibo.Monto).ThenBy(o => o.recinto.Siglas)
                .Select(s => new ObjetoResumen
                {
                    SecondIdInt = s.recibo.IdMoneda,
                    IdInt = s.recibo.IdCaja,
                    Name = s.recinto.Siglas,
                    SecondName = s.recibo.Nombre,
                    Coin = s.recibo.Moneda1,
                    Total = s.recibo.Monto
                })
                .ToList();

            var areasMoney =
                (pago
                .Where(w => !w.regAnulado && !w.Recibo.regAnulado)
                .Select(s => new
                {
                    s.Moneda.Moneda1,
                    s.Monto,
                    IdArea = s.Recibo.ReciboDatos.IdArea ?? s.Recibo.OrdenPago.IdArea,
                    OPAnulada = s.Recibo.OrdenPago != null ? s.Recibo.OrdenPago.regAnulado : false
                })).Where(w => !w.OPAnulada)
                .GroupBy(g => new { g.IdArea, g.Moneda1 })
                .Select(s => new
                {
                    s.Key.Moneda1,
                    Total = s.Sum(sum => sum.Monto),
                    s.Key.IdArea
                })
                .Join(db.vw_Areas,
                pa => pa.IdArea,
                area => area.codigo,
                (pay, area) => new ObjetoResumen
                {
                    Name = area.descripcion,
                    Coin = pay.Moneda1,
                    Total = pay.Total
                }).ToList()

                ;


            var FormaPagoMoney = pago.Select(s => new { s.FormaPago.FormaPago1, s.Moneda.Moneda1, s.Monto })
                .GroupBy(g => new { g.FormaPago1, g.Moneda1 })
                .OrderBy(o => o.Key.Moneda1).ThenByDescending(o => o.Sum(s1 => s1.Monto)).ThenBy(o => o.Key.FormaPago1)
                .Select(s => new ObjetoResumen
                {

                    Name = s.Key.FormaPago1,
                    Coin = s.Key.Moneda1,
                    Total = s.Sum(s1 => s1.Monto)
                })
                .ToList();



            return new Tuple<List<Recibo>, List<ObjetoResumen>, List<ObjetoResumen>, List<ObjetoResumen>, List<ObjetoResumen>, List<ObjetoResumen>, Tuple<List<ReciboPago>>>(recibos.OrderByDescending(o => o.Serie).ThenBy(o => o.IdRecibo).ToList(), recintosCount, areasCount, recintosMoney, areasMoney, FormaPagoMoney, new Tuple<List<ReciboPago>>(pago.ToList()));
        }

        private static Tuple<int, int> IntegerDivide(int dividend, int divisor)
        {
            try
            {
                int remainder;
                int quotient = Math.DivRem(dividend, divisor, out remainder);
                return new Tuple<int, int>(quotient, remainder);
            }
            catch (DivideByZeroException)
            {
                return null;
            }
        }

    }

    class ObjetoResumen
    {
        public int? IdInt { get; set; }
        public int SecondIdInt { get; set; }
        public string IdString { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public decimal Total { get; set; }
        public int Count { get; set; }
        public string Coin { get; set; }
    }
}
