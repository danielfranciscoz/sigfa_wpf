using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.Model
{
    class vista_RecibosPago
    {
        public int IdRecibo { get; set; }
        public string Serie { get; set; }
        public string porCuenta { get; set; }
        public string FormaPago { get; set; }
        public string Moneda { get; set; }
        public double Monto { get; set; }
        public string OrdenPago { get; set; }
        public string FechaROC { get; set; }
    }
}
