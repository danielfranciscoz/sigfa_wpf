﻿
using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
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
            //string VersionCSharp = (typeof(string).Assembly.ImageRuntimeVersion);
            InitializeComponent();
            txtUsuario.Text = clsConfiguration.Actual().userRemember;
            lblYear.Text = DateTime.Now.Year.ToString();
            CamposNormales();
            Ensamblados();
            //       new Window1().Show();
            var a = GetDateTime();
        }

        private void Ensamblados()
        {
            clsSessionHelper.SystemName = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;
            clsSessionHelper.SystemVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            TipoEntorno();

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
                bool result = false;
                try
                {
                    result = await ValidarCredenciales(txtUsuario.Text, txtPassword.Password);

                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(EndpointNotFoundException))
                    {
                        WrongUserPass("No se ha podido establecer comunicación con el servidor de autenticación de credenciales, es posible que no haya conexión a internet o que exista un bloqueo interno, en caso de tener internet porfavor contacte al administrador del sistema para solicitar asistencia.");

                    }
                    else
                    {
                        WrongUserPass("Ha ocurrido un error al intentar comunicarnos con el servicio de autenticación de la UNI, revise su conexión a internet o contacte al administrador del sistema.");

                    }

                    btnAceptar.IsEnabled = true;
                    progressbar.Visibility = Visibility.Hidden;
                    return;
                }


                if (result)
                {
                    //Credenciales correctas
                    VerificarRoles(txtUsuario.Text);
                    //VerificarRoles("gdvelez");
                }
                else
                {
                    WrongUserPass();
                }
                btnAceptar.IsEnabled = true;
                progressbar.Visibility = Visibility.Hidden;
            }


        }

        public Task<bool> ValidarCredenciales(String Usuario, String Password)
        {
            return Task.Run(() => controller.ValidarCredenciales(Usuario, Password));
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
            //var programa = controller.ObtenerProgramas(clsSessionHelper.usuario.Login);
            //cboPrograma.ItemsSource = programa;
        }

        //private void CargarPeriodosEspecificos()
        //{
        //    var periodoEsp = controller.ObtenerPeriodosEspecificos().Select(a => new { a.IdPeriodoEspecifico, Contenido = "Periodo: " + a.FechaInicio.Month.ToString() + "/" + a.FechaInicio.Year.ToString() + " - " + a.Periodo, a.Estado });
        //    cboPeriodo.ItemsSource = periodoEsp;
        //    cboPeriodo.DisplayMemberPath = "Contenido";
        //    cboPeriodo.SelectedValuePath = "IdPeriodoEspecifico";
        //}

        public static DateTime GetDateTime()
        {
            DateTime dateTime = DateTime.MinValue;
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://www.microsoft.com");
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string todaysDates = response.Headers["date"];

                dateTime = DateTime.ParseExact(todaysDates, "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                    System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, System.Globalization.DateTimeStyles.AssumeUniversal);
            }

            return dateTime;
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

        private void WrongUserPass(string mensaje = null)
        {
            OpenDialog(mensaje ?? clsReferencias.MESSAGE_Wrong_User);
        }

        private void NoAccess()
        {
            OpenDialog(clsReferencias.MESSAGE_User_NoAccess);
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
            controller.AsignarServer();
            controller.MacMemory(); //Cargo en memoria el MAC de la computadora en la que se incia sesion
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


        void TipoEntorno()
        {
            string BuildType;
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(DebuggableAttribute), true);

            if (attributes.Length > 0)
            {
                DebuggableAttribute debuggableAttribute = attributes[0] as DebuggableAttribute;
                BuildType = debuggableAttribute.IsJITOptimizerDisabled ? clsReferencias.Debug : clsReferencias.Release;
            }
            else
            {
                BuildType = clsReferencias.Release;
            }

            clsSessionHelper.entorno = BuildType;
        }

    }
}
