
using Confortex.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;



namespace PruebaWPF.Views.Acceso
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoginViewModel controller = new LoginViewModel();
        public MainWindow()
        {
            InitializeComponent();
            txtUsuario.Text = "dfzamora";
            txtPassword.Password = "zamora2112u";

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void txtUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (txtUsuario.Text.Length > 0)
                {
                    txtPassword.Focus();
                }
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Iniciar();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Iniciar();
            }
            catch (Exception ex)
            {
                OpenDialog(new clsException(ex).ErrorMessage());
            }
        }

        private void Iniciar()
        {
            if (txtUsuario.Text.Length > 0 && txtPassword.Password.Length > 0)
            {
                ValidarCredenciales(txtUsuario.Text, txtPassword.Password);

            }
            else
            {
                txtMensaje.Text = InformacionNecesaria((txtUsuario.Text.Length == 0) ? "Usuario" : "Contraseña");
                msgDialog.IsOpen = true;
            }
        }

        private void ValidarCredenciales(String Usuario, String Password)
        {
            if (Usuario.Contains("@")) // Verificando la autenticación con Office365
            {
                var credencialesOffice = new wsOffice365.authSoapClient();

                if (credencialesOffice.Validate(Usuario, Password))
                {
                    //Credenciales correctas
                    VerificarRoles(Usuario);
                }
                else
                {
                    WrongUserPass();
                }
            }
            else //Verificando la autenticación con LDAP
            {
                var credencialesLdap = new wsLDAP.LDAPSoapClient();

                if (credencialesLdap.EsUsuarioValido(Usuario, Password))
                {
                    //Credenciales correctas
                    VerificarRoles(Usuario);
                }
                else
                {
                    WrongUserPass();
                }
            }
        }

        private void VerificarRoles(String Usuario)
        {
            sbyte VerificaPerfiles = SoloEsCajero(Usuario);

            if (VerificaPerfiles == -1) // Indica que el usuario no posee perfiles en el sistema, por lo tanto no puede acceder
            {
                NoAccess();
                return;
            }

            if (VerificaPerfiles == 1) //Si solo tiene perfil de cajero entonces directamente se inicia el sistema
            {
                IniciarMain();
            }
            else
            {
                panel_Credenciales.Visibility = Visibility.Hidden;
                panel_Periodo.Visibility = Visibility.Visible;

                CargarProgramas(Usuario);
                CargarPeriodosEspecificos();
            }

        }

        private SByte SoloEsCajero(String Usuario)
        {
            List<UsuarioPerfil> perfiles = controller.ObtenerPerfilesUsuario(Usuario);
            if (perfiles.Count == 0)
            {
                return -1;
            }

            //Asigno el usuario y sus perfiles a la sesión
            controller.SeleccionarPerfilUsuario(perfiles);

            if (perfiles.Count == 1 && perfiles.First().IdPerfil == clsReferencias.PerfilCajero)
            {
                //Aqui se que es cajero, por lo tanto solo puede acceder al periodo activo vigente 
                controller.SeleccionarPeriodo(clsReferencias.Default);
                return 1;
            }
            else
            {
                return 0;
            }

        }

        private void CargarProgramas(String Usuario)
        {
            var programa = controller.ObtenerProgramas(Usuario);
            cboPrograma.ItemsSource = programa;
        }

        private void CargarPeriodosEspecificos()
        {
            var periodoEsp = controller.ObtenerPeriodosEspecificos().Select(a => new { a.IdPeriodoEspecifico, Contenido = "Periodo: " + a.FechaInicio.Month.ToString() + "/" + a.FechaInicio.Year.ToString() + " - " + a.Periodo, a.Estado });
            cboPeriodo.ItemsSource = periodoEsp;
            cboPeriodo.DisplayMemberPath = "Contenido";
            cboPeriodo.SelectedValuePath = "IdPeriodoEspecifico";
        }

        private bool verficarProgramaPeriodo()
        {
            if (cboPeriodo.SelectedIndex == -1 || cboPrograma.SelectedIndex == -1)
            {
                return false;
            }
            else
            {
                //Asigno a la sesión el programa y periodo específico seleccionado
                controller.SeleccionarPrograma(cboPrograma.SelectedValue.ToString());
                controller.SeleccionarPeriodo(cboPeriodo.SelectedValue.ToString());

                return true;
            }
        }

        private string InformacionNecesaria(String campo)
        {
            return "Debe completar el campo " + campo + " para poder continuar";
        }

        private string SeleccionNecesaria(String campo)
        {
            return "Debe seleccionar el " + campo + " para poder continuar";
        }

        private void WrongUserPass()
        {
            OpenDialog("Usuario o contraseña incorrecta");
        }

        private void NoAccess()
        {
            OpenDialog("El usuario no posee accesos al sistema");
        }

        private void OpenDialog(string Message)
        {
            txtMensaje.Text = Message;
            msgDialog.IsOpen = true;
        }

        private void btnAceptarRol_Click(object sender, RoutedEventArgs e)
        {
            if (verficarProgramaPeriodo())
            {
                IniciarMain();
            }
            else
            {
                txtMensaje.Text = SeleccionNecesaria((cboPrograma.SelectedIndex == -1) ? "Programa" : "Periodo");
                msgDialog.IsOpen = true;
            }


        }

        private void IniciarMain()
        {
            controller.RecintosMemory(); //Cargo los recintos en memoria, para no estar realizando peticiones a la base de datos cuando haga validaciones de autorización
            this.Hide();
            frmMain main = new frmMain();
            main.Show();
        }

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            panel_Credenciales.Visibility = Visibility.Visible;
            panel_Periodo.Visibility = Visibility.Hidden;
        }

    }
}
