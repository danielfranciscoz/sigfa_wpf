
using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;



namespace PruebaWPF.Views.Acceso
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoginViewModel controller = new LoginViewModel();
        clsValidateInput validate = new clsValidateInput();

        public MainWindow()
        {
            InitializeComponent();
            txtUsuario.Text = clsConfiguration.Actual().userRemember;
            txtPassword.Password = "zamora2112u";
            CamposNormales();
        }

        private void CamposNormales()
        {
            validate.AsignarBorderNormal(new Control[] { txtUsuario, txtPassword, cboPeriodo, cboPrograma });
        }

        private bool ValidarCamposCredenciales()
        {
            return clsValidateInput.ValidateALL(new Control[] { txtUsuario, txtPassword });
        }

        private bool ValidarCombos()
        {
            return clsValidateInput.ValidateALL(new Control[] { cboPeriodo, cboPrograma });
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

        private async void Iniciar()
        {

            if (ValidarCamposCredenciales())
            {
                progressbar.Visibility = Visibility.Visible;
                btnAceptar.IsEnabled = false;
                bool result = await ValidarCredenciales(txtUsuario.Text, txtPassword.Password);

                if (result)
                {
                    //Credenciales correctas
                    VerificarRoles(txtUsuario.Text);
                }
                else
                {
                    WrongUserPass();
                }
                btnAceptar.IsEnabled = true;
                progressbar.Visibility = Visibility.Hidden;
            }


        }

        private Task<bool> ValidarCredenciales(String Usuario, String Password)
        {
            if (Usuario.Contains("@")) // Verificando la autenticación con Office365
            {
                var credencialesOffice = new wsOffice365.authSoapClient();
                return Task.Run(() => credencialesOffice.Validate(Usuario, Password));
            }
            else //Verificando la autenticación con LDAP
            {
                var credencialesLdap = new wsLDAP.LDAPSoapClient();
                return Task.Run(() => credencialesLdap.EsUsuarioValido(Usuario, Password));
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

            IniciarMain();

            //Se retira la seleccion de periodo y programa, acordado en reunion el dia 06/06/2019

            //if (VerificaPerfiles == 1) //Si solo tiene perfil de cajero entonces directamente se inicia el sistema
            //{
            //    IniciarMain();
            //}
            //else
            //{
            //    panel_Credenciales.Visibility = Visibility.Hidden;
            //    panel_Periodo.Visibility = Visibility.Visible;

            //    CargarProgramas();
            //    CargarPeriodosEspecificos();
            //}

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

        private void CargarProgramas()
        {
            var programa = controller.ObtenerProgramas(clsSessionHelper.usuario.Login);
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
            if (ValidarCombos())
            {
                //Asigno a la sesión el programa y periodo específico seleccionado
                controller.SeleccionarPrograma(cboPrograma.SelectedValue.ToString());
                controller.SeleccionarPeriodo(cboPeriodo.SelectedValue.ToString());
                return true;
            }
            else
            {
                return false;

            }
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
        }

        private void IniciarMain()
        {
            controller.RecintosMemory(); //Cargo los recintos en memoria, para no estar realizando peticiones a la base de datos cuando haga validaciones de autorización
            controller.AreasMemory();
            controller.MacMemory();
            this.Hide();

            clsConfiguration.saveUser(txtUsuario.Text);

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
