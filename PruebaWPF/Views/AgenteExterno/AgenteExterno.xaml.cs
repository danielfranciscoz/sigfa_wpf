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

namespace PruebaWPF.Views.AgenteExterno
{
    /// <summary>
    /// Lógica de interacción para AgenteExterno.xaml
    /// </summary>
    public partial class AgenteExterno : Page
    {
        private AgenteExternoViewModel controller;

        private ObservableCollection<Model.AgenteExterno> items;
        private Operacion operacion;
        public static Boolean Cambios = false;
        
        public Model.Pantalla pantalla;

        public AgenteExterno()
        {
            
            InitializeComponent();
        }

        public AgenteExterno(Pantalla pantalla)
        {
            this.pantalla = pantalla;

            controller = new AgenteExternoViewModel(pantalla);
            operacion = new Operacion();
            
            InitializeComponent();
        }

        private void ResizeGrid()
        {
            tblAgenteExterno.Height = panelGrid.ActualHeight;

        }

        private async void LoadTable(string text)
        {
            try
            {
                items = await FindAsync(text);
                Load();
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = ex.Message, OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private Task<ObservableCollection<Model.AgenteExterno>> FindAsync(String text)
        {
            if (text.Equals(""))
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<Model.AgenteExterno>(controller.FindAll());
                });
            }
            else
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<Model.AgenteExterno>(controller.FindByText(text)); ;
                });
            }
        }

        private void Load()
        {
            tblAgenteExterno.ItemsSource = items;
            ContarRegistros();
        }

        private void ContarRegistros()
        {
            lblCantidadRegitros.Text = "" + tblAgenteExterno.Items.Count;
        }

        private void LoadTitle()
        {
            Bar_Back e = new Bar_Back();
            e.Value = pantalla.Titulo;
            e.AutoReload = false;
            this.layoutRoot.DataContext = e;
        }

        private void btn_Exportar(object sender, RoutedEventArgs e)
        {
            Exportar export = new Exportar(GetDataTable.GetDataGridRows(tblAgenteExterno));
            export.ShowDialog();
            operacion = Exportar.Operacion;

            clsutilidades.OpenMessage(operacion);
        }

        private void txtFindText(object sender, KeyEventArgs e)
        {
            LoadTable(txtFind.Text);
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (clsutilidades.OpenDeleteQuestionMessage())
            {
                clsutilidades.OpenMessage(Eliminar());
            }
        }

        private Operacion Eliminar()
        {

            Operacion o = new Operacion();
            try
            {
                controller.Eliminar((Model.AgenteExterno)tblAgenteExterno.CurrentItem);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller.Autorice(((Button)sender).Tag.ToString()))
                {
                    GestionarAgenteExterno ga = new GestionarAgenteExterno(pantalla, btnSave.Tag.ToString());
                    ga.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
