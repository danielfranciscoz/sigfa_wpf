using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PruebaWPF.Views.Shared
{
    /// <summary>
    /// Lógica de interacción para CatalogoCuentas.xaml
    /// </summary>
    public partial class CatalogoCuentas : Window
    {

        private ObservableCollection<CuentaContable> cuentas;
        public CuentaContable SelectedCuentaContable = null;

        public CatalogoCuentas()
        {
            InitializeComponent();
            Diseñar();
        }

        public CatalogoCuentas(Pantalla pantalla)
        {
            InitializeComponent();
            Diseñar();
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

        private void Diseñar()
        {
            clsUtilidades.Dialog_Perfomance(this);
        }

        private async void LoadTable(string text)
        {
            try
            {
                cuentas = await FindAsyncCatalogo(text);
                CargarTabla();

            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }

        }

        private void CargarTabla()
        {
            tblCatalogo.ItemsSource = cuentas;
        }



        private Task<ObservableCollection<CuentaContable>> FindAsyncCatalogo(String text)
        {
            if (text.Equals(""))
            {
                return Task.Run(() =>
            {
                return new ObservableCollection<CuentaContable>(controller().FindAll());
            });
            }
            else
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<CuentaContable>(controller().FindByText(text)); ;
                });
            }
        }

        private CatalogoCuentasViewModel controller()
        {
            return new CatalogoCuentasViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTable(txtFind.Text);
        }

        private void txtFind_KeyUp(object sender, KeyEventArgs e)
        {
            LoadTable(txtFind.Text);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Seleccionar();
        }

        private void Seleccionar()
        {
            if (tblCatalogo.SelectedItem != null)
            {

                CuentaContable c = (CuentaContable)tblCatalogo.SelectedItem;
                if (!controller().esPadre(c))
                {
                    SelectedCuentaContable = c;
                    this.Close();
                }
                else
                {
                    panelError.Visibility = Visibility.Visible;
                }
            }
            else
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
            }
        }

        private void TblCatalogo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (tblCatalogo.SelectedItem != null)
            {
                Seleccionar();
            }
        }
    }
}
