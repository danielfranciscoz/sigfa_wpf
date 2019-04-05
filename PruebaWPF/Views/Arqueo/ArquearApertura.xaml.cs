using PruebaWPF.Clases;
using PruebaWPF.Model;
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

namespace PruebaWPF.Views.Arqueo
{
    /// <summary>
    /// Lógica de interacción para ArquearApertura.xaml
    /// </summary>
    public partial class ArquearApertura : Window
    {
        private Operacion operacion;
        private Pantalla pantalla;

        public ArquearApertura()
        {
            InitializeComponent();
        }

        public ArquearApertura(Pantalla pantalla)
        {
            this.pantalla = pantalla;

            operacion = new Operacion();
            InitializeComponent();
            Diseñar();
        }

        private void Diseñar()
        {
            clsutilidades.Dialog_ModalDesign(this);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
