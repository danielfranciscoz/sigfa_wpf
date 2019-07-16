using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.UserControls;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Shared;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PruebaWPF.Views.VariacionCambiaria
{
    /// <summary>
    /// Lógica de interacción para VariacionCambiaria.xaml
    /// </summary>
    public partial class VariacionCambiaria : Page
    {
        private VariacionCambiariaViewModel controller;

        private ObservableCollection<Model.VariacionCambiaria> items;
        private Operacion operacion;
        public static Boolean Cambios = false;

        public Model.Pantalla pantalla;

        public VariacionCambiaria()
        {
            controller = new VariacionCambiariaViewModel();
            operacion = new Operacion();

            InitializeComponent();
           
        }
        public VariacionCambiaria(Pantalla pantalla)
        {
            this.pantalla = pantalla;

            controller = new VariacionCambiariaViewModel();
            operacion = new Operacion();

            InitializeComponent();
            
        }

        private void ResizeGrid()
        {
            tblVariacionCambiaria.Height = panelGrid.ActualHeight;

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
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = ex.Message, OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private Task<ObservableCollection<Model.VariacionCambiaria>> FindAsync(String text)
        {
            if (text.Equals(""))
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<Model.VariacionCambiaria>(controller.FindAll());
                });
            }
            else
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<Model.VariacionCambiaria>(controller.FindByText(text)); ;
                });
            }
        }

        private void Load()
        {
            tblVariacionCambiaria.ItemsSource = items;
            ContarRegistros();
        }

        private void ContarRegistros()
        {
            lblCantidadRegitros.Text = "" + tblVariacionCambiaria.Items.Count;
        }

        private void LoadTitle()
        {
            Bar_Back e = new Bar_Back();
            e.Value = pantalla.Titulo;
            e.AutoReload = false;
            this.layoutRoot.DataContext = e;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GestionarVariacionCambiaria gvc = new GestionarVariacionCambiaria();
            gvc.ShowDialog();
        }

        private void btn_Exportar(object sender, RoutedEventArgs e)
        {
            Exportar export = new Exportar(GetDataTable.GetDataGridRows(tblVariacionCambiaria));
            export.ShowDialog();
            operacion = Exportar.Operacion;

            clsUtilidades.OpenMessage(operacion);
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
            if (clsUtilidades.OpenDeleteQuestionMessage())
            {
                clsUtilidades.OpenMessage(Eliminar());
            }
        }

        private Operacion Eliminar()
        {

            Operacion o = new Operacion();
            try
            {
                controller.Eliminar((VariacionCambiariaSon)tblVariacionCambiaria.CurrentItem);

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
    }
}
