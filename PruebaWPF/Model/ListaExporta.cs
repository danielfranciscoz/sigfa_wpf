using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.Model
{
    class ListaExporta
    {
       
        public string Name { get; set; }
        public string Caption { get; set; }

        public bool isChecked { get;  set; }

        public ListaExporta(string name,string caption,bool ischecked)
        {
            this.Name = name;
            this.Caption = caption;
            this.isChecked = ischecked;
        }
    }
}
