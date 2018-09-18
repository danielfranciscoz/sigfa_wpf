using Confortex.Clases;
using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.UserControls;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Shared;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PruebaWPF.Views.Recibo
{
    /// <summary>
    /// Lógica de interacción para Recibo.xaml
    /// </summary>
    public partial class Recibo : Page
    {

        //private ReciboViewModel controller;
        public ObservableCollection<ReciboSon> items;
        private Pantalla pantalla;
        private Operacion operacion;
        private Boolean isOpening = true;

        public Recibo()
        {
            InitializeComponent();
        }

        public Recibo(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            //controller = new ReciboViewModel(pantalla);
            operacion = new Operacion();

            InitializeComponent();
            LoadTitle();
        }

        private ReciboViewModel controller()
        {
            return new ReciboViewModel(pantalla);
        }

        private void LoadTitle()
        {
            Bar_Back e = new Bar_Back();
            e.Value = pantalla.Titulo;
            this.layoutRoot.DataContext = e;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ResizeGrid();
                LoadTable(txtFind.Text);
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Exportar(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Autorice(((Button)sender).Tag.ToString()))
                {
                    Exportar export = new Exportar(GetDataTable.GetDataGridRows(tblRecibo));
                    export.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void txtFindText(object sender, KeyEventArgs e)
        {
            LoadTable(txtFind.Text);
            LimpiarTablas();
        }

        private void LimpiarTablas()
        {
            tblReciboPay.ItemsSource = null;
            tblReciboDet.ItemsSource = null;
        }

        private async void LoadTable(string text)
        {
            try
            {
                items = await FindAsync(text);
                Load();
                if (isOpening)
                {
                    isOpening = false;
                    if (clsConfiguration.AutomaticReload())
                    {
                        AutomaticReloadTask();
                    }
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private Task<ObservableCollection<ReciboSon>> FindAsync(String text)
        {
            if (text.Equals(""))
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<ReciboSon>(controller().FindAll());
                });
            }
            else
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<ReciboSon>(controller().FindByText(text)); ;
                });
            }
        }

        private async void AutomaticReloadTask()
        {
            while (true)
            {
                Boolean data = await Reload(txtFind.Text);
                await Dormir();
                if (data)
                {
                    Load();
                }
            }
        }

        private async Task<Boolean> Reload(String texto)
        {
            bool needreload = false;
            await Task.Run(() =>
            {
                IEnumerable<ReciboSon> colection = (IEnumerable<ReciboSon>)items;
                List<ReciboSon> item2 = new List<ReciboSon>(colection);

                List<ReciboSon> item3;

                if (texto.Equals(""))
                {
                    item3 = controller().FindAll();
                }
                else
                {
                    item3 = controller().FindByText(texto);
                }

                if (!ClsComparer<ReciboSon>.ListComparer(item2, item3))
                {
                    items = new ObservableCollection<ReciboSon>(item3);
                    needreload = true;
                }
            });

            return needreload;
        }

        private async Task Dormir()
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(clsConfiguration.ThreadSpeep());
            });
        }

        private void Load()
        {
            tblRecibo.ItemsSource = items;
            ContarRegistros();
        }
        private void ContarRegistros()
        {
            lblCantidadRegitros.Text = "" + tblRecibo.Items.Count;
        }

        private void ResizeGrid()
        {
            tblRecibo.Height = panelGrid.ActualHeight;
            tblReciboPay.Height = panelGridPay.ActualHeight;
            tblReciboDet.Height = panelGridDet.ActualHeight;
        }

        private void CargarDetalles()
        {
            ReciboSon selected = (ReciboSon)tblRecibo.SelectedItem;
            tblReciboDet.ItemsSource = controller().DetallesRecibo(selected);
            tblReciboPay.ItemsSource = selected.ReciboPago;
        }

        private void tblRecibo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tblRecibo.SelectedItem != null)
            {
                CargarDetalles();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Autorice(((Button)sender).Tag.ToString()))
                {
                    if (tblRecibo.SelectedItem != null)
                    {
                        rptRecibo boucher = new rptRecibo((ReciboSon)tblRecibo.SelectedItem);
                        boucher.ShowDialog();
                    }
                    else
                    {
                        operacion = new Operacion(clsReferencias.TYPE_MESSAGE_Advertencia, clsReferencias.MESSAGE_NoSelection);
                        clsutilidades.OpenMessage(operacion);
                    }
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tblRecibo.SelectedItem != null)
                {
                    ReciboSon fila = (ReciboSon)tblRecibo.SelectedItem;
                    if (controller().Autorice_Recinto(((Button)sender).Tag.ToString(), fila.Caja.IdRecinto))
                    {
                        AnularRecibo anularRecibo = new AnularRecibo(fila);
                        anularRecibo.ShowDialog();
                    }
                }
                else
                {
                    operacion = new Operacion(clsReferencias.TYPE_MESSAGE_Advertencia, clsReferencias.MESSAGE_NoSelection);
                    clsutilidades.OpenMessage(operacion);
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }
    }
}
