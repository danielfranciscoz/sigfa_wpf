using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PruebaWPF.Views.AgenteExterno
{
    /// <summary>
    /// Lógica de interacción para GestionarAgenteExterno.xaml
    /// </summary>
    public partial class GestionarAgenteExterno : Window
    {
        //AgenteExternoViewModel controller;
        Model.AgenteExterno agente;
        Pantalla pantalla;
        clsValidateInput validate = new clsValidateInput();

        public GestionarAgenteExterno()
        {
            InitializeComponent();
        }
        public GestionarAgenteExterno(Pantalla pantalla, String PermisoName)
        {
            agente = new Model.AgenteExterno();
            this.pantalla = pantalla;
            InitializeComponent();
            Diseñar();
        }
        
        private void Diseñar()
        {
            if (agente.IdAgenteExterno > 0)
            {
                btnSave.Visibility = btnEdit.Visibility;
                btnEdit.Visibility = Visibility.Visible;
                txtTitle.Text = "Editar Agente Externo";
            }
            clsutilidades.Dialog_ModalDesign(this);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
