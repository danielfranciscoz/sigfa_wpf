using PruebaWPF.UserControls;
using PruebaWPF.ViewModel;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using PruebaWPF.Views.Main;
using PruebaWPF.Views.Shared;
using System.Windows;
using PruebaWPF.Model;
using PruebaWPF.Clases;
using PruebaWPF.Referencias;
using PruebaWPF.Views.Recibo;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Confortex.Clases;

namespace PruebaWPF.Views.OrdenPago
{
    /// <summary>
    /// Lógica de interacción para OrdenPago.xaml
    /// </summary>
    public partial class OrdenPago : Page
    {
        private OrdenPagoViewModel controller;
        public ObservableCollection<OrdenPagoSon> items;
        private Pantalla pantalla;
        private Operacion operacion;
        private Boolean isOpening = true;
        public OrdenPago()
        {

            InitializeComponent();

        }

        public OrdenPago(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            controller = new OrdenPagoViewModel(pantalla);
            operacion = new Operacion();

            InitializeComponent();
            LoadTitle();
        }


        private void Load()
        {
            tblOrdenPago.ItemsSource = items;
            ContarRegistros();
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

        private Task<ObservableCollection<OrdenPagoSon>> FindAsync(String text)
        {
            if (text.Equals(""))
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<OrdenPagoSon>(controller.FindAll());
                });
            }
            else
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<OrdenPagoSon>(controller.FindByText(text)); ;
                });
            }
        }

        private void AddEventItems(ObservableCollection<OrdenPagoSon> items)
        {
            items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Categories_CollectionChanged);
        }

        private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ContarRegistros();
        }

        private void LoadTitle()
        {
            Bar_Back e = new Bar_Back();
            e.Value = pantalla.Titulo;
            this.layoutRoot.DataContext = e;
        }

        private void txtFindText(object sender, KeyEventArgs e)
        {
            LoadTable(txtFind.Text);
        }

        private void ContarRegistros()
        {
            lblCantidadRegitros.Text = "" + tblOrdenPago.Items.Count;
        }


        private void btn_Exportar(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (controller.Autorice(((Button)sender).Tag.ToString()))
                {
                    Exportar export = new Exportar(GetDataTable.GetDataGridRows(tblOrdenPago));
                    export.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            if (tblOrdenPago.SelectedItem != null)
            {
                AbrirVentanaRecibo((OrdenPagoSon)tblOrdenPago.SelectedItem);
            }
            else
            {
                operacion = new Operacion(clsReferencias.TYPE_MESSAGE_Advertencia, clsReferencias.MESSAGE_NoSelection);
                clsutilidades.OpenMessage(operacion);
            }
            //}
            //catch (Exception ex)
            //{
            //    operacion.Mensaje = new clsException(ex).Message();
            //    operacion.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            //}
            //clsutilidades.OpenMessage(operacion);
        }

        private void AbrirVentanaRecibo(OrdenPagoSon item)
        {
            //Debo enviar el objeto clonado porque si envío el objeto capturado en la tabla
            //se edita el objeto aún cuando no hay cambios confirmados
            OrdenPagoSon Objeto = (OrdenPagoSon)item.Clone();

            CrearRecibo window = new CrearRecibo(Objeto);

            window.Owner = Window.GetWindow(frmMain.principal);
            window.ShowDialog();
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

        private void ResizeGrid()
        {
            tblOrdenPago.Height = panelGrid.ActualHeight;
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
                IEnumerable<OrdenPagoSon> colection = (IEnumerable<OrdenPagoSon>)items;
                List<OrdenPagoSon> item2 = new List<OrdenPagoSon>(colection);

                List<OrdenPagoSon> item3;

                if (texto.Equals(""))
                {
                    item3 = controller.FindAll();
                }
                else
                {
                    item3 = controller.FindByText(texto);
                }

                if (!ClsComparer<OrdenPagoSon>.ListComparer(item2, item3))
                {
                    items = new ObservableCollection<OrdenPagoSon>(item3);
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
    }

}
