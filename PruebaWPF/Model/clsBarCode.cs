using PruebaWPF.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.Model
{
    public class clsBarCode
    {
        public string texto { get; set; }
        public byte[] barcode => clsutilidades.CodigoBarraRecibo(texto);

        public clsBarCode() { }
    }
}
