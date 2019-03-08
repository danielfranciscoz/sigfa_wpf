﻿using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.UserControls;
using PruebaWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Linq;
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
        private List<Perfil> perfiles;
        private List<PermisoSon> permisos;
        private bool isLastAccesDelete = false;
        public static bool Cambios = false;
        public static bool isCreated = false;
        public static bool isAddPermiso = false;

        public static Usuario selectedUser = null;
        public static Perfil selectedPerfil = null;
        public static Pantalla selectedPantallaPerfil = null;

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
                        if (perfiles == null || Cambios)
                        {
                            LoadListPerfiles(txtFindRol.Text);

                        }
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

        private async void LoadListPerfiles(string text)
        {
            try
            {
                perfiles = await FindAsyncPerfiles(text);
                lstRoles.ItemsSource = perfiles;
                lstRoles.Height = panelListaRoles.ActualHeight;

                if (selectedPerfil != null)
                {
                    lstRoles.SelectedItem = perfiles.Find(f => f.IdPerfil == selectedPerfil.IdPerfil);
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
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

                if (selectedUser != null)
                {
                    lstUsuarios.SelectedItem = usuarios.Find(f => f.Login == selectedUser.Login);

                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private Task<List<Usuario>> FindAsyncUsers(string text)
        {
            if (text.Equals("") || isCreated)
            {
                isCreated = false;
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

        private Task<List<Perfil>> FindAsyncPerfiles(string text)
        {
            if (text.Equals("") || isCreated)
            {
                isCreated = false;
                return Task.Run(() =>
                {
                    return controller().FindAllPerfiles();
                });
            }
            else
            {
                return Task.Run(() =>
                {
                    return controller().FindPerfilesByName(text);
                });
            }
        }


        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTables();
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
                if (panelInfoEmpty.Visibility != Visibility.Collapsed)
                {
                    panelInfoEmpty.Visibility = Visibility.Collapsed;
                }
                selectedUser = (Usuario)lstUsuarios.SelectedItem;
                CargarInfoUsuario(selectedUser);
            }
        }

        private void CargarInfoUsuario(Usuario u)
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
            CargarRolesUser(u);
        }

        private void CargarRolesUser(Usuario u)
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
                controller().DeletePerfilUsuario(usuarioPerfil);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                CargarRolesUser(selectedUser);
            }
            catch (Exception ex)
            {
                o.Mensaje = new clsException(ex).ErrorMessage();
                o.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            }

            return o;
        }

        private void btnAddPerfil_Click(object sender, RoutedEventArgs e)
        {
            GestionarUsuario gu = new GestionarUsuario(pantalla, selectedUser);
            gu.ShowDialog();
        }

        private void btn_Enable_Disable_User_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedUser.RegAnulado)
                {
                    if (clsutilidades.OpenDeleteQuestionMessage("Esta a punto de Reactivar el inicio de sesión del usuario " + selectedUser.Login + ", ¿Realmente desea continuar con esta acción?"))
                    {
                        clsutilidades.OpenMessage(ActivaDesactivaUsuario(selectedUser));
                    }
                }
                else
                {
                    if (clsutilidades.OpenDeleteQuestionMessage("Esta a punto de desactivar la cuenta del usuario " + selectedUser.Login + ", por lo tanto no podrá volver a iniciar sesión, ¿Realmente desea continuar con esta acción?"))
                    {
                        clsutilidades.OpenMessage(ActivaDesactivaUsuario(selectedUser));
                    }
                }

            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }

        private Operacion ActivaDesactivaUsuario(Usuario user)
        {
            Operacion o = new Operacion();
            try
            {
                user.RegAnulado = !user.RegAnulado;
                controller().ActivaDesactivaUsuario(user);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                CargarInfoUsuario(user);
            }
            catch (Exception ex)
            {
                o.Mensaje = new clsException(ex).ErrorMessage();
                o.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            }

            return o;
        }

        private void btnNewUser_Click(object sender, RoutedEventArgs e)
        {

            GestionarUsuario gu = new GestionarUsuario(pantalla);
            gu.ShowDialog();
        }

        private void btnEditTrabajador_Click(object sender, RoutedEventArgs e)
        {
            Usuario user = selectedUser;
            user.Nombre = lblNombreCompleto.Text;
            int parse = 0;

            int.TryParse(lblCodigo.Text, out parse);

            GestionarUsuario gu = new GestionarUsuario(pantalla, user, parse);
            gu.ShowDialog();
        }

        private void txtFindRol_KeyUp(object sender, KeyEventArgs e)
        {
            LoadListPerfiles(txtFindRol.Text);
        }

        private void lstRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstRoles.SelectedItem != null)
            {
                if (panelEmptyMenu.Visibility != Visibility.Collapsed)
                {
                    panelEmptyMenu.Visibility = Visibility.Collapsed;
                    panelEmptyAccess.Visibility = Visibility.Collapsed;
                }
                if (panelEmptyPermisos.Visibility != Visibility.Visible)
                {
                    panelEmptyPermisos.Visibility = Visibility.Visible;
                }
                selectedPerfil = (Perfil)lstRoles.SelectedItem;
                CargarPantallasPerfil(selectedPerfil);
            }
        }

        private void CargarPantallasPerfil(Perfil perfil)
        {//TODO Aun queda pendiente la carga de datos cuando se seleccionan filas de la lista de roles y del arbol de pantallas
            CargarMenu(perfil);
            tree.Height = panelListaPantallasRol.ActualHeight;

            CargarAccesos(perfil);
            lstAccesos.Height = panelListaAccesosDirectos.ActualHeight;
        }

        private void CargarAccesos(Perfil perfil)
        {
            List<AccesoDirectoPerfil> pantallas = controller().FindAccesosDirectos(perfil);
            lstAccesos.ItemsSource = pantallas;
        }

        private void CargarPermisos(Pantalla PantallaPerfil, Perfil perfil)
        {
            permisos = controller().FindPermisos(PantallaPerfil, perfil);
            tblPermisos.ItemsSource = permisos;
            tblPermisos.Height = panelListaPermisos.ActualHeight;

        }

        private void CargarMenu(Perfil perfil)
        {
            List<Pantalla> pantallas = controller().FindPantallas(perfil);

            TreeViewItem mi;
            tree.Items.Clear();
            foreach (Pantalla item in pantallas.Where(w => w.IdPadre == null))
            {
                mi = new TreeViewItem();
                mi.Header = item.Titulo;
                mi.Height = Double.NaN;
                mi.IsExpanded = true;
                mi.FontWeight = FontWeights.Bold;
                CargarSubMenu(item, pantallas, mi);

                if (mi.Items.Count == 0 && Uid != null)
                {
                    mi.DataContext = item;
                }
                tree.Items.Add(mi);
            }
        }

        private void CargarSubMenu(Pantalla padre, List<Pantalla> pantallas, TreeViewItem mi)
        {
            TreeViewItem subMi;

            foreach (Pantalla item in pantallas.Where(w => w.IdPadre == padre.IdPantalla))
            {
                subMi = new TreeViewItem();

                if (item.Tipo != null)
                {
                    if (!item.Tipo.Equals("Separador"))
                    {
                        subMi.Header = item.Titulo;
                        subMi.FontWeight = FontWeights.Normal;
                        subMi.DataContext = item;

                        if (selectedPantallaPerfil != null && isAddPermiso)
                        {
                            if (item.IdPantalla == selectedPantallaPerfil.IdPantalla)
                            {
                                subMi.IsSelected = true;
                                selectedPantallaPerfil = null;
                            }
                        }

                        mi.Items.Add(subMi);
                        mi.IsExpanded = true;
                        CargarSubMenu(item, pantallas, subMi);
                    }
                }
                else
                {
                    subMi.Header = item.Titulo;
                    subMi.Height = Double.NaN;
                    subMi.IsExpanded = true;
                    subMi.FontWeight = FontWeights.Bold;
                    mi.Items.Add(subMi);
                    CargarSubMenu(item, pantallas, subMi);
                }

            }

        }

        private void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tree.SelectedItem != null)
            {
                selectedPantallaPerfil = (Pantalla)((TreeViewItem)tree.SelectedItem).DataContext;

                if (selectedPantallaPerfil != null)
                {
                    if (panelEmptyPermisos.Visibility != Visibility.Collapsed)
                    {
                        panelEmptyPermisos.Visibility = Visibility.Collapsed;
                    }
                    CargarPermisos(selectedPantallaPerfil, selectedPerfil);
                }
                else
                {
                    if (panelEmptyPermisos.Visibility != Visibility.Visible)
                    {
                        panelEmptyPermisos.Visibility = Visibility.Visible;
                    }
                    if (tblPermisos.Items.Count > 0)
                    {
                        tblPermisos.ItemsSource = null;
                    }
                }
            }

        }

        private void btnDeleteAccess_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AccesoDirectoPerfil accesoDirecto = (AccesoDirectoPerfil)((Button)sender).DataContext;

                if (clsutilidades.OpenDeleteQuestionMessage())
                {
                    clsutilidades.OpenMessage(EliminarAccesoDirecto(accesoDirecto));
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void btnNewRol_Click(object sender, RoutedEventArgs e)
        {
            GestionarRoles gr = new GestionarRoles(pantalla);
            gr.ShowDialog();
        }

        private void btnEditRol_Click(object sender, RoutedEventArgs e)
        {
            if (lstRoles.SelectedItem != null)
            {
                GestionarRoles gr = new GestionarRoles(pantalla, selectedPerfil);
                gr.ShowDialog();
            }
            else
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
            }
        }

        private void btnDeleteRol_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (lstRoles.SelectedItem != null)
                {
                    var roles = selectedPerfil.UsuarioPerfil.Select(s => s.Login).Distinct();

                    if (roles.Count() > 0)
                    {
                        if (clsutilidades.OpenDeleteQuestionMessage(string.Format("Atención, hemos detectado que este perfil se encuentra asociado a {0} usuario(s), tenga en cuenta que si lo elimina hará que a estos usuarios les sea eliminado el acceso mediante este perfil y posiblemente no podrán ingresar al sistema, ¿Realmente desea continuar con esta acción?", roles.Count())))
                        {
                            clsutilidades.OpenMessage(EliminarRol(selectedPerfil));
                        }
                    }
                    else
                    {
                        if (clsutilidades.OpenDeleteQuestionMessage())
                        {
                            clsutilidades.OpenMessage(EliminarRol(selectedPerfil));
                        }

                    }
                }
                else
                {
                    clsutilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                }

            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }

        private Operacion EliminarAccesoDirecto(AccesoDirectoPerfil accesoDirecto)
        {
            Operacion o = new Operacion();
            try
            {
                controller().DeleteAccesoDirecto(accesoDirecto);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;

                CargarAccesos(selectedPerfil);

            }
            catch (Exception ex)
            {
                o.Mensaje = new clsException(ex).ErrorMessage();
                o.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            }

            return o;
        }

        private Operacion EliminarRol(Perfil p)
        {
            Operacion o = new Operacion();
            try
            {
                controller().DeletePerfil(p);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                selectedPerfil = null;
                LoadListPerfiles(txtFind.Text);

            }
            catch (Exception ex)
            {
                o.Mensaje = new clsException(ex).ErrorMessage();
                o.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            }

            return o;
        }

        private void btnNewMenu_Click(object sender, RoutedEventArgs e)
        {
            if (lstRoles.SelectedItem != null)
            {
                UpdateMenu um = new UpdateMenu(pantalla, selectedPerfil);
                um.ShowDialog();
            }
            else
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
            }
        }

        private void btnNewAcces_Click(object sender, RoutedEventArgs e)
        {
            if (lstRoles.SelectedItem != null)
            {
                UpdateMenu um = new UpdateMenu(pantalla, selectedPerfil, true);
                um.ShowDialog();
            }
            else
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
            }
        }

        private void btnNewPermiso_Click(object sender, RoutedEventArgs e)
        {
            AddPermiso ap = new AddPermiso(pantalla, selectedPerfil, selectedPantallaPerfil);
            ap.ShowDialog();
        }

        private void btnDeletePermiso_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Permiso permiso = (Permiso)tblPermisos.CurrentItem;

                if (MensajeConfirmacion(permiso))
                {
                    clsutilidades.OpenMessage(EliminarPermiso(permiso));
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }
        }

        private bool MensajeConfirmacion(Permiso permiso)
        {
            isLastAccesDelete = false;
            if (permiso.IdPermisoName == 1)
            {
                if (permisos.Where(w => w.IdPermisoName == 1).Count() == 1)
                {
                    isLastAccesDelete = true;
                    return clsutilidades.OpenDeleteQuestionMessage("Esta a punto de Eliminar por completo el acceso a esta pantalla, por lo tanto, también serán eliminados los accesos directos de esta pantalla en este perfil, ¿Realmente desea continuar con esta acción?");
                }
                else
                {
                    return clsutilidades.OpenDeleteQuestionMessage("Esta a punto de Eliminar por completo el acceso a esta pantalla, por lo tanto, los usuarios con este Perfil no podrán acceder a ella, ¿Realmente desea continuar con esta acción?");
                }
            }
            else
            {
                return clsutilidades.OpenDeleteQuestionMessage();
            }
        }

        private Operacion EliminarPermiso(Permiso permiso)
        {
            Operacion o = new Operacion();
            try
            {
                controller().DeletePermiso(permiso);

                o.Mensaje = clsReferencias.MESSAGE_Exito_Delete;
                o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                if (isLastAccesDelete)
                {
                    CargarMenu(selectedPerfil);
                    CargarAccesos(selectedPerfil);
                    selectedPantallaPerfil = null;
                    panelEmptyPermisos.Visibility = Visibility.Visible;
                }
                else
                {
                    CargarPermisos(selectedPantallaPerfil, selectedPerfil);
                }

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
