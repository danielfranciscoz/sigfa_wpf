using System;

namespace PruebaWPF.Model
{
   public class ColumnasFiltro
    {
        public string FuenteFinanciamiento { get; set; }
        public string Fecha { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public string Recinto { get; set; }
        public string Area { get; set; }
        public string Caja { get; set; }
    }
}
