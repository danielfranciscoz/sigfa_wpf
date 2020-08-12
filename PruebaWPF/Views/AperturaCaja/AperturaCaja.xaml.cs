using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.UserControls;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Shared;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PruebaWPF.Views.AperturaCaja
{
    /// <summary>
    /// Lógica de interacción para AperturaCaja.xaml
    /// </summary>
    public partial class AperturaCaja : Page
    {

        private Pantalla pantalla;
        private List<DetAperturaCajaSon> items;

        public static Boolean Cambios = false;
        public AperturaCaja()
        {
            InitializeComponent();
        }

        public AperturaCaja(Pantalla pantalla)
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

        private void ResizeGrid()
        {
            tblAperturas.Height = panelGrid.ActualHeight;

        }

        private Task<List<DetAperturaCajaSon>> FindAsyncAperturas(String text)
        {
            if (text.Equals(""))
            {
                return Task.Run(() =>
                {
                    return controller().FindAllAperturas();
                });

            }
            else
            {
                return Task.Run(() =>
                {
                    return controller().FindAperturasByText(text);
                });

            }
        }

        private async void LoadTable(string text)
        {
            try
            {
                items = await FindAsyncAperturas(text);
                tblAperturas.ItemsSource = items;
                ContarRegistros();
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = ex.Message, OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void ContarRegistros()
        {
            lblCantidadRegitros.Text = "" + tblAperturas.Items.Count;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTitle();
            ResizeGrid();

            if (items == null || Cambios)
            {
                LoadTable(txtFind.Text);
                Cambios = false;
            }
        }
        private AperturaCajaViewModel controller()
        {
            return new AperturaCajaViewModel(pantalla);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PermisoName = ((Button)sender).Tag.ToString();
                if (controller().Authorize(PermisoName))
                {
                    AperturarCajas ac = new AperturarCajas(pantalla, PermisoName);
                    ac.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btn_Exportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    Exportar export = new Exportar(GetDataTable.GetDataGridRows(tblAperturas));
                    export.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void txtFind_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tblAperturas.SelectedItem != null)
                {

                    if (clsUtilidades.OpenDeleteQuestionMessage(
                        tblAperturas.SelectedItems.Count > 1 ? "Está a punto de cerrar varias cajas, lo cual provocará que no puedan generarse mas recibos desde ellas, ¿Realmente dese continuar?" :
                        "Esta a punto de cerrar esta caja, no podrán generarse mas recibos, ¿Realmente desea continuar?"
                        ))
                    {
                        clsUtilidades.OpenMessage(CerrarCaja());
                    }
                }
                else
                {
                    Operacion operacion = new Operacion(clsReferencias.TYPE_MESSAGE_Advertencia, clsReferencias.MESSAGE_NoSelection);
                    clsUtilidades.OpenMessage(operacion);
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }

        private Operacion CerrarCaja()
        {
            Operacion o = new Operacion();
            try
            {
                controller().CerrarCaja(tblAperturas.SelectedItems.Cast<DetAperturaCaja>().ToList());

                o.Mensaje = clsReferencias.MESSAGE_Exito_Save;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                LoadTable(txtFind.Text);
            }
            catch (Exception ex)
            {
                o.Mensaje = new clsException(ex).ErrorMessage();
                o.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            }
            return o;
        }

        private void tblAperturas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tblAperturas.SelectedItem != null)
            {
                BotonCierre(tblAperturas.SelectedItems.Cast<DetAperturaCajaSon>().ToList());
            }
        }


        private void BotonCierre(List<DetAperturaCajaSon> selectedItem)
        {
            btnCerrar.IsEnabled = true;
            if (selectedItem.Count > 1)
            {
                btnCerrar.Content = "CERRAR CAJAS";
            }
            else
            {
                btnCerrar.Content = "CERRAR CAJA";
            }

            try
            {
                foreach (var i in selectedItem)
                {
                    int recinto = i.AperturaCaja.IdRecinto;

                    if (i.FechaCierre != null)
                    {
                        btnCerrar.IsEnabled = false;
                        break;
                    }
                    else if (!controller().Authorize_Recinto(btnCerrar.Tag.ToString(), recinto))
                    {
                        btnCerrar.IsEnabled = false;
                        break;
                    }
                }
            }
            catch (AuthorizationException)
            {
                btnCerrar.IsEnabled = false;
            }

        }

        private void btnInformeCierre_Click(object sender, RoutedEventArgs e)
        {
            rptInforme cierre = new rptInforme((DetAperturaCajaSon)tblAperturas.SelectedItem);
            cierre.ShowDialog();
        }
    }
}
