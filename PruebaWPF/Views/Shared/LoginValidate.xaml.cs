using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PruebaWPF.Views.Shared
{
    /// <summary>
    /// Lógica de interacción para LoginValidate.xaml
    /// </summary>
    public partial class LoginValidate : Window
    {
        LoginViewModel controller = new LoginViewModel();
        clsValidateInput validate = new clsValidateInput();
        List<Usuario> cajerosPermitidos;
        int ValidationType = 0;
        public string cajero { get; set; }

        private enum validationsType
        {
            CajeroPermito = 1
        }

        public LoginValidate(String mensaje, List<Usuario> cajerosPermitidos)
        {
            ValidationType = (int)validationsType.CajeroPermito;
            InitializeComponent();
            Diseñar();

            txtMensaje.Text = mensaje;
            this.cajerosPermitidos = cajerosPermitidos;
            CamposNormales();
        }

        private void Diseñar()
        {
            clsUtilidades.Dialog_ModalDesign(this);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Iniciar();
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = ex.Message, OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void CamposNormales()
        {
            validate.AsignarBorderNormal(new Control[] { txtUsuario, txtPassword });
        }

        private bool ValidarCamposCredenciales()
        {
            return clsValidateInput.ValidateALL(new Control[] { txtUsuario, txtPassword });
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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

        private async void Iniciar()
        {

            if (ValidarCamposCredenciales())
            {
                progressbar.Visibility = Visibility.Visible;
                btnAceptar.IsEnabled = false;
                string user = txtUsuario.Text;
                bool result = await ValidarCredenciales(user, txtPassword.Password);

                if (result)
                {
                    SegundaValidacionDynamic(user);
                }
                else
                {
                    PanelError(clsReferencias.MESSAGE_Wrong_User);
                }
                btnAceptar.IsEnabled = true;
                progressbar.Visibility = Visibility.Hidden;
            }
        }

        private void SegundaValidacionDynamic(String user)
        {
            switch (ValidationType)
            {
                case (int)validationsType.CajeroPermito:
                    if (VerificarCajerosPermitidos(user))
                    {
                        cajero = user;
                        Close();
                    }
                    else
                    {
                        PanelError(clsReferencias.MESSAGE_CajeroNoAutorizado);
                    }
                    break;

                default:
                    break;
            }
        }

        private bool VerificarCajerosPermitidos(string text)
        {
            if (cajerosPermitidos.Count > 0)
            {
                return cajerosPermitidos.Exists(e => e.Login == text);
            }
            else
            {
                return controller.isCajero(text, null);
            }
        }

        public Task<bool> ValidarCredenciales(String Usuario, String Password)
        {
            return Task.Run(() => controller.ValidarCredenciales(Usuario, Password));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PanelError(string Mensaje)
        {
            panelError.Visibility = Visibility.Visible;
            lblError.Text = Mensaje;
        }
    }
}
