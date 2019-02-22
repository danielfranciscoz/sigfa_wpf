using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.UserControls;
using PruebaWPF.ViewModel;
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

namespace PruebaWPF.Views.Administracion
{
    /// <summary>
    /// Lógica de interacción para Administracion.xaml
    /// </summary>
    public partial class Administracion : Page
    {
        private Pantalla pantalla;
        private List<Usuario> usuarios;

        public static Boolean Cambios = false;

        public Administracion()
        {
            InitializeComponent();
        }

        public Administracion(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            InitializeComponent();
        }


        private AdministracionViewModel controller()
        {
            return new AdministracionViewModel(pantalla);
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
                        if (usuarios == null || Cambios)
                        {
                            LoadUsuarios(txtFind.Text);

                        }
                        break;
                    case 1:
                        //if (tarjetas == null || bancos == null || formaspago == null || Cambios)
                        //{
                        //    CargarPesataña2();

                        //}
                        break;
                    case 2:
                        //if (fuentes == null || monedas == null || identificaciones == null || Cambios)
                        //{
                        //    CargarPesataña3();
                        //}
                        break;
                    case 3:
                        //if (infoRecibos == null || Cambios)
                        //{
                        //    CargarPesataña4();
                        //}
                        break;
                }
            }
            else
            {
                LoadUsuarios(txtFind.Text);
            }
        }



        private async void LoadUsuarios(string text)
        {
            try
            {
                usuarios = await FindAsyncUsers(text);
                if (lstUsuarios.ItemsSource == null)
                {
                    lstUsuarios.Height = panelListaUsuarios.ActualHeight;

                }
                lstUsuarios.ItemsSource = usuarios;
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private Task<List<Usuario>> FindAsyncUsers(string text)
        {
            if (text.Equals(""))
            {
                return Task.Run(() =>
                {
                    return controller().FindAllUsers();
                });
            }
            else
            {
                return Task.Run(() =>
                {
                    return controller().FindUsersByName(text);
                });
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExportar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Exportar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lstUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstUsuarios.SelectedItem != null)
            {
                panelInfoEmpty.Visibility = Visibility.Collapsed;
                CargarInfo((Usuario)lstUsuarios.SelectedItem);
            }
        }

        private void CargarInfo(Usuario u)
        {
            InfoUsuario info = controller().ObtenerInfoUsuario(u);
            if (info.foto == null)
            {
                lblNoFoto.Visibility = Visibility.Visible;
                lblFoto.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblNoFoto.Visibility = Visibility.Collapsed;
                lblFoto.Visibility = Visibility.Visible;
            }
            panelInfo.DataContext = info;
            CargarRoles(u);
        }

        private void CargarRoles(Usuario u)
        {
            tblRolesUsuario.ItemsSource = controller().ObtenerPerfilesUsuario(u);
            tblRolesUsuario.Height = panelRolesUsuario.ActualHeight;
        }

        private void txtFindText(object sender, KeyEventArgs e)
        {
            LoadUsuarios(txtFind.Text);
        }

        private void btnDeleteRoleUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UsuarioPerfil usuarioPerfil = (UsuarioPerfil)tblRolesUsuario.CurrentItem;

                ////OJO NO VOY A USAR SEGURIDAD EN ESTA PANTALLA DEBIDO A QUE TODO LO GESTIONADO CORRESPONDE A LA ADMINISTRACION COMO SUPER USUARIO
                //if (controller().Autorice(((Button)sender).Tag.ToString()))
                //{
                if (tblRolesUsuario.Items.Count > 1)
                {
                    if (clsutilidades.OpenDeleteQuestionMessage())
                    {
                        clsutilidades.OpenMessage(EliminarRolUsuario(usuarioPerfil));
                    }
                }
                else
                {
                    clsutilidades.OpenMessage(new Operacion() { Mensaje = "No podemos eliminar este perfil, debido a que es el único con el que cuenta el usuario", OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                }

                //}
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }

        private Operacion EliminarRolUsuario(UsuarioPerfil usuarioPerfil)
        {
            Operacion o = new Operacion();
            try
            {
                controller().EliminarPerfilUsuario(usuarioPerfil);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                CargarRoles((Usuario)lstUsuarios.SelectedItem);
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
