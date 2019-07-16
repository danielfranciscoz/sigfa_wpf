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

namespace PruebaWPF.Views.AgenteExterno
{
    /// <summary>
    /// Lógica de interacción para AgenteExterno.xaml
    /// </summary>
    public partial class AgenteExterno : Page
    {
        

        private ObservableCollection<AgenteExternoCat> items;
        private Operacion operacion;
        public static Boolean Cambios = false;

        private Model.Pantalla pantalla;

        public AgenteExterno()
        {

            InitializeComponent();
        }

        public AgenteExterno(Pantalla pantalla)
        {
            this.pantalla = pantalla;

            //controller = new AgenteExternoViewModel(pantalla);
            operacion = new Operacion();

            InitializeComponent();
        }

        private void ResizeGrid()
        {
            tblAgenteExterno.Height = panelGrid.ActualHeight;

        }

        private AgenteExternoViewModel controller()
        {
            return new AgenteExternoViewModel(pantalla);
        }

        private async void LoadTable(string text)
        {
            try
            {
                items = await FindAsync(text);
                tblAgenteExterno.ItemsSource = items;
                ContarRegistros();
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = ex.Message, OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private Task<ObservableCollection<AgenteExternoCat>> FindAsync(String text)
        {
            if (text.Equals(""))
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<AgenteExternoCat>(controller().FindAll());
                });
            }
            else
            {
                return Task.Run(() =>
                {
                    return new ObservableCollection<AgenteExternoCat>(controller().FindByText(text)); ;
                });
            }
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
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    if (clsUtilidades.OpenDeleteQuestionMessage())
                    {
                        clsUtilidades.OpenMessage(Eliminar());
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private Operacion Eliminar()
        {

            Operacion o = new Operacion();
            try
            {
                controller().Eliminar((AgenteExternoCat)tblAgenteExterno.CurrentItem);

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
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    GestionarAgenteExterno ga = new GestionarAgenteExterno(pantalla, btnSave.Tag.ToString());
                    ga.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (controller().Authorize(((Button)sender).Tag.ToString()))
                {
                    AgenteExternoCat item = (AgenteExternoCat)tblAgenteExterno.CurrentItem;
                    //AgenteExternoCat Objeto = (AgenteExternoCat)item.Clone();
                    //Para no crear el clon, voy a crear un objeto a partir de este otro, parametro por parametro
                    //Esto es porque para crear el clon se debe crear la clase hijo que herede de AgenteExterno e implemente IClonneable, y es muy largo el cambio

                    Model.AgenteExternoCat objeto = new AgenteExternoCat() {
                        IdAgenteExterno=item.IdAgenteExterno,
                        Nombre=item.Nombre,
                        IdIdentificacion=item.IdIdentificacion,
                        Identificacion=item.Identificacion,
                        FechaCreacion=item.FechaCreacion,
                        Usuario=item.Usuario,
                        UsuarioCreacion=item.UsuarioCreacion,
                        IdentificacionAgenteExterno=item.IdentificacionAgenteExterno,
                        Procedencia=item.Procedencia,
                        regAnulado=item.regAnulado
                    };
                    GestionarAgenteExterno ga = new GestionarAgenteExterno(objeto, pantalla, ((Button)sender).Tag.ToString());
                    ga.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }
    }
}
