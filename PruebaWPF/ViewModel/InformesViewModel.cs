using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PruebaWPF.ViewModel
{
    class InformesViewModel
    {
        private SIFOPEntities db = new SIFOPEntities();

        public InformesViewModel() { }

        public Tuple<List<Recibo1>, List<ObjetoResumen>, List<ObjetoResumen>, List<ObjetoResumen>> InformeIngresos()
        {
            var recibos = db.Recibo1.Where(w => !w.regAnulado);

            var recintosCount = recibos.Select(s => s.InfoRecibo)
                .Join(db.vw_RecintosRH,
                recibo => recibo.IdRecinto,
                recinto => recinto.IdRecinto,
                (recibo, recinto) => new { recibo, recinto }
                )
                .GroupBy(g => new { g.recibo.IdRecinto, g.recinto.Siglas }).Select(s => new ObjetoResumen
                {
                    Name = s.Key.Siglas,
                    Count = s.Count()
                }).ToList();

            var cajasCount = recibos.Select(s => s.DetAperturaCaja.Caja).GroupBy(g => new { g.IdCaja, g.Nombre }).Select(s => new ObjetoResumen
            {
                Name = s.Key.Nombre,
                Count = s.Count()
            }).ToList();

            var areasCount = (
                             from r in recibos
                             join area in db.vw_Areas on (r.OrdenPago != null ? r.OrdenPago.IdArea : r.ReciboDatos.IdArea) equals area.codigo into Areas
                             from AreaTable in Areas
                             select new { r, AreaTable })
                             .GroupBy(g => new { g.AreaTable.codigo, g.AreaTable.descripcion }).Select(s => new ObjetoResumen
                              {
                                  Name = s.Key.descripcion,
                                  Count = s.Count()
                              }).ToList();

            //var recintosmoney = (
            //                 from r in recibos
            //                 join area in db.vw_Areas on (r.OrdenPago != null ? r.OrdenPago.IdArea : r.ReciboDatos.IdArea) equals area.codigo into Areas
            //                 from AreaTable in Areas
            //                 join recinto in db.vw_RecintosRH on (r.OrdenPago != null?r.OrdenPago.IdRecinto:r.InfoRecibo.IdRecinto) equals recinto.IdRecinto into Recintos
            //                 from RecintoTable in Recintos
            //                 select new { r, Recintos })
            //  .GroupBy(g => new { g.Recintos., g.recinto.Siglas }).Select(s => new ObjetoResumen
            //  {
            //      Name = s.Key.Siglas,
            //      Count = s.Count()
            //  }).ToList();

            return new Tuple<List<Recibo1>, List<ObjetoResumen>, List<ObjetoResumen>, List<ObjetoResumen>>(recibos.ToList(), recintosCount, cajasCount, areasCount);
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
        public string Name { get; set; }
        public decimal Count { get; set; }
        public string coin { get; set; }
    }
}
