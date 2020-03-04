using PruebaWPF.Clases;
using System.Windows;

namespace PruebaWPF.Views.Shared
{
    /// <summary>
    /// Lógica de interacción para Loading.xaml
    /// </summary>
    public partial class Loading : Window
    {

        public Loading()
        {
            InitializeComponent();
            Diseñar();
        }

        public Loading(bool isPago) //true es pagando, false anulando
        {
            InitializeComponent();
            Diseñar();
            OcultarIconos(isPago);
        }

        private void OcultarIconos(bool isPago)
        {
            if (!isPago)
            {
                iconoPanel.Visibility = Visibility.Collapsed;
                icono2.Visibility = Visibility.Visible;
                txtMensaje.Text = "ESTAMOS ANULANDO LA TRANSACCIÓN, POR FAVOR ESPERE";
            }
        }

        private void Diseñar()
        {
            clsUtilidades.Dialog_ModalDesign(this);
        }
        public void Show(string tt)
        {
            Show();
        }
    }
}
