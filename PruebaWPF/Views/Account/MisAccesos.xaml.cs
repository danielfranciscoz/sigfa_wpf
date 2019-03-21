using PruebaWPF.Clases;
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

namespace PruebaWPF.Views.Account
{
    /// <summary>
    /// Lógica de interacción para MisAccesos.xaml
    /// </summary>
    public partial class MisAccesos : Window
    {
        ObservableCollection<AccesoDirectoUsuarioSon> accesos;
        AccountViewModel controller;

        public MisAccesos()
        {
            InitializeComponent();
            Inicializar();
        }

        private void Inicializar()
        {
            controller = new AccountViewModel();
            Diseñar();
        }
        private void Diseñar()
        {
            clsutilidades.Dialog_ModalDesign(this);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                clsutilidades.OpenMessage(Guardar(), this);
                Finalizar();
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void Finalizar()
        {
            frmMain.reloadMain();
            Close();
        }

        private Operacion Guardar()
        {
            string result = controller.SaveAccesosDirectos(accesos.ToList());
            return new Operacion { Mensaje = result, OperationType = clsReferencias.TYPE_MESSAGE_Exito };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarAccesos();
        }

        private void CargarAccesos()
        {
            accesos = new ObservableCollection<AccesoDirectoUsuarioSon>(controller.AccesosDirectoUsuario());
            tblAccesosDirectos.ItemsSource = accesos;
        }
    }
}
