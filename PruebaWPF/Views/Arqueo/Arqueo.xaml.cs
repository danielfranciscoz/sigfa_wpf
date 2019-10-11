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

namespace PruebaWPF.Views.Arqueo
{
    /// <summary>
    /// Lógica de interacción para Arqueo.xaml
    /// </summary>
    public partial class Arqueo : Page
    {
        private ArqueoViewModel controller;

        private ObservableCollection<Model.Arqueo> items;
        private Operacion operacion;
        public static bool Cambios = false;

        public Model.Pantalla pantalla;

        public Arqueo()
        {
            controller = new ArqueoViewModel();
            operacion = new Operacion();
            InitializeComponent();
        }

        public Arqueo(Pantalla pantalla)
        {
            this.pantalla = pantalla;

            controller = new ArqueoViewModel();
            operacion = new Operacion();
            InitializeComponent();
        }

        private void ResizeGrid()
        {
            tblArqueos.Height = panelGrid.ActualHeight;

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

        private Task<ObservableCollection<Model.Arqueo>> FindAsync(String text)
        {
            if (text.Equals(""))
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<Model.Arqueo>(controller.FindAll());
                });
            }
            else
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<Model.Arqueo>(controller.FindByText(text)); ;
                });
            }
        }

        private void Load()
        {
            tblArqueos.ItemsSource = items;

        }

        private void LoadTitle()
        {
            Bar_Back e = new Bar_Back();
            e.Value = pantalla.Titulo;
            e.AutoReload = false;
            this.layoutRoot.DataContext = e;
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

        private void btn_Exportar_Click(object sender, RoutedEventArgs e)
        {
            Exportar export = new Exportar(GetDataTable.GetDataGridRows(tblArqueos));
            export.ShowDialog();
            operacion = Exportar.Operacion;

            clsUtilidades.OpenMessage(operacion);
        }

        private void txtFind_KeyUp(object sender, KeyEventArgs e)
        {
            LoadTable(txtFind.Text);
        }

        private void BtnInformeCierre_Click(object sender, RoutedEventArgs e)
        {
            Model.Arqueo arqueo = (Model.Arqueo)tblArqueos.CurrentItem;
            VerInformeArqueo(arqueo);
        }

        private void VerInformeArqueo(Model.Arqueo arqueo)
        {
            rptInforme cierre = new rptInforme(arqueo);
            cierre.ShowDialog();
        }
    }
}
