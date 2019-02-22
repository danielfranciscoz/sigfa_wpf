using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.Model
{
    class InfoUsuario
    {
        public decimal no_empleado { get; set; }
        public string cod_reloj { get; set; }
        public byte[] foto { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string NombreCompleto => string.Format("{0} {1}",nombres,apellidos);
        public string cargo{ get; set; }
        public string area{ get; set; }
        public Usuario usuario { get; set; }
    }
}
