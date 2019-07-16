using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
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
using System.Windows.Shapes;

namespace PruebaWPF.Views.Administracion
{
    /// <summary>
    /// Lógica de interacción para GestionarUsuario.xaml
    /// </summary>
    public partial class GestionarUsuario : Window
    {
        AdministracionViewModel controller;
        Pantalla pantalla;
        clsValidateInput validate = new clsValidateInput();
        Usuario usuario;
        ObservableCollection<UsuarioPerfilSon> uperfiles;

        public GestionarUsuario()
        {
            InitializeComponent();
        }

        public GestionarUsuario(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            usuario = new Usuario();
            InitializeComponent();
            Inicializar();
        }

        public GestionarUsuario(Pantalla pantalla, Usuario usuario, int noInterno)
        {
            this.pantalla = pantalla;
            this.usuario = usuario;
            InitializeComponent();
            Inicializar();

            txtUser.IsEnabled = false;
            txtcodigoEmpleado.Text = noInterno.ToString();
            panel1.Width = new GridLength(0);
            panel2.Width = new GridLength(0);
            panel0.Width = new GridLength(350);
            txtTitle.Text = "Editar Usuario";
            btnSave.Visibility = btnEdit.Visibility;
            btnEdit.Visibility = Visibility.Visible;
        }
        public GestionarUsuario(Pantalla pantalla, Usuario usuario)
        {
            this.pantalla = pantalla;
            this.usuario = usuario;
            InitializeComponent();
            Inicializar();

            panel0.Width = new GridLength(0);
            panel1.Width = new GridLength(0);
            txtTitle.Text = "Asignar Perfiles";

        }
        private void Inicializar()
        {
            controller = new AdministracionViewModel();
            uperfiles = new ObservableCollection<UsuarioPerfilSon>();
            Diseñar();
            CargarCombo();
            DataContext = usuario;
            CamposNormales();
        }

        private void Diseñar()
        {
            clsUtilidades.Dialog_ModalDesign(this);
        }

        private void CargarCombo()
        {
            cboRecinto.ItemsSource = controller.Recintos();
        }

        private void CamposNormales()
        {
            validate.AsignarBorderNormal(new Control[] { txtUser, txtEmail, txtcodigoEmpleado, txtNombreCompleto });
        }

        private bool ValidarCampos()
        {
            if (clsValidateInput.ValidateALL(new Control[] { txtUser, txtEmail, txtcodigoEmpleado, txtNombreCompleto }))
            {
                if (clsValidateInput.ValidateMayorCero(new TextBox[] { txtcodigoEmpleado }))
                {
                    return clsValidateInput.ValidateEmail(txtEmail);

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool ValidarTrabajador()
        {
            return clsValidateInput.ValidateALL(new Control[] { txtcodigoEmpleado });
        }



        private void cboRecinto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboRecinto.SelectedIndex != -1)
            {
                CargarPerfiles();
            }
        }

        private void CargarPerfiles()
        {
            lstRoles.ItemsSource = controller.FindAllRolesUser(usuario, ((vw_RecintosRH)cboRecinto.SelectedItem).IdRecinto);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnAddRole_Click(object sender, RoutedEventArgs e)
        {
            Perfil p = (Perfil)((Button)sender).DataContext;
            AgregarPerfil(p);
        }

        private void AgregarPerfil(Perfil p)
        {
            vw_RecintosRH r = (vw_RecintosRH)cboRecinto.SelectedItem;

            UsuarioPerfilSon nuevo = new UsuarioPerfilSon();
            nuevo.Perfil = p;
            nuevo.IdPerfil = p.IdPerfil;
            nuevo.Recinto = r.Siglas;
            nuevo.IdRecinto = byte.Parse(r.IdRecinto.ToString());
            if (!uperfiles.Any(a => a.Perfil.IdPerfil == p.IdPerfil && a.IdRecinto == nuevo.IdRecinto))
            {
                uperfiles.Add(nuevo);
            }
        }

        private void btnDeleteRoleUser_Click(object sender, RoutedEventArgs e)
        {
            uperfiles.Remove((UsuarioPerfilSon)tblRolesUsuario.CurrentItem);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnSave.Visibility == Visibility.Visible && !string.IsNullOrEmpty(usuario.LoginCreacion))
                {
                    if (uperfiles.Count > 0)
                    {
                        clsUtilidades.OpenMessage(Guardar(), this);
                        Finalizar();
                    }
                    else
                    {
                        clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_Cero_Registro_Table, OperationType = clsReferencias.TYPE_MESSAGE_Error });
                    }
                }
                else
                {
                    if (ValidarCampos())
                    {
                        if (controller.ValidarUsuario(usuario))
                        {

                            if (btnEdit.Visibility == Visibility.Visible)
                            {
                                if (lastVerifyUserEmail())
                                {
                                    clsUtilidades.OpenMessage(Guardar(), this);
                                    Finalizar();
                                }
                            }
                            else
                            {
                                if (uperfiles.Count > 0)
                                {
                                    if (lastVerifyUserEmail())
                                    {
                                        clsUtilidades.OpenMessage(Guardar(), this);
                                        Finalizar();
                                    }

                                }
                                else
                                {
                                    clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_Cero_Registro_Table, OperationType = clsReferencias.TYPE_MESSAGE_Error });
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private bool lastVerifyUserEmail()
        {
            if (string.IsNullOrEmpty(usuario.LoginCreacion))
            {
                return clsUtilidades.OpenDeleteQuestionMessage(string.Format("Estamos listos para crear el nuevo usuario, pero antes por favor verifique si la siguiente información es correcta:\n\nUsuario:\t{0}\t(Este campo no podrá ser editado)\nCorreo:\t{1}\n\n¿Los datos son correctos?", usuario.Login, usuario.LoginEmail));
            }
            else
            {
                return clsUtilidades.OpenDeleteQuestionMessage(string.Format("Por favor verifique si la siguiente información es correcta:\n\nCorreo:\t{0}\n\n¿Los datos son correctos?", usuario.LoginEmail));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tblRolesUsuario.ItemsSource = uperfiles;
        }

        private void btnFindEmpleado_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarTrabajador())
            {
                BuscarTrabajador();
            }
        }

        private void BuscarTrabajador()
        {
            try
            {
                vwEmpleadosRH trabajador = controller.ObtenerTrabajador(txtcodigoEmpleado.Text);
                usuario.Nombre = string.Format("{0} {1}", trabajador.Nombres, trabajador.Apellidos);
                usuario.noInterno = int.Parse(trabajador.Cod_Interno.ToString());
                clsUtilidades.UpdateControl(txtNombreCompleto);
                txtNombreCompleto.Focus();
                cboRecinto.Focus();
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void txtcodigoEmpleado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (ValidarTrabajador())
                {
                    BuscarTrabajador();
                }
            }
            else
            {
                bool d = string.IsNullOrEmpty(usuario.Nombre);
                if (!string.IsNullOrEmpty(usuario.Nombre))
                {
                    usuario.Nombre = "";
                    clsUtilidades.UpdateControl(txtNombreCompleto);
                }
            }

        }


        private void Finalizar()
        {
            Administracion.Cambios = true;
            Administracion.isCreated = true;

            frmMain.Refrescar();
            Close();
        }

        private Operacion Guardar()
        {
            if (string.IsNullOrEmpty(usuario.LoginCreacion))
            {
                Administracion.selectedUser = controller.SaveUser(usuario, uperfiles.ToList());
            }
            else
            {
                if (btnSave.Visibility == Visibility.Visible)
                {
                    controller.UpdateRoles(usuario.Login, uperfiles.ToList());
                    Administracion.selectedUser = usuario;
                }
                else
                {
                    Administracion.selectedUser = controller.UpdateUser(usuario);
                }

            }
            return new Operacion { Mensaje = clsReferencias.MESSAGE_Exito_Save, OperationType = clsReferencias.TYPE_MESSAGE_Exito };
        }
    }
}
