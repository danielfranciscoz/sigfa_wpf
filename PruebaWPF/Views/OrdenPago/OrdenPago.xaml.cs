﻿using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.UserControls;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using PruebaWPF.Views.Recibo;
using PruebaWPF.Views.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        public static Boolean isOpening = true;
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

        }


        private void Load()
        {
            tblOrdenPago.ItemsSource = items;

        }

        private async void LoadTable(bool allRecintos, string text)
        {

            bool focusTxtFind = txtFind.IsFocused;
            try
            {
                txtFind.IsEnabled = false;
                //if (isOpening)
                //{
                //    isOpening = false;
                if (clsConfiguration.Actual().AutoLoad)
                {
                    AutomaticReloadTask();
                }
                else
                {
                    items = await FindAsync(allRecintos, text);
                    Load();
                }
                //}
                //else
                //{
                //    //Esto funciona usando la palabra reservada Async en el metodo y hace que se recargue todo cuando la pantalla es redimencionada
                //    //items = await FindAsync(text);
                //    //Load();
                //}
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
            finally
            {
                txtFind.IsEnabled = true;

                if (focusTxtFind)
                {
                    txtFind.Focus();
                }
            }
        }

        private Task<ObservableCollection<OrdenPagoSon>> FindAsync(bool isMultiRecinto, String text)
        {
            if (text.Equals(""))
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<OrdenPagoSon>(controller.FindAll(isMultiRecinto));
                });
            }
            else
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<OrdenPagoSon>(controller.FindByText(isMultiRecinto, text));
                });
            }
        }




        private void LoadTitle()
        {
            Bar_Back e = new Bar_Back();
            e.Value = pantalla.Titulo;
            e.AutoReload = true;
            this.tittle_bar.DataContext = e;
        }

        private void txtFindText(object sender, KeyEventArgs e)
        {
            //LoadTable(chkAll.IsChecked.Value, txtFind.Text);
        }

        private void btn_Exportar(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (controller.Authorize(((Button)sender).Tag.ToString()))
                {
                    Exportar export = new Exportar(GetDataTable.GetDataGridRows(tblOrdenPago));
                    export.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
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
                clsUtilidades.OpenMessage(operacion);
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
                LoadTitle();
                ResizeGrid();

                if (items == null || isOpening)
                {
                    isOpening = false;
                    LoadTable(chkAll.IsChecked.Value, txtFind.Text);
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void ResizeGrid()
        {
            tblOrdenPago.Height = panelGrid.ActualHeight;
        }

        private async void AutomaticReloadTask()
        {
            while (this.IsVisible && clsConfiguration.Actual().AutoLoad)
            {
                Boolean data = await Reload(chkAll.IsChecked.Value, txtFind.Text);
                if (data)
                {
                    Load();
                }
                await Dormir();
            }
        }

        private async Task<Boolean> Reload(bool recintos, String texto)
        {
            bool needreload = false;
            await Task.Run(() =>
            {
                IEnumerable<OrdenPagoSon> colection = (IEnumerable<OrdenPagoSon>)items;
                List<OrdenPagoSon> item2;
                if (items == null)
                {
                    item2 = null;
                }
                else
                {
                    item2 = new List<OrdenPagoSon>(colection);
                }

                List<OrdenPagoSon> item3;

                if (texto.Equals(""))
                {
                    item3 = controller.FindAll(recintos);
                }
                else
                {
                    item3 = controller.FindByText(recintos, texto);
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
                 System.Threading.Thread.Sleep(clsConfiguration.MiliSecondSleep());
             });
        }


        private void TxtFindOrder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadTable(chkAll.IsChecked.Value, txtFind.Text);
            }
        }
    }

}
