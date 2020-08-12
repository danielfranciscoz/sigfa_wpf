using MaterialDesignExtensions.Model;
using PruebaWPF.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaWPF.ViewModel
{
    class AutoCompleteArancelViewModel
    {
        private IAutocompleteSource m_autocompleteSource;

        public event PropertyChangedEventHandler PropertyChanged;


        private object m_selectedItem;

        public IAutocompleteSource AutocompleteSource
        {
            get
            {
                return m_autocompleteSource;
            }
        }

        public object SelectedItem
        {
            get
            {
                return m_selectedItem;
            }

            set
            {
                m_selectedItem = value;

                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null && !string.IsNullOrWhiteSpace(propertyName))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public AutoCompleteArancelViewModel(string IdArea, int IdTipoDeposito, int IdTipoArancel, string criterio, int? IdPrematricula, int? IdMatricula)
            : base()
        {
            m_autocompleteSource = new ArancelAutocompleteSource(IdArea, IdTipoDeposito, IdTipoArancel, criterio, IdPrematricula, IdMatricula);

            m_selectedItem = null;
        }
    }

    public class ArancelAutocompleteSource : IAutocompleteSource
    {
        private List<ArancelPrecio> m_operatingSystemItems;
        ReciboViewModel controller = new ReciboViewModel();

        public ArancelAutocompleteSource(string IdArea, int IdTipoDeposito, int IdTipoArancel, string criterio, int? IdPrematricula, int? IdMatricula)
        {
            m_operatingSystemItems = controller.ObtenerAranceles(IdArea, IdTipoDeposito, IdTipoArancel, criterio, IdPrematricula, IdMatricula);
        }

        IEnumerable IAutocompleteSource.Search(string searchTerm)
        {
            searchTerm = searchTerm ?? string.Empty;
            searchTerm = searchTerm.ToLower();

            return m_operatingSystemItems.Where(item => item.ArancelArea.Arancel.Nombre.ToLower().Contains(searchTerm));
        }
    }

}
