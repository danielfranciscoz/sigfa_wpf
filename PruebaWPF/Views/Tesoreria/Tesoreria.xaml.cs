using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.UserControls;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using PruebaWPF.Views.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PruebaWPF.Views.Tesoreria
{
    /// <summary>
    /// Lógica de interacción para Tesoreria.xaml
    /// </summary>
    public partial class Tesoreria : Page
    {
        private Pantalla pantalla;
        private List<CajaSon> cajas;
        private List<SerieRecibo> series;
        public static Boolean Cambios = false;
        public Tesoreria()
        {
            InitializeComponent();

        }

        public Tesoreria(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            InitializeComponent();


        }

        private void LoadTitle()
        {
            Bar_Back e = new Bar_Back();
            e.Value = pantalla.Titulo;
            e.AutoReload = false;
            this.layoutRoot.DataContext = e;
        }

        private Task<List<CajaSon>> FindAsyncCajas()
        {
            return Task.Run(() =>
            {
                return controller().FindAllCajas();
            });
        }

        private Task<List<SerieRecibo>> FindAsyncSeires()
        {
            return Task.Run(() =>
            {
                return controller().FindAllSeries();
            });
        }

        private async void LoadTableCaja()
        {
            try
            {
                cajas = await FindAsyncCajas();
                tblCajas.ItemsSource = cajas;
                ContarRegistrosCaja();
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void ContarRegistrosCaja()
        {
            lblCantidadRegitrosCaja.Text = "" + tblCajas.Items.Count;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTitle();
            ResizeGrid();

            if (cajas == null || series == null || Cambios)
            {
                LoadTables();
                Cambios = false;
            }
        }
        private void ResizeGrid()
        {
            tblCajas.Height = panelGrid.ActualHeight;
            tblSerie.Height = panelGridSerie.ActualHeight;
        }

        private void LoadTables()
        {
            LoadTableCaja();
            LoadTableSerie();
        }

        private async void LoadTableSerie()
        {
            try
            {
                series = await FindAsyncSeires();
                tblSerie.ItemsSource = series;
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }

        }


        private void btn_Exportar(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Autorice(((Button)sender).Tag.ToString()))
                {
                    Exportar export = new Exportar(GetDataTable.GetDataGridRows(tblCajas));
                    export.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private TesoreriaViewModel controller()
        {
            return new TesoreriaViewModel(pantalla);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Autorice(((Button)sender).Tag.ToString()))
                {
                    GestionarCaja gc = new GestionarCaja(pantalla, btnNew.Tag.ToString());
                    gc.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }
        private void btnNewSerie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Autorice(((Button)sender).Tag.ToString()))
                {
                    AddSerie ad = new AddSerie(pantalla);
                    ad.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Autorice_Recinto(((Button)sender).Tag.ToString(), ((CajaSon)tblCajas.CurrentItem).IdRecinto))
                {
                    if (clsutilidades.OpenDeleteQuestionMessage())
                    {
                        clsutilidades.OpenMessage(EliminarCaja());
                    }

                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }

        private Operacion EliminarCaja()
        {

            Operacion o = new Operacion();
            try
            {
                controller().EliminarCaja((CajaSon)tblCajas.CurrentItem);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                LoadTableCaja();
            }
            catch (Exception ex)
            {
                o.Mensaje = new clsException(ex).ErrorMessage();
                o.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            }
            return o;

        }

        private Operacion EliminarSerie()
        {

            Operacion o = new Operacion();
            try
            {
                controller().EliminarSerie((SerieRecibo)tblSerie.CurrentItem);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                LoadTableSerie();
            }
            catch (Exception ex)
            {
                o.Mensaje = new clsException(ex).ErrorMessage();
                o.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            }
            return o;

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string permiso = ((Button)sender).Tag.ToString();
                CajaSon item = (CajaSon)tblCajas.CurrentItem;

                if (controller().Autorice_Recinto(permiso, item.IdRecinto))
                {
                    CajaSon Objeto = (CajaSon)item.Clone();

                    GestionarCaja gc = new GestionarCaja(Objeto, pantalla, permiso);
                    gc.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }

        }

        private void btnDeleteSerie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Autorice(((Button)sender).Tag.ToString()))
                {
                    if (clsutilidades.OpenDeleteQuestionMessage("Esta a punto de Inactivar un registro, el proceso es irreversible y ya no será posible crear recibos con esta serie, tenga en cuenta que tampoco será posible volver a crear esta serie aún cuando se haya inactivado, ¿Realmente desea continuar?"))
                    {
                        clsutilidades.OpenMessage(EliminarSerie());
                    }

                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }
    }
}
