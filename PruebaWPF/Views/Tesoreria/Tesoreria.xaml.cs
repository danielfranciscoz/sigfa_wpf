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

        private List<CiaTarjetaCredito> tarjetas;
        private List<Banco> bancos;
        private List<FormaPago> formaspago;

        private List<FuenteFinanciamientoSon> fuentes;
        private List<Moneda> monedas;
        private List<IdentificacionAgenteExterno> identificaciones;

        private List<InfoReciboSon> infoRecibos;

        public static Boolean Cambios = false;
        public Tesoreria()
        {
            InitializeComponent();

        }
        //TODO La gestion de Monedas queda incompleta, falta la creacion de la formula de conversion de divisa
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

        private Task<List<CiaTarjetaCredito>> FindAsyncTarjetas()
        {
            return Task.Run(() =>
            {
                return controller().FindAllTarjetas();
            });
        }

        private Task<List<Banco>> FindAsyncBancos()
        {
            return Task.Run(() =>
            {
                return controller().FindAllBancos();
            });
        }

        private Task<List<FormaPago>> FindAsyncFormasPago()
        {
            return Task.Run(() =>
            {
                return controller().FindAllFormasPago();
            });
        }

        private Task<List<InfoReciboSon>> FindAsyncInfoRecibo()
        {
            return Task.Run(() =>
            {
                return controller().FindAllInfoRecibos();
            });
        }

        private async void LoadTableCaja()
        {
            try
            {
                cajas = await FindAsyncCajas();
                tblCajas.ItemsSource = cajas;
                tblCajas.Height = panelGrid.ActualHeight;
                ContarRegistrosCaja();
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void ContarRegistrosCaja()
        {
            lblCantidadRegitrosCaja.Text = "" + tblCajas.Items.Count;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTitle();

            LoadTables();
            Cambios = false;

        }

        private void LoadTables()
        {
            if (tabControl != null)
            {
                switch (tabControl.SelectedIndex)
                {
                    case 0:
                        if (series == null || cajas == null || Cambios)
                        {
                            CargarPestaña1();

                        }
                        break;
                    case 1:
                        if (tarjetas == null || bancos == null || formaspago == null || Cambios)
                        {
                            CargarPesataña2();

                        }
                        break;
                    case 2:
                        if (fuentes == null || monedas == null || identificaciones == null || Cambios)
                        {
                            CargarPesataña3();
                        }
                        break;
                    case 3:
                        if (infoRecibos == null || Cambios)
                        {
                            CargarPesataña4();
                        }
                        break;
                }
            }
            else
            {
                CargarPestaña1();
            }
        }

        private void CargarPestaña1()
        {
            LoadTableCaja();
            LoadTableSerie();
        }

        private void CargarPesataña2()
        {
            LoadListTarjeta();
            LoadListBanco();
            LoadListFormaPago();
        }

        private void CargarPesataña3()
        {
            LoadListFF();
            LoadListMoneda();
            LoadListIdentificaciones();
        }

        private void CargarPesataña4()
        {
            LoadTableInfoRecibo();
        }

        private async void LoadListIdentificaciones()
        {
            try
            {
                identificaciones = await FindAsyncIdentificaciones();
                lstIdentificacion.ItemsSource = identificaciones;
                lstIdentificacion.Height = panelGridIdentificacion.ActualHeight;
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private async void LoadTableInfoRecibo()
        {
            try
            {
                infoRecibos = await FindAsyncInfoRecibo();
                tblInfoRecibo.ItemsSource = infoRecibos;
                tblInfoRecibo.Height = panelGridInfoRecibo.ActualHeight;
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private Task<List<IdentificacionAgenteExterno>> FindAsyncIdentificaciones()
        {
            return Task.Run(() =>
            {
                return controller().FindAllIdentifiaciones();
            });
        }

        private async void LoadListMoneda()
        {
            try
            {
                monedas = await FindAsyncMonedas();
                lstMoneda.ItemsSource = monedas;
                lstMoneda.Height = panelGridMoneda.ActualHeight;
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private Task<List<Moneda>> FindAsyncMonedas()
        {
            return Task.Run(() =>
            {
                return controller().FindAllMonedas();
            });
        }

        private async void LoadListFF()
        {
            try
            {
                fuentes = await FindAsyncFuentes();
                lstFF.ItemsSource = fuentes;
                lstFF.Height = panelGridFF.ActualHeight;
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private Task<List<FuenteFinanciamientoSon>> FindAsyncFuentes()
        {
            return Task.Run(() =>
            {
                return controller().FindAllFuentesFinanciamiento();
            });
        }

        private async void LoadTableSerie()
        {
            try
            {
                series = await FindAsyncSeires();
                tblSerie.ItemsSource = series;
                tblSerie.Height = panelGridSerie.ActualHeight;
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }

        }

        private async void LoadListTarjeta()
        {
            try
            {
                tarjetas = await FindAsyncTarjetas();
                lstTarjeta.ItemsSource = tarjetas;
                lstTarjeta.Height = panelGridTarjeta.ActualHeight;
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private async void LoadListBanco()
        {
            try
            {
                bancos = await FindAsyncBancos();
                lstBanco.ItemsSource = bancos;
                lstBanco.Height = panelGridBanco.ActualHeight;
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private async void LoadListFormaPago()
        {
            try
            {
                formaspago = await FindAsyncFormasPago();
                lstFormaPago.ItemsSource = formaspago;
                lstFormaPago.Height = panelGridFormaPago.ActualHeight;
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btn_Exportar(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    Exportar export = new Exportar(GetDataTable.GetDataGridRows(tblCajas));
                    export.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
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
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    GestionarCaja gc = new GestionarCaja(pantalla, btnNew.Tag.ToString());
                    gc.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }
        private void btnNewSerie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    AddSerie ad = new AddSerie(pantalla);
                    ad.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CajaSon son = ((CajaSon)tblCajas.CurrentItem);
                if (controller().Authorize_Recinto(((Button)sender).Tag.ToString(), son.IdRecinto))
                {
                    if (controller().VeriricarAperturasArquedas(son))
                    {
                        if (clsUtilidades.OpenDeleteQuestionMessage())
                        {
                            clsUtilidades.OpenMessage(EliminarCaja());
                        }
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() {Mensaje= "La caja no puede ser eliminada debido a que aun existen arqueos pendientes en el equipo",OperationType=clsReferencias.TYPE_MESSAGE_Advertencia });
                    }

                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

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

                if (controller().Authorize_Recinto(permiso, item.IdRecinto))
                {
                    CajaSon Objeto = (CajaSon)item.Clone();

                    GestionarCaja gc = new GestionarCaja(Objeto, pantalla, permiso);
                    gc.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }

        }

        private void btnDeleteSerie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    if (clsUtilidades.OpenDeleteQuestionMessage("Esta a punto de Inactivar un registro, el proceso es irreversible y ya no será posible crear recibos con esta serie, tenga en cuenta que tampoco será posible volver a crear esta serie aún cuando se haya inactivado, ¿Realmente desea continuar?"))
                    {
                        clsUtilidades.OpenMessage(EliminarSerie());
                    }

                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTables();
        }

        private void btnNewCard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    GestionarT_B_FP gt = new GestionarT_B_FP(new CiaTarjetaCredito(), pantalla);
                    gt.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnEditCad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    if (lstTarjeta.SelectedItem != null)
                    {
                        CiaTarjetaCredito selected = (CiaTarjetaCredito)lstTarjeta.SelectedItem;
                        CiaTarjetaCredito newEdit = new CiaTarjetaCredito()
                        {
                            IdCiaTarjetaCredito = selected.IdCiaTarjetaCredito,
                            Nombre = selected.Nombre,
                            Siglas = selected.Siglas
                        };
                        GestionarT_B_FP gt = new GestionarT_B_FP(newEdit, pantalla);
                        gt.ShowDialog();
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnDeleteCard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    if (lstTarjeta.SelectedItem != null)
                    {
                        if (clsUtilidades.OpenDeleteQuestionMessage())
                        {
                            clsUtilidades.OpenMessage(EliminarTarjeta());
                        }
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                    }


                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }

        private Operacion EliminarTarjeta()
        {
            Operacion o = new Operacion();
            try
            {
                controller().EliminarTarjeta((CiaTarjetaCredito)lstTarjeta.SelectedItem);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                LoadListTarjeta();
            }
            catch (Exception ex)
            {
                o.Mensaje = new clsException(ex).ErrorMessage();
                o.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            }
            return o;
        }

        private void btnNewBanco_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    GestionarT_B_FP gt = new GestionarT_B_FP(new Banco(), pantalla);
                    gt.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnEditBanco_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    if (lstBanco.SelectedItem != null)
                    {
                        Banco selected = (Banco)lstBanco.SelectedItem;
                        Banco newEdit = new Banco()
                        {
                            IdBanco = selected.IdBanco,
                            Nombre = selected.Nombre,
                            Siglas = selected.Siglas
                        };
                        GestionarT_B_FP gt = new GestionarT_B_FP(newEdit, pantalla);
                        gt.ShowDialog();
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnDeleteBanco_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    if (lstBanco.SelectedItem != null)
                    {
                        if (clsUtilidades.OpenDeleteQuestionMessage())
                        {
                            clsUtilidades.OpenMessage(EliminarBanco());
                        }
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                    }


                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }

        private Operacion EliminarBanco()
        {
            Operacion o = new Operacion();
            try
            {
                controller().EliminarBanco((Banco)lstBanco.SelectedItem);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                LoadListBanco();
            }
            catch (Exception ex)
            {
                o.Mensaje = new clsException(ex).ErrorMessage();
                o.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            }
            return o;
        }

        private void btnNewFormaPago_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    GestionarT_B_FP gt = new GestionarT_B_FP(new FormaPago(), pantalla);
                    gt.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnEditFormaPago_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    if (lstFormaPago.SelectedItem != null)
                    {
                        FormaPago selected = (FormaPago)lstFormaPago.SelectedItem;
                        FormaPago newEdit = new FormaPago()
                        {
                            IdFormaPago = selected.IdFormaPago,
                            FormaPago1 = selected.FormaPago1
                        };
                        GestionarT_B_FP gt = new GestionarT_B_FP(newEdit, pantalla);
                        gt.ShowDialog();
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnDeleteFormaPago_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    if (lstFormaPago.SelectedItem != null)
                    {
                        if (clsUtilidades.OpenDeleteQuestionMessage())
                        {
                            clsUtilidades.OpenMessage(EliminarFormaPago());
                        }
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }

        private Operacion EliminarFormaPago()
        {
            Operacion o = new Operacion();
            try
            {
                controller().EliminarFormaPago((FormaPago)lstFormaPago.SelectedItem);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                LoadListFormaPago();
            }
            catch (Exception ex)
            {
                o.Mensaje = new clsException(ex).ErrorMessage();
                o.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            }
            return o;
        }

        private void btnNewFF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    GestionarT_B_FP gt = new GestionarT_B_FP(new FuenteFinanciamiento(), pantalla);
                    gt.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnEditFF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    if (lstFF.SelectedItem != null)
                    {
                        FuenteFinanciamientoSon selected = (FuenteFinanciamientoSon)lstFF.SelectedItem;
                        FuenteFinanciamiento newEdit = new FuenteFinanciamiento()
                        {
                            IdFuenteFinanciamiento = selected.IdFuenteFinanciamiento,
                            Nombre = selected.Nombre,
                            Siglas = selected.Siglas,
                            Tiene_Ingreso = selected.Tiene_Ingreso,
                            Tiene_Egreso = selected.Tiene_Egreso,
                            IdFuente_SIPPSI = selected.IdFuente_SIPPSI,
                            LoginCreacion = selected.LoginCreacion,
                            RegAnulado = selected.RegAnulado
                        };
                        GestionarT_B_FP gt = new GestionarT_B_FP(newEdit, pantalla);
                        gt.ShowDialog();
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnDeleteFF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    if (lstFF.SelectedItem != null)
                    {
                        if (clsUtilidades.OpenDeleteQuestionMessage())
                        {
                            clsUtilidades.OpenMessage(EliminarFF());
                        }
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }

        private Operacion EliminarFF()
        {
            Operacion o = new Operacion();
            try
            {
                controller().EliminarFF((FuenteFinanciamiento)lstFF.SelectedItem);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                LoadListFF();
            }
            catch (Exception ex)
            {
                o.Mensaje = new clsException(ex).ErrorMessage();
                o.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            }
            return o;
        }

        private void btnNewMoneda_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    GestionarT_B_FP gt = new GestionarT_B_FP(new Moneda(), pantalla);
                    gt.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnEditMoneda_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    if (lstMoneda.SelectedItem != null)
                    {
                        Moneda selected = (Moneda)lstMoneda.SelectedItem;
                        Moneda newEdit = new Moneda()
                        {
                            IdMoneda = selected.IdMoneda,
                            Moneda1 = selected.Moneda1,
                            Simbolo = selected.Simbolo,
                            WebService = selected.WebService
                        };
                        GestionarT_B_FP gt = new GestionarT_B_FP(newEdit, pantalla);
                        gt.ShowDialog();
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnDeleteMoneda_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    if (lstMoneda.SelectedItem != null)
                    {
                        if (clsUtilidades.OpenDeleteQuestionMessage())
                        {
                            clsUtilidades.OpenMessage(EliminarMoneda());
                        }
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }

        private Operacion EliminarMoneda()
        {
            Operacion o = new Operacion();
            try
            {
                controller().EliminarMoneda((Moneda)lstMoneda.SelectedItem);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                LoadListMoneda();
            }
            catch (Exception ex)
            {
                o.Mensaje = new clsException(ex).ErrorMessage();
                o.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            }
            return o;
        }

        private void btnNewIdentificacion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    GestionarT_B_FP gt = new GestionarT_B_FP(new IdentificacionAgenteExterno(), pantalla);
                    gt.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnEditIdentificacion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    if (lstIdentificacion.SelectedItem != null)
                    {
                        IdentificacionAgenteExterno selected = (IdentificacionAgenteExterno)lstIdentificacion.SelectedItem;
                        IdentificacionAgenteExterno newEdit = new IdentificacionAgenteExterno()
                        {
                            IdIdentificacion = selected.IdIdentificacion,
                            Identificacion = selected.Identificacion,
                            MaxCaracteres = selected.MaxCaracteres,
                            isMaxMin = selected.isMaxMin,
                        };
                        GestionarT_B_FP gt = new GestionarT_B_FP(newEdit, pantalla);
                        gt.ShowDialog();
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnDeleteIdentificacion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    if (lstIdentificacion.SelectedItem != null)
                    {
                        if (clsUtilidades.OpenDeleteQuestionMessage())
                        {
                            clsUtilidades.OpenMessage(EliminarIdentificacion());
                        }
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }

        private Operacion EliminarIdentificacion()
        {
            Operacion o = new Operacion();
            try
            {
                controller().EliminarIdentificacion((IdentificacionAgenteExterno)lstIdentificacion.SelectedItem);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                LoadListIdentificaciones();
            }
            catch (Exception ex)
            {
                o.Mensaje = new clsException(ex).ErrorMessage();
                o.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            }
            return o;
        }

        private void btnNewInfoRecibo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Permiso = ((Button)sender).Tag.ToString();
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    GestionarT_B_FP gt = new GestionarT_B_FP(new InfoRecibo(), null, pantalla, Permiso);
                    gt.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnEditInfoRecibo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Permiso = ((Button)sender).Tag.ToString();
                if (controller().Authorize(Permiso))
                {
                    if (tblInfoRecibo.SelectedItem != null)
                    {
                        InfoReciboSon selected = (InfoReciboSon)tblInfoRecibo.SelectedItem;

                        InfoRecibo newEdit = new InfoRecibo()
                        {
                            IdInfoRecibo = selected.IdInfoRecibo,
                            Encabezado = selected.Encabezado,
                            Pie = selected.Pie,
                            IdRecinto = selected.IdRecinto,
                        };
                        GestionarT_B_FP gt = new GestionarT_B_FP(newEdit, selected, pantalla, Permiso);
                        gt.ShowDialog();
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }
    }
}
