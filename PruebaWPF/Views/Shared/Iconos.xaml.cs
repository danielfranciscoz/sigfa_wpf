using MaterialDesignThemes.Wpf;
using PruebaWPF.Clases;
using PruebaWPF.Referencias;
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

namespace PruebaWPF.Views.Shared
{
    /// <summary>
    /// Lógica de interacción para Iconos.xaml
    /// </summary>
    public partial class Iconos : Window
    {
        public string selectedIcon = "";
        List<ModelIcon> iconos;
        public Iconos()
        {
            InitializeComponent();
            Diseñar();
        }

        private void Diseñar()
        {
            clsUtilidades.Dialog_Perfomance(this);
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            SolidColorBrush colorFondo = (SolidColorBrush)FindResource("CloseButton_Dark");
            btnClose.Background = new SolidColorBrush(colorFondo.Color);
            btnClose.Foreground = new SolidColorBrush(Colors.White);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            btnClose.Background = new SolidColorBrush(Colors.Transparent);
            btnClose.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lstIconos.Height = panelGrid.ActualHeight;
            LoadList();
        }

        private void txtFind_KeyUp(object sender, KeyEventArgs e)
        {
            Buscar(txtFind.Text);
        }

        private void Buscar(string text)
        {
            List<ModelIcon> buscados;
            if (!string.IsNullOrWhiteSpace(text))
            {
                buscados = iconos.Where(w=>w.iconString.ToLower().Contains(text.ToLower())).ToList();
            }
            else
            {
                buscados = iconos;
            }

            lstIconos.ItemsSource = buscados;
        }

        private void LoadList()
        {
            iconos = Enum.GetValues(typeof(PackIconKind)).Cast<PackIconKind>().ToList().Select(s => new ModelIcon() { iconString = s.ToString() }).ToList();

            lstIconos.ItemsSource = iconos;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Seleccionar();
        }

        private void Seleccionar()
        {
            if (lstIconos.SelectedItem != null)
            {
                selectedIcon = ((ModelIcon)lstIconos.SelectedItem).iconString;
                this.Close();
            }
            else
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
            }
        }

        private void lstIconos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Seleccionar();
        }
    }

    class ModelIcon
    {
        public string iconString { get; set; }
    }
}
