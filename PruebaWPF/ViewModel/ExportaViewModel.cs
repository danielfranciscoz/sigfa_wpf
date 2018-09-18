using PruebaWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.ViewModel
{
    class ExportaViewModel
    {
        private DataTable data;
        public ExportaViewModel(DataTable datatable) {
            this.data = datatable;
        }

        public ObservableCollection<ListaExporta> FindChecks(bool Ischecked)
        {
            ObservableCollection<ListaExporta> lista = new ObservableCollection<ListaExporta>();
            foreach (DataColumn columna in data.Columns)
            {
                
                lista.Add(new ListaExporta(columna.ColumnName, columna.Caption, Ischecked));
            }
            return lista;
        }
    }
}
