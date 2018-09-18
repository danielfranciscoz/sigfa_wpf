using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica de interacción para AreasRRHH.xaml
    /// </summary>
    public partial class AreasRRHH : Window
    {
        private ObservableCollection<vw_Areas> items { get; set; }
        private SharedViewModel controller;

        public vw_Areas SelectedArea = null;

        public AreasRRHH()
        {
            InitializeComponent();
            Inicializar();
            Diseñar();
        }

        private void Inicializar()
        {
            controller = new SharedViewModel();
            items = new ObservableCollection<vw_Areas>();

        }
        private void Diseñar()
        {
            clsutilidades.Dialog_Perfomance(this);
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
            ResizeGrid();
            LoadTable();
        }

        private async void LoadTable()
        {
            items = await FindAsync(true,null);
            AsignarItemSource(items);
        }

        private async void Buscar(string text)
        {
            items = await FindAsync(false, text);
            AsignarItemSource(items);
        }
        private Task<ObservableCollection<vw_Areas>> FindAsync(Boolean isAll, String text)
        {
            if (isAll)
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<vw_Areas>(controller.ObtenerAreasRH());
                }
    );
            }
            else
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<vw_Areas>(controller.FindAreaByText(text)); ;
                });
            }
        }

        private void AsignarItemSource(ObservableCollection<vw_Areas> items)
        {
            tblAreasRRHH.ItemsSource = items;
            ContarRegistros();
        }

        private void ResizeGrid()
        {
            tblAreasRRHH.Height = panelGrid.ActualHeight;
        }

        private void txtFind_KeyUp(object sender, KeyEventArgs e)
        {
            Buscar(txtFind.Text);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Seleccionar();
        }

        private void Seleccionar()
        {
            if (tblAreasRRHH.SelectedItem != null)
            {
                SelectedArea = (vw_Areas)tblAreasRRHH.SelectedItem;
                this.Close();
            }
            else {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
            }
        }

        private void tblAreasRRHH_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Seleccionar();
        }

        private void ContarRegistros()
        {
            lblCantidadRegitros.Text = "" + tblAreasRRHH.Items.Count;
        }
    }
}
